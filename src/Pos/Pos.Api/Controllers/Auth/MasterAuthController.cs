using System.Text;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.WebUtilities;

namespace FoodSphere.Pos.Api.Controllers;

/// <see cref="IdentityApiEndpointRouteBuilderExtensions.MapIdentityApi"/>
/// <see cref="Microsoft.AspNetCore.Authentication.BearerToken.BearerTokenOptions"/>
/// <see cref="Microsoft.AspNetCore.Authentication.IAuthenticationService"/>
[Route("auth/master")]
public class MasterAuthController(
    ILogger<MasterAuthController> logger,
    Utilities.EmailService emailSender,
    UserManager<MasterUser> userManager,
    MasterAuthService authService
) : FoodSphereControllerBase
{
    readonly EmailAddressAttribute emailAddressAttribute = new();

    /// <summary>
    /// Register a master user.
    /// </summary>
    [HttpPost]
    public async Task<Results<
        Created,
        ValidationProblem
    >> Register(MasterRegisterRequest body)
    {
        if (string.IsNullOrEmpty(body.email) || !emailAddressAttribute.IsValid(body.email))
        {
            return CreateValidationProblem(
                IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(body.email))
            );
        }

        var user = new MasterUser()
        {
            UserName = body.email,
            Email = body.email,
        };

        var result = await userManager.CreateAsync(user, body.password);

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        await SendConfirmationEmailAsync(user, body.email);

        return TypedResults.Created();
    }

    /// <summary>
    /// Login master user
    /// </summary>
    [HttpPost("token")]
    public async Task<Results<
        Ok<MasterTokenResponse>,
        ProblemHttpResult,
        UnauthorizedHttpResult
    >> GenerateToken(MasterTokenRequest body)
    {
        var user = await userManager.FindByEmailAsync(body.email);
        if (user is null)
            return TypedResults.Unauthorized();

        if (!await userManager.CheckPasswordAsync(user, body.password))
            return TypedResults.Unauthorized();

        if (await authService.IsTwoFactorEnabledAsync(user))
        {
            if (string.IsNullOrEmpty(body.two_factor_code))
            {
                return TypedResults.Problem(
                   "Two-factor required.",
                   statusCode: StatusCodes.Status401Unauthorized
               );
            }

            // token providers was registered in IdentityBuilderExtensions.AddDefaultTokenProviders() on Program.cs
            if (await userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, body.two_factor_code))
                return TypedResults.Unauthorized();
        }

        var token = await authService.GenerateToken(user);

        var response = new MasterTokenResponse
        {
            access_token = token
            // refresh_token =
        };

        return TypedResults.Ok(response);
    }

    [HttpGet("email/confirm")]
    public async Task<Results<
        ContentHttpResult,
        UnauthorizedHttpResult
    >> ConfirmEmail(string user_id, string code, string? changed_email)
    {
        if (await userManager.FindByIdAsync(user_id) is not { } user)
        {
            return TypedResults.Unauthorized();
        }

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return TypedResults.Unauthorized();
        }

        IdentityResult result;

        if (string.IsNullOrEmpty(changed_email))
        {
            result = await userManager.ConfirmEmailAsync(user, code);
        }
        else
        {
            result = await userManager.ChangeEmailAsync(user, changed_email, code);

            if (result.Succeeded)
            {
                result = await userManager.SetUserNameAsync(user, changed_email);
            }
        }

        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Text("Thank you for confirming your email.");
    }

    [HttpPost("email/comfirm/resend")]
    public async Task<Ok> ResendEmail(SendEmailRequest body)
    {
        if (await userManager.FindByEmailAsync(body.email) is not { } user)
        {
            return TypedResults.Ok();
        }

        await SendConfirmationEmailAsync(user, body.email);
        return TypedResults.Ok();
    }

    [HttpPost("password/forgot")]
    public async Task<Results<
        Ok, ValidationProblem
        >> ForgotPassword(ForgotPasswordRequest body)
    {
        var user = await userManager.FindByEmailAsync(body.email);

        if (user is not null && await userManager.IsEmailConfirmedAsync(user))
        {
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            await emailSender.SendPasswordResetCodeAsync(user, body.email, HtmlEncoder.Default.Encode(code));
        }

        // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
        // returned a 400 for an invalid code given a valid user email.
        return TypedResults.Ok();
    }

    [HttpPost("password/reset")]
    public async Task<Results<
        Ok, ValidationProblem
        >> ResetPassword(ResetPasswordRequest body)
    {
        var user = await userManager.FindByEmailAsync(body.email);

        if (user is null || !await userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed, so don't return a 200 if we would have
            // returned a 400 for an invalid code given a valid user email.
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        }

        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(body.reset_code));
            result = await userManager.ResetPasswordAsync(user, code, body.new_password);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }

        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    [Authorize]
    [HttpGet("info")]
    public async Task<Results<
        Ok<InfoResponse>,
        ValidationProblem,
        NotFound
        >> GetInfo()
    {
        if (await userManager.GetUserAsync(User) is not MasterUser user)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(await CreateInfoResponseAsync(user));
    }

    [Authorize]
    [HttpPost("info")]
    public async Task<Results<
        Ok<InfoResponse>,
        ValidationProblem,
        NotFound
        >> PostInfo(InfoRequest body)
    {
        if (await userManager.GetUserAsync(User) is not MasterUser user)
        {
            return TypedResults.NotFound();
        }

        if (!string.IsNullOrEmpty(body.new_email) && !emailAddressAttribute.IsValid(body.new_email))
        {
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(body.new_email)));
        }

        if (!string.IsNullOrEmpty(body.new_password))
        {
            if (string.IsNullOrEmpty(body.old_password))
            {
                return CreateValidationProblem("OldPasswordRequired",
                    "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
            }

            var changePasswordResult = await userManager.ChangePasswordAsync(user, body.old_password, body.new_password);
            if (!changePasswordResult.Succeeded)
            {
                return CreateValidationProblem(changePasswordResult);
            }
        }

        if (!string.IsNullOrEmpty(body.new_email))
        {
            var email = await userManager.GetEmailAsync(user);

            if (email != body.new_email)
            {
                await SendConfirmationEmailAsync(user, body.new_email, isChange: true);
            }
        }

        return TypedResults.Ok(await CreateInfoResponseAsync(user));
    }

    async Task SendConfirmationEmailAsync(MasterUser user, string email, bool isChange = false)
    {
        var code = isChange
            ? await userManager.GenerateChangeEmailTokenAsync(user, email)
            : await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var userId = await userManager.GetUserIdAsync(user);
        var routeValues = new RouteValueDictionary()
        {
            ["userId"] = userId,
            ["code"] = code,
        };

        if (isChange)
        {
            // This is validated by the /confirmEmail endpoint on change.
            routeValues.Add("changedEmail", email);
        }

        var confirmEmailUrl = Url.Action(nameof(ConfirmEmail), routeValues)!;

        await emailSender.SendConfirmationLinkAsync(user, email, HtmlEncoder.Default.Encode(confirmEmailUrl));
    }

    static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
        TypedResults.ValidationProblem(
            new Dictionary<string, string[]> { { errorCode, [errorDescription] } }
        );

    static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    async Task<InfoResponse> CreateInfoResponseAsync(MasterUser user)
    {
        return new()
        {
            email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            is_email_confirmed = await userManager.IsEmailConfirmedAsync(user),
            is_two_factor_enabled = await userManager.GetTwoFactorEnabledAsync(user),
        };
    }
}
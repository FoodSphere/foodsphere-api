namespace FoodSphere.Pos.Api.DTO;

public record MasterRegisterRequest
{
    /// <example>swagger@foodsphere.com</example>
    public required string email { get; init; }

    /// <example>swaggerpassword</example>
    public required string password { get; init; }
}

public record MasterTokenRequest
{
    /// <example>swagger@foodsphere.com</example>
    public required string email { get; init; }

    /// <example>swaggerpassword</example>
    public required string password { get; init; }

    public string? two_factor_code { get; init; }
}

public record MasterTokenResponse
{
    public required string access_token { get; init; }

    // public required string refresh_token { get; init; }
}

public record RefreshTokenRequest
{
    public required string refresh_token { get; init; }
}

public record RefreshTokenResponse
{
    public required string access_token { get; init; }
}

public record SendEmailRequest
{
    public required string email { get; init; }
}

public record ResetPasswordRequest
{
    public required string email { get; init; }
    public required string reset_code { get; init; }
    public required string new_password { get; init; }
}

public record InfoRequest
{
    public string? new_email { get; init; }
    public string? new_password { get; init; }
    public string? old_password { get; init; }
    public bool? enable_two_factor { get; init; }
}

public record InfoResponse
{
    public required string email { get; init; }
    public required bool is_email_confirmed { get; init; }
    public required bool is_two_factor_enabled { get; init; }
}

public record ForgotPasswordRequest
{
    public required string email { get; init; }
}

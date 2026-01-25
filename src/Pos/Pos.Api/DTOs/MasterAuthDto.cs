namespace FoodSphere.Pos.Api.DTOs;

public class MasterRegisterRequest
{
    public required string email { get; init; }
    public required string password { get; init; }
}

public class MasterTokenRequest
{
    public required string email { get; init; }
    public required string password { get; init; }
    public string? two_factor_code { get; init; }
}

public class MasterTokenResponse
{
    public required string access_token { get; init; }

    // public required string refresh_token { get; init; }
}

public class RefreshTokenRequest
{
    public required string refresh_token { get; init; }
}

public class RefreshTokenResponse
{
    public required string access_token { get; init; }
}

public class SendEmailRequest
{
    public required string email { get; init; }
}

public class ResetPasswordRequest
{
    public required string email { get; init; }
    public required string reset_code { get; init; }
    public required string new_password { get; init; }
}

public class InfoRequest
{
    public string? new_email { get; init; }
    public string? new_password { get; init; }
    public string? old_password { get; init; }
    public bool? enable_two_factor { get; init; }
}

public class InfoResponse
{
    public required string email { get; init; }
    public required bool is_email_confirmed { get; init; }
    public required bool is_two_factor_enabled { get; init; }
}

public sealed class ForgotPasswordRequest
{
    public required string email { get; init; }
}

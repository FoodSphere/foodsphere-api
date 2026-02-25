using Microsoft.AspNetCore.Identity;

namespace FoodSphere.Pos.Api.Utility;

public class EmailService(
    ILogger<EmailService> logger
) : IEmailSender<MasterUser>
{
    public async Task SendConfirmationLinkAsync(MasterUser user, string email, string confirmationLink)
    {
        logger.LogInformation("SendConfirmationLinkAsync {email}: {link}", email, confirmationLink);
    }

    public async Task SendPasswordResetLinkAsync(MasterUser user, string email, string resetLink)
    {
        logger.LogInformation("SendPasswordResetLinkAsync {email}: {link}", email, resetLink);
    }

    public async Task SendPasswordResetCodeAsync(MasterUser user, string email, string resetCode)
    {
        logger.LogInformation("SendPasswordResetCodeAsync {email}: {code}", email, resetCode);
    }
}
using Microsoft.AspNetCore.Identity;
using FoodSphere.Data.Models;

namespace FoodSphere.Utilities;

public class EmailService(
    ILogger<EmailService> logger
) : IEmailSender<MasterUser>
{
    readonly ILogger<EmailService> _logger = logger;

    public async Task SendConfirmationLinkAsync(MasterUser user, string email, string confirmationLink)
    {
        _logger.LogInformation("SendConfirmationLinkAsync {email}: {link}", email, confirmationLink);
    }

    public async Task SendPasswordResetLinkAsync(MasterUser user, string email, string resetLink)
    {
        _logger.LogInformation("SendPasswordResetLinkAsync {email}: {link}", email, resetLink);
    }

    public async Task SendPasswordResetCodeAsync(MasterUser user, string email, string resetCode)
    {
        _logger.LogInformation("SendPasswordResetCodeAsync {email}: {code}", email, resetCode);
    }
}
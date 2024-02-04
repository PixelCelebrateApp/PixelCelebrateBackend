using PixelCelebrateBackend.Service.Model;

namespace PixelCelebrateBackend.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailData mailData);
    }
}
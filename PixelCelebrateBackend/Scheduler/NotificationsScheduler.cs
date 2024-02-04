using MailKit;
using Microsoft.IdentityModel.Tokens;
using PixelCelebrateBackend.Dtos;
using PixelCelebrateBackend.Service;
using PixelCelebrateBackend.Service.Model;

namespace PixelCelebrateBackend.Scheduler
{
    public class NotificationsScheduler
    {
        private readonly PeriodicTimer _periodicTimer;
        private Task? _task;
        private CancellationTokenSource _cts = new();
        private readonly IEmailService _emailService;
        private readonly NotificationsService _notificationsService;
        public int _notificationValue;

        public NotificationsScheduler(TimeSpan timeSpan, IEmailService emailService, NotificationsService notificationsService)
        {
            _periodicTimer = new PeriodicTimer(timeSpan);
            _emailService = emailService;
            _notificationsService = notificationsService;
        }

        public void Start()
        {
            _cts = new();
            _notificationValue = GetNotificationValue();
            _task = ExecuteWorkAsync();
        }

        private int GetNotificationValue()
        {
            return _notificationsService.GetBirthdayNotification();
        }

        public List<GetUserDto> GetBirthdayUsers(DateOnly date)
        {
            return _notificationsService.GetBirthdayUsers(date);
        }

        public List<GetUserDto> GetOtherUsers(DateOnly date)
        {
            return _notificationsService.GetOtherUsers(date);
        }

        private async Task ExecuteWorkAsync()
        {
            try
            {
                while (await _periodicTimer.WaitForNextTickAsync(_cts.Token))
                {
                    DateOnly date = DateOnly.FromDateTime(DateTime.Now);
                    date = date.AddDays(_notificationValue);
                    Console.WriteLine("Calculated date: " + date.ToString() + "; Value: " + _notificationValue);

                    List<GetUserDto> birthdayUsers = GetBirthdayUsers(date);
                    List<GetUserDto> otherUsers = GetOtherUsers(date);

                    // Send email in bulk to a group of user (only possible with a domain):
                    /*
                    string otherUsersList = string.Empty;
                    foreach (var user in otherUsers)
                    {
                        otherUsersList += user.Email.ToString() + ";";
                    }
                    */

                    foreach (var user in birthdayUsers)
                    {
                        foreach (var otherUser in otherUsers)
                        {
                            EmailData emailData = new(otherUser.Email, "Employees", "Birthday Notification",
                                user.FirstName + " " + user.LastName + "'s (" + user.Username + ") birthday is in " +
                                _notificationValue + " days!");
                            await _emailService.SendEmailAsync(emailData);
                        }
                        foreach (var otherUser in birthdayUsers)
                        {
                            if (otherUser != user)
                            {
                                EmailData emailData = new(otherUser.Email, "Employees", "Birthday Notification",
                               user.FirstName + " " + user.LastName + "'s (" + user.Username + ") birthday is in " +
                               _notificationValue + " days!");
                                await _emailService.SendEmailAsync(emailData);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async Task StopAsync()
        {
            if (_task is null)
            {
                return;
            }
            _cts.Cancel();
            await _task;
            _cts.Dispose();
            Console.WriteLine("Task cancelled!");
        }
    }
}
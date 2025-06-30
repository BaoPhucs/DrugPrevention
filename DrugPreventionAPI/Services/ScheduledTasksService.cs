
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DrugPreventionAPI.Interfaces;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Services
{
    public class ScheduledTasksService : BackgroundService
    {
        private readonly ILogger<ScheduledTasksService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer _timer;

        public ScheduledTasksService(IServiceScopeFactory scopeFactory,
                                     ILogger<ScheduledTasksService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            // Chạy lần đầu sau 1 phút, sau đó cứ 5 phút chạy tiếp
            _timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var scheduleRepo = scope.ServiceProvider.GetRequiredService<IConsultantScheduleRepository>();
                var courseRepo = scope.ServiceProvider.GetRequiredService<ICourseRepository>();
                var apptRepo = scope.ServiceProvider.GetRequiredService<IAppointmentRequestRepository>();
                var emailSvc = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var notifRepo = scope.ServiceProvider.GetRequiredService<INotificationRepository>();

                // 0) Disable tất cả slot <= 24h tới
                var closedCount = scheduleRepo
                    .DisableSlotsExpiringWithinAsync(TimeSpan.FromHours(24))
                    .GetAwaiter().GetResult();
                if (closedCount > 0)
                    _logger.LogInformation($"Disabled {closedCount} slots expiring within 24h at {DateTime.UtcNow}.");


                // 1) Publish all due courses
                var publishedCount = courseRepo.PublishAllDueAsync().GetAwaiter().GetResult();
                if (publishedCount > 0)
                    _logger.LogInformation($"Published {publishedCount} course(s) at {DateTime.UtcNow}.");

                // 2) Send appointment reminders 24h trước
                var toRemind = apptRepo.GetConfirmedAppointmentsForReminderAsync(TimeSpan.FromDays(1)) //FromDays(1)
                                       .GetAwaiter().GetResult();
                foreach (var appt in toRemind)
                {
                    // gửi email
                    var html = $@"
                    <p>Xin chào {appt.Member.Name},</p>
                    <p>Bạn có lịch tư vấn ngày {appt.Schedule:dd/MM/yyyy HH:mm} (1 ngày nữa).</p>
                    <p>Vui lòng chuẩn bị và có mặt đúng giờ.</p>";
                    emailSvc.SendEmailAsync(
                        appt.Member.Email,
                        "Nhắc lịch tư vấn ngày mai",
                        html
                    ).GetAwaiter().GetResult();

                    // lưu notification
                    var note = new Notification
                    {
                        UserId = appt.MemberId,
                        Type = "AppointmentReminder",
                        Title = "Nhắc lịch tư vấn",
                        Message = $"Bạn có lịch tư vấn ngày {appt.Schedule:dd/MM/yyyy HH:mm}.",
                        SendDate = DateTime.UtcNow
                    };
                    notifRepo.CreateAsync(note).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ScheduledTasksService");
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DrugPreventionAPI.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DrugPreventionAPI.Services
{
    public class CommunicationActivityCancellationService : BackgroundService
    {
        private readonly ILogger<CommunicationActivityCancellationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5); // Kiểm tra mỗi 5 phút

        public CommunicationActivityCancellationService(
            IServiceScopeFactory scopeFactory,
            ILogger<CommunicationActivityCancellationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CommunicationActivityCancellationService khởi động.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var commActivityRepo = scope.ServiceProvider.GetRequiredService<ICommunicationActivityRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    var (cancelledCount, details) = await commActivityRepo.CheckAndCancelAllUnderCapacityAsync();
                    if (cancelledCount > 0)
                    {
                        _logger.LogInformation($"Cancelled {cancelledCount} activity(ies) at {DateTime.UtcNow}.");
                        foreach (var (activityId, (title, emails)) in details)
                        {
                            foreach (var email in emails.Where(e => !string.IsNullOrEmpty(e)))
                            {
                                var html = $@"
                                    <p>Xin chào,</p>
                                    <p>Sự kiện <strong>{title}</strong> (ID: {activityId}) đã bị hủy do số lượng tham gia dưới 60% sức chứa.</p>
                                    <p>Chúng tôi xin lỗi vì sự bất tiện này. Vui lòng liên hệ với quản lý để được hỗ trợ.</p>
                                    <p>Trân trọng,</p>
                                    <p>Đội ngũ Drug Prevention</p>";
                                await emailService.SendEmailAsync(
                                    email,
                                    "Thông báo: Sự kiện bị hủy",
                                    html);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy CommunicationActivityCancellationService");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("CommunicationActivityCancellationService dừng.");
        }
    }
}
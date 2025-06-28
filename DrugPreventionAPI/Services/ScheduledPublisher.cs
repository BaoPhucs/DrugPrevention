using DrugPreventionAPI.Interfaces;

namespace DrugPreventionAPI.Services
{
    public class ScheduledPublisher : BackgroundService
    {
        private readonly ILogger<ScheduledPublisher> _logger;
        private readonly IServiceProvider _sp;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        // (Đặt 1 phút để test, sau đó có thể chuyển thành TimeSpan.FromHours(1) hoặc theo nhu cầu)
        public ScheduledPublisher(
            IServiceProvider sp,
            ILogger<ScheduledPublisher> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ScheduledPublisher khởi động.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Tạo scope để lấy repository từ DI
                    using var scope = _sp.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ICourseRepository>();

                    // Gọi hàm publish tất cả khóa học đến giờ
                    var count = await repo.PublishAllDueAsync();
                    if (count > 0)
                        _logger.LogInformation("Published {Count} course(s) at {Time}.", count, DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi chạy ScheduledPublisher");
                }

                // Đợi 1 khoảng trước khi lặp lại
                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("ScheduledPublisher dừng.");
        }
    }
}

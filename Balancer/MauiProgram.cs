using Balancer.Components.Services;
using Microsoft.Extensions.Logging;
using MudBlazor.Services;

namespace Balancer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<DonorService>();
            builder.Services.AddScoped<DonationEntryService>();
            builder.Services.AddScoped<BalancerService>();
            builder.Services.AddSingleton<DialogService>();
            builder.Services.AddScoped<CashService>();
            builder.Services.AddScoped<CheckService>();
            builder.Services.AddMudServices();

            // Register Database Services
            builder.Services.AddDatabaseServices();

            // Ensure database is created at runtime
            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var dbService = scope.ServiceProvider.GetRequiredService<DBService>();
                dbService.InitializeDatabase();
            }

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
            builder.Logging.SetMinimumLevel(LogLevel.Trace);
#endif

            return builder.Build();
        }
    }
}
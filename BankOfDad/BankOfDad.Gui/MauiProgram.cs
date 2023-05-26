using BankOfDad.Client;
using BankOfDad.Gui.Views;
using Microsoft.Extensions.Logging;

namespace BankOfDad.Gui
{
    public static class MauiProgram
    {
        //private const string Url = "https://garuda:7261";
        private const string Url = "https://bankofdad.vkapadia.com";

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(x => new BankOfDadClient(Url));
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<TransactionPage>();

            return builder.Build();
        }
    }
}
using Microsoft.Extensions.Logging;

namespace NEKODORO
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Uygulama her zaman İngilizce başlasın
            var culture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Ebolditalic.ttf", "Ebolditalic");
                    fonts.AddFont("Ebolditvart.ttf", "Ebolditvart");
                    fonts.AddFont("Ebold.ttf", "Ebold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

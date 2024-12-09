using Plugin.Maui.Audio;

namespace PoirotCollectionApp
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

            // Register the audio services
            builder.Services.AddSingleton<IAudioManager, AudioManager>();
            builder.Services.AddSingleton<AudioController>();

            return builder.Build();
        }
    }
}

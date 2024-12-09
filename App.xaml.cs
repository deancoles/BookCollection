using Plugin.Maui.Audio;

namespace PoirotCollectionApp
{
    public partial class App : Application
    {
        public App(AudioController audioController)
        {
            InitializeComponent();

            // Wrap MainPage in a NavigationPage
            MainPage = new NavigationPage(new MainPage(audioController));
        }
    }
}

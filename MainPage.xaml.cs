using Plugin.Maui.Audio; // Import the plugin for using audio features
using PoirotCollectionApp.DataAccess; // Correct namespace for DatabaseHelper

// Defines namespace for the application
namespace PoirotCollectionApp
{
    // Defines MainPage class, inheriting from ContentPage
    public partial class MainPage : ContentPage
    {
        private readonly AudioController _audioController; // The audio controller for music playback

        // Constructor for the MainPage class
        public MainPage(AudioController audioController)
        {
            InitializeComponent(); // Initialize the XAML components
            _audioController = audioController;

            InitializeMusic(); // Start music initialization
        }

        // Method to initialize and play music
        private async void InitializeMusic()
        {
            try
            {
                await _audioController.LoadTrackAsync("MainTheme", "background_music.mp3"); // Load music track with a key and file name
                _audioController.PlayTrack("MainTheme"); // Play the loaded music track
            }
            catch (Exception ex) // Catch any errors 
            {
                await DisplayAlert("Error", $"Failed to play music: {ex.Message}", "OK"); // If music fails to load or play
            }
        }

        // Browse Button
        private async void OnBrowseClicked(object sender, EventArgs e)
        {
            try
            {
                var dbHelper = new DatabaseHelper();
                var users = await dbHelper.GetUsersAsync(); // Fetch users from the database
                var userNames = users.Select(u => u.Username).ToArray(); // Extract usernames

                string selectedUser = await DisplayActionSheet("Select User", "Cancel", null, userNames);

                if (selectedUser == "Cancel" || string.IsNullOrEmpty(selectedUser))
                {
                    return; // User canceled the action
                }

                var selected = users.FirstOrDefault(u => u.Username == selectedUser);
                if (selected == null)
                {
                    await DisplayAlert("Error", "User not found. Please try again.", "OK");
                    return; // Exit if no user is found
                }

                int userId = selected.UserID;
                string userName = selected.Username;

                await Navigation.PushAsync(new BrowsePage(userId, userName)); // Navigate to BrowsePage
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
            }
        }

        // Event handler for the Database button
        private async void OnDatabaseClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Database", "Here you would view and manage your database.", "OK");
        }

        // Event handler for the About button
        private async void OnAboutClicked(object sender, EventArgs e)
        {
            await DisplayAlert("About ", "Version 1.4", "OK");
        }

        // Event handler for the Settings button
        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Settings", "Here you would adjust your application settings.", "OK");
        }


        // Exit button behaviour
        private async void OnExitButtonClicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Exit", "Do you want to close the application?", "Yes", "No");

            // Close the application based on the platform
            if (answer)
            {
#if ANDROID
            // Close the app on Android by killing the process
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
            // Ensure Application.Current is not null before calling Quit()
            if (Application.Current != null)
            {
                Application.Current.Quit(); // Close the app on Windows
            }
            else
            {
                await DisplayAlert("Error", "Application context is not available.", "OK");
            }
#endif
            }
        }
    }
}

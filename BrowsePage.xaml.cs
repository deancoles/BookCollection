using PoirotCollectionApp.DataAccess; // For DatabaseHelper
using PoirotCollectionApp.Models;
using System; // For general system utilities
using System.ComponentModel; // For INotifyPropertyChanged
using System.Runtime.CompilerServices; // For CallerMemberName attribute
using Microsoft.Maui.Controls; // For UI elements

namespace PoirotCollectionApp
{
    public partial class BrowsePage : ContentPage, INotifyPropertyChanged
    {
        private string _userName = string.Empty; // Active user's name
        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _userId; // Active user's ID

        private string _currentFilter = "All"; // Currently active filter
        public string CurrentFilter
        {
            get => _currentFilter;
            set
            {
                if (_currentFilter != value)
                {
                    _currentFilter = value;
                    OnPropertyChanged();
                    UpdateButtonStyles(); // Update button styles when filter changes
                }
            }
        }

        public BrowsePage(int userId, string userName)
        {
            InitializeComponent();

            _userId = userId;
            UserName = userName;

            CurrentFilter = "Owned"; // Default filter
            BindingContext = this; // Bind data to UI

            LoadBooks("Owned");
        }

        private async void LoadBooks(string filter)
        {
            try
            {
                var dbHelper = new DatabaseHelper();
                var books = await dbHelper.GetPoirotCollectionAsync(_userId, filter);

                // Set the full list of books as the source
                BooksListView.ItemsSource = books;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load books: {ex.Message}", "OK");
            }
        }

        private void FilterOwned(object sender, EventArgs e)
        {
            CurrentFilter = "Owned";
            LoadBooks("Owned");
        }

        private void FilterWishlist(object sender, EventArgs e)
        {
            CurrentFilter = "Wishlist";
            LoadBooks("Wishlist");
        }

        private void FilterAll(object sender, EventArgs e)
        {
            CurrentFilter = "All";
            LoadBooks("All");
        }

        private async void OnChangeUserClicked(object sender, EventArgs e)
        {
            try
            {
                var dbHelper = new DatabaseHelper();
                var users = await dbHelper.GetUsersAsync(); // Fetch users from the database
                var userNames = users.Select(u => u.Username).ToArray(); // Get user names for display

                string selectedUser = await DisplayActionSheet("Select User", "Cancel", null, userNames);

                if (selectedUser != "Cancel")
                {
                    var selected = users.First(u => u.Username == selectedUser);
                    _userId = selected.UserID;
                    UserName = selected.Username;
                    LoadBooks("All"); // Reload books for the selected user
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load users: {ex.Message}", "OK");
            }
        }



        private void UpdateButtonStyles()
        {
            // Reset all button styles
            OwnedButton.BackgroundColor = Colors.LightGray;
            WishlistButton.BackgroundColor = Colors.LightGray;
            AllButton.BackgroundColor = Colors.LightGray;

            // Highlight the active filter button
            switch (CurrentFilter)
            {
                case "Owned":
                    OwnedButton.BackgroundColor = Colors.Purple;
                    break;
                case "Wishlist":
                    WishlistButton.BackgroundColor = Colors.Purple;
                    break;
                case "All":
                    AllButton.BackgroundColor = Colors.Purple;
                    break;
            }
        }

        // Event triggered when the text in the search bar changes
        private async void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Get the current search text from the search bar
                string searchText = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

                var dbHelper = new DatabaseHelper();

                // If search text is empty, load all books for the current filter
                if (string.IsNullOrEmpty(searchText))
                {
                    BooksListView.ItemsSource = await dbHelper.GetPoirotCollectionAsync(_userId, CurrentFilter);
                }
                else
                {
                    // Perform a filtered search based on the search text
                    var filteredBooks = await dbHelper.GetPoirotCollectionAsync(_userId, CurrentFilter);

                    // Apply a search filter to display matching titles
                    BooksListView.ItemsSource = filteredBooks.Where(book =>
                        book.Title.ToLower().Contains(searchText));
                }
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                await DisplayAlert("Error", $"An error occurred while searching: {ex.Message}", "OK");
            }
        }

        // Back button
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Navigate back to the previous page
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            base.OnPropertyChanged(propertyName);
        }
    }
}

using MySqlConnector; // For connecting to MySQL
using PoirotCollectionApp.Models; // To use the book models
using System.Collections.Generic; // For lists
using System.Data; // For working with databases
using System.Threading.Tasks; // For async methods

namespace PoirotCollectionApp.DataAccess
{
    public class DatabaseHelper
    {
        private readonly string _connectionString; // Stores the database connection details

        // Sets up the connection string
        public DatabaseHelper()
        {
            _connectionString = "Server=sql8.freemysqlhosting.net;Database=sql8746381;User=sql8746381;Password=SUVJ3DqUNZ;";
        }

        // Creates a connection to the database
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // Checks if the database connection works
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    await connection.OpenAsync(); // Try to connect
                    return true; // Connection is good
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}"); // Show error
                return false; // Connection failed
            }
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();

            try
            {
                using (var connection = GetConnection()) // Get MySQL connection
                {
                    await connection.OpenAsync();

                    string query = "SELECT UserID, UserName FROM users";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(new User
                                {
                                    UserID = reader.IsDBNull("UserID") ? 0 : reader.GetInt32("UserID"), // Default to 0 if null
                                    Username = reader.IsDBNull("UserName") ? "Unknown" : reader.GetString("UserName") // Default to "Unknown" if null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching users: {ex.Message}");
            }

            return users;
        }

        // Gets a list of books for a user based on the filter
        public async Task<List<PoirotCollection>> GetPoirotCollectionAsync(int userId, string filter)
        {
            var books = new List<PoirotCollection>(); // Holds the books

            using (var connection = GetConnection())
            {
                await connection.OpenAsync(); // Connect to the database
                string query = "SELECT * FROM poirotcollection WHERE UserID = @UserID";  // Base query to get books for the user

                // Add filter for "Owned" books
                if (filter == "Owned")
                {
                    query += " AND Owned = 1"; 
                }
                
                // Add filter for "Wishlist" books
                else if (filter == "Wishlist")
                {
                    query += " AND Wishlist = 1"; 
                }

                // Set up the query
                using (var command = new MySqlCommand(query, connection)) 
                {
                    command.Parameters.AddWithValue("@UserID", userId); // Add user ID to the query

                    // Run the query
                    using (var reader = await command.ExecuteReaderAsync()) 
                    {
                        while (await reader.ReadAsync())
                        {
                            // Add each book to the list
                            books.Add(new PoirotCollection
                            {
                                UserID = reader.GetInt32("UserID"), 
                                BookID = reader.GetInt32("BookID"), 
                                Title = reader.GetString("Title"), 
                                ReleaseDate = reader.IsDBNull("ReleaseDate") ? (DateTime?)null : reader.GetDateTime("ReleaseDate"), 
                                Notes = reader.IsDBNull("Notes") ? string.Empty : reader.GetString("Notes"), 
                                CoverImagePath = reader.IsDBNull("CoverImagePath") ? string.Empty : reader.GetString("CoverImagePath"), // Book cover
                                Owned = reader.GetBoolean("Owned"), 
                                Wishlist = reader.GetBoolean("Wishlist") 
                            });
                        }
                    }
                }
            }

            return books; // Return the list of books
        }
    }
}

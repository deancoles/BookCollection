using System;

namespace PoirotCollectionApp.Models
{
    public class PoirotCollection
    {
        public int UserID { get; set; } // The user who owns the book
        public int BookID { get; set; } // Unique ID of the book
        public string Title { get; set; } = string.Empty; // Title of the book
        public DateTime? ReleaseDate { get; set; } // Release date 
        public string Notes { get; set; } = string.Empty; // Blurb for the book
        public string CoverImagePath { get; set; } = string.Empty; // URL to book's cover image
        public bool Owned { get; set; } // Indicates if the book is owned by the user
        public bool Wishlist { get; set; } // Indicates if the book is on user's wishlist
    }

    public class User
    {
        public int UserID { get; set; } // Unique ID for each user
        public string Username { get; set; } = string.Empty; // User's username
        public string PasswordHash { get; set; } = string.Empty; // User's password
    }

}

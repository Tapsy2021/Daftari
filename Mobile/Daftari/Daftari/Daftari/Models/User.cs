using Daftari.ViewModels;
using SQLite;

namespace Daftari.Models
{
    public class User : ViewModelBase
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        //public DateTime? LastLoggedIn { get; set; }
        public string DeviceId { get; set; }
    }
}

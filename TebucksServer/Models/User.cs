
using System.Text.Json.Serialization;

namespace TEBucksServer.Models
{
    public class User
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        
        [JsonPropertyName("id")]
        public int UserId { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public string Salt { get; set; }
        public string Email { get; set; }
    }

    /// <summary>
    /// Model to return upon successful login
    /// </summary>
    public class ReturnUser
    {
        public User user { get; set; }
        //public string Role { get; set; }
        public string Token { get; set; }
    }

    /// <summary>
    /// Model to accept login parameters
    /// </summary>
    public class LoginUser
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

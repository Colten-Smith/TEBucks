using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TEBucksServer.Models;
using TEBucksServer.Security;
using TEBucksServer.Security.Models;

namespace TEBucksServer.DAO
{
    public class UserSqlDao : IUserDao
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public UserSqlDao(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, firstname, lastname, username, password_hash, salt FROM tebucks_user WHERE username = @username", conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        public List<User> GetUsers(string usernameToIgnore = "")
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, firstname, lastname, username, password_hash, salt FROM tebucks_user", conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        User u = GetUserFromReader(reader);
                        if(u.Username != usernameToIgnore)
                        {
                            returnUsers.Add(u);
                        }
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(RegisterUser registerUser)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(registerUser.Password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("INSERT INTO tebucks_user (firstname, lastname, username, password_hash, salt) " +
                        "OUTPUT INSERTED.user_id VALUES (@firstname, @lastname, @username, @password_hash, @salt)", conn);
                    cmd.Parameters.AddWithValue("@username", registerUser.Username);
                    cmd.Parameters.AddWithValue("@firstname", registerUser.Firstname);
                    cmd.Parameters.AddWithValue("@lastname", registerUser.Lastname);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    int newUsersID = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return GetUser(registerUser.Username);
        }
        public User GetUserById(int id)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT user_id, firstname, lastname, username, password_hash, salt FROM tebucks_user WHERE user_id = @id", conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUser;
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                Firstname = reader["firstname"] != null ? Convert.ToString(reader["firstname"]) : "",
                Lastname = reader["lastname"] != null ? Convert.ToString(reader["lastname"]) : "",
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"]),
            };

            return u;
        }
    }
}

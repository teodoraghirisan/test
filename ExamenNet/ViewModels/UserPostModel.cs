using ExamenNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExamenNet.ViewModels
{
    public class UserPostModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DataRegistered { get; set; }
        public string UserRole { get; set; }



        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   

            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static User ToUser(UserPostModel userModel)
        {
            UserRole rol = ExamenNet.Models.UserRole.Regular;

            if (userModel.UserRole == "Moderator")
            {
                rol = ExamenNet.Models.UserRole.Moderator;
            }
            else if (userModel.UserRole == "Admin")
            {
                rol = ExamenNet.Models.UserRole.Admin;
            }

            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Username = userModel.Username,
                Email = userModel.Email,
                Password = ComputeSha256Hash(userModel.Password),
                UserRole = rol
            };
        }
    }
}

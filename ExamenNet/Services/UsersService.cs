using ExamenNet.Models;
using ExamenNet.Validators;
using ExamenNet.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ExamenNet.Services
{
    public interface IUsersService
    {
        LoginGetModel Authenticate(string username, string password);
        ErrorsCollection Register(RegisterPostModel registerinfo);
        User GetCurrentUser(HttpContext httpContext);

        IEnumerable<UserGetModel> GetAll();
        UserGetModel GetById(int id);
        ErrorsCollection Create(RegisterPostModel userModel);
        UserGetModel Upsert(int id, UserPostModel userPostModel);
        UserGetModel Delete(int id);
    }

    public class UsersService : IUsersService
    {
        private ExamDbContext context;
        private readonly AppSettings appSettings;
        private IRegisterValidator registerValidator;

        public UsersService(ExamDbContext context, IRegisterValidator registerValidator, IOptions<AppSettings> appSettings)
        {
            this.context = context;
            this.appSettings = appSettings.Value;
            this.registerValidator = registerValidator;

        }

        public LoginGetModel Authenticate(string username, string password)
        {
            var user = context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Username == username && u.Password == ComputeSha256Hash(password));


            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.UserRole.ToString()),                               //DataRegistered si rolul imi vin ca string
                    new Claim(ClaimTypes.UserData, user.DataRegistered.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var result = new LoginGetModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                Token = tokenHandler.WriteToken(token),

            };

            return result;
        }

        private string ComputeSha256Hash(string rawData)
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


        public ErrorsCollection Register(RegisterPostModel registerinfo)
        {
            var errors = registerValidator.Validate(registerinfo, context);
            if (errors != null)
            {
                return errors;
            }

            User toAdd = new User
            {
                Email = registerinfo.Email,
                LastName = registerinfo.LastName,
                FirstName = registerinfo.FirstName,
                Password = ComputeSha256Hash(registerinfo.Password),
                Username = registerinfo.Username,
                DataRegistered = DateTime.Now,
                UserRole = UserRole.Regular
            };
            //Regular ca default
            //var regularRole = context
            //    .Users
            //    .FirstOrDefault(ur => ur.Username == UserRoles.Regular);

            context.Users.Add(toAdd);
            context.SaveChanges();
            return null;
        }

        public User GetCurrentUser(HttpContext httpContext)
        {
            string username = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
            //string accountType = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod).Value;
            //return _context.Users.FirstOrDefault(u => u.Username == username && u.AccountType.ToString() == accountType);

            //User user = context
            //    .Users
            //    //.Include(u => u.User_UserRoles)
            //    .FirstOrDefault(u => u.Username == username);
            //return user;
            return context.Users.FirstOrDefault(u => u.Username == username);
        }

        //IMPLEMENTARE CRUD PENTRU USER

        public IEnumerable<UserGetModel> GetAll()
        {
            return context.Users.Select(user => UserGetModel.FromUser(user));
        }

        public UserGetModel GetById(int id)
        {
            User user = context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id);

            return UserGetModel.FromUser(user);
        }

        public ErrorsCollection Create(RegisterPostModel registerPostModel)
        {
            var errors = registerValidator.Validate(registerPostModel, context);
            if (errors != null)
            {
                return errors;
            }
            User toAdd = RegisterPostModel.ToUser(registerPostModel);
            //se atribuie rolul de Regular ca default
            var regularRole = context
                //.UserRoles
                .Users
                .FirstOrDefault(ur => ur.UserRole == UserRole.Regular);

            context.Users.Add(toAdd);

            //context.User_UserRoles.Add(new User_UserRole
            //{
            //    User = toAdd,
            //    UserRole = regularRole,
            //    StartTime = DateTime.Now,
            //    EndTime = null
            //});

            context.SaveChanges();
            return null;
        }


        public UserGetModel Upsert(int id, UserPostModel userPostModel)
        {
            var existing = context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
            if (existing == null)
            {
                User toAdd = UserPostModel.ToUser(userPostModel);
                context.Users.Add(toAdd);
                context.SaveChanges();
                return UserGetModel.FromUser(toAdd);
            }

            User toUpdate = UserPostModel.ToUser(userPostModel);
            toUpdate.Id = id;
            context.Users.Update(toUpdate);
            context.SaveChanges();
            return UserGetModel.FromUser(toUpdate);
        }


        public UserGetModel Delete(int id)
        {
            var existing = context.Users
            .FirstOrDefault(user => user.Id == id);
            if (existing == null)
            {
                return null;
            }
            context.Users.Remove(existing);
            context.SaveChanges();

            return UserGetModel.FromUser(existing);
        }

    }
}

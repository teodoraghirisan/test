using ExamenNet.Models;
using ExamenNet.Services;
using ExamenNet.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using System.Linq;

namespace Tests
{
    public class Tests
    {
        private IOptions<AppSettings> config;

        [SetUp]
        public void Setup()
        {
            config = Options.Create(new AppSettings
            {
                //trebuie sa fie suficient de lung sirul de caractere.
                Secret = "dsadhjcghduihdfhdifd8ih"
            });

        }

        [Test]
        public void ValidRegisterShouldCreateNewUser()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
                         .UseInMemoryDatabase(databaseName: nameof(ValidRegisterShouldCreateNewUser))// "ValidRegisterShouldCreateANewUser")
                         .Options;

            using (var context = new ExamDbContext(options))
            {
                var validator = new RegisterValidator();
                var usersService = new UsersService(context, validator, config);
                var added = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "lastName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111111"
                };

                var result = usersService.Register(added);

                Assert.IsNull(result);
                Assert.AreEqual(added.Username, context.Users.FirstOrDefault(u => u.Id == 1).Username);
                // Assert.AreEqual(1, context.Users.UserRole.FirstOrDefault(uur => uur.Id == 1).UserId);
            }
        }

        [Test]
        public void InvalidRegisterShouldReturnErrorsCollection()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
                         .UseInMemoryDatabase(databaseName: nameof(InvalidRegisterShouldReturnErrorsCollection))
                         .Options;

            using (var context = new ExamDbContext(options))
            {
                var validator = new RegisterValidator();
                var usersService = new UsersService(context, validator, config);
                var added = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "lastName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111"    //invalid password should invalidate register
                };

                var result = usersService.Register(added);

                Assert.IsNotNull(result);
                Assert.AreEqual(1, result.ErrorMessages.Count());
            }
        }

        [Test]
        public void AuthenticateShouldLogTheRegisteredUser()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(AuthenticateShouldLogTheRegisteredUser))
              .Options;

            using (var context = new ExamDbContext(options))
            {
                var validator = new RegisterValidator();
               // var validatorUser = new UserRoleValidator();
                //var userUserRoleService = new UserUserRolesService(validatorUser, context);
                var usersService = new UsersService(context, validator, config);

               
                var added = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "lastName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111111"
                };
                var result = usersService.Register(added);

                var authenticated = new ExamenNet.ViewModels.LoginPostModel
                {
                    Username = "test_userName1",
                    Password = "111111"
                };
                //valid authentification
                var authresult = usersService.Authenticate(added.Username, added.Password);

                Assert.IsNotNull(authresult);
                Assert.AreEqual(1, authresult.Id);
                Assert.AreEqual(authenticated.Username, authresult.Username);

                //invalid user authentification
                var authresult1 = usersService.Authenticate("unknown", "abcdefg");
                Assert.IsNull(authresult1);
            }
        }

        [Test]
        public void GetAllShouldReturnAllRegisteredUsers()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
              .UseInMemoryDatabase(databaseName: nameof(GetAllShouldReturnAllRegisteredUsers))
              .Options;

            using (var context = new ExamDbContext(options))
            {
                var validator = new RegisterValidator();
                var usersService = new UsersService(context, validator, config);
                var added1 = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "firstName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111111"
                };
                var added2 = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "secondName2",
                    LastName = "secondName2",
                    Username = "test_userName2",
                    Email = "second@yahoo.com",
                    Password = "111111"
                };
                usersService.Register(added1);
                usersService.Register(added2);

                int numberOfElements = usersService.GetAll().Count();

                Assert.NotZero(numberOfElements);
                Assert.AreEqual(2, numberOfElements);

            }
        }

        [Test]
        public void GetByIdShouldReturnAnValidUser()
        {
            var options = new DbContextOptionsBuilder<ExamDbContext>()
         .UseInMemoryDatabase(databaseName: nameof(GetByIdShouldReturnAnValidUser))
         .Options;

            using (var context = new ExamDbContext(options))
            {
                var validator = new RegisterValidator();
                var usersService = new UsersService(context, validator, config);
                var added1 = new ExamenNet.ViewModels.RegisterPostModel
                {
                    FirstName = "firstName1",
                    LastName = "firstName1",
                    Username = "test_userName1",
                    Email = "first@yahoo.com",
                    Password = "111111"
                };

                usersService.Register(added1);
                var userById = usersService.GetById(1);

                Assert.NotNull(userById);
                Assert.AreEqual("firstName1", userById.FirstName);

            }
        }

    }

}

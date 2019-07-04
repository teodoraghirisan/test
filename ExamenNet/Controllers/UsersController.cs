using ExamenNet.Models;
using ExamenNet.Services;
using ExamenNet.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExamenNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUsersService userService;


        public UsersController(IUsersService userService)
        {
            this.userService = userService;

        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]LoginPostModel login)
        {
            var user = userService.Authenticate(login.Username, login.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterPostModel registerModel)
        {
            var errors = userService.Register(registerModel);//, out User user);
            if (errors != null)
            {
                return BadRequest(errors);
            }
            return Ok();
        }
        [HttpGet]
        //[Authorize(Roles = "Admin,UserManager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IEnumerable<UserGetModel> Get()
        {
            return userService.GetAll();
        }

        /// <summary>
        /// Find an user by the given id.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Get /users
        ///     {  
        ///        id: 3,
        ///        firstName = "Bodea",
        ///        lastName = "Raluca",
        ///        userName = "raluca",
        ///        email = "raluca@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <param name="id">The id given as parameter</param>
        /// <returns>The user with the given id</returns>
        // GET: api/Users/5
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult Get(int id)
        {
            var found = userService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }
            return Ok(found);
        }

        /// <summary>
        /// Add a new User
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     Post /users
        ///     {
        ///        firstName = "Bodea",
        ///        lastName = "Raluca",
        ///        userName = "raluca",
        ///        email = "raluca@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <param name="postModel">The input user to be added</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPost]
        public IActionResult Post([FromBody] RegisterPostModel postModel)
        {
            User currentUserLogin = userService.GetCurrentUser(HttpContext);
            string roleNameLoged = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            if (roleNameLoged.Equals("Moderator"))
            {
                var anulUserRegistered = currentUserLogin.DataRegistered;        //data inregistrarii
                var curentMonth = DateTime.Now;                                 //data curenta
                var nrLuni = curentMonth.Subtract(anulUserRegistered).Days / (365.25 / 12);   //diferenta in luni dintre datele transmise

                if (nrLuni >= 6)
                {
                    if (UserRole.Admin.Equals("Admin"))
                    {
                        return Forbid("You don`t have the right role for this action!");
                    }
                    if (UserRole.Moderator.Equals("Moderator") || UserRole.Regular.Equals("Regular"))
                    {
                        return Forbid("You don`t have the right role for this action!");
                    }
                }
                else
                {
                    return Forbid("Your Moderator is not more than 6 month");
                }
            }
            userService.Create(postModel);
            return Ok();
        }


        /// <summary>
        /// Modify an user if exists in dbSet , or add if not exist
        /// </summary>
        /// <param name="id">id-ul user to update</param>
        /// <param name="userPostModel"></param>
        /// Sample request:
        ///     <remarks>
        ///     Put /users/id
        ///     {
        ///        firstName = "Bodea",
        ///        lastName = "Raluca",
        ///        userName = "raluca",
        ///        email = "raluca@yahoo.com",
        ///        userRole = "regular"
        ///     }
        /// </remarks>
        /// <returns>Status 200 daca a fost modificat</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserPostModel userPostModel)
        {
            User curentUserLogIn = userService.GetCurrentUser(HttpContext);
            string roleNameLoged = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            var currentUserRoleName = userService.GetById(id).UserRole;

            if (roleNameLoged.Equals("Moderator"))
            {

                var anulUserRegistered = curentUserLogIn.DataRegistered;        //data inregistrarii
                var curentMonth = DateTime.Now;                                 //data curenta
                var nrLuni = curentMonth.Subtract(anulUserRegistered).Days / (365.25 / 12);   //diferenta in luni dintre datele transmise

                if (nrLuni < 6)
                {
                    return Forbid("Your Moderator is not more than 6 month");
                }

            }

            var result = userService.Upsert(id, userPostModel);
            return Ok(result);
        }


        /// <summary>
        /// Delete one user
        /// </summary>
        /// <param name="id">User id to delete</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string roleNameLoged = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;

            if (roleNameLoged.Equals("Moderator"))
            {
                UserGetModel userToDelete = userService.GetById(id);

                if (UserRole.Admin.Equals("Admin"))
                {
                    return Forbid("You don`t have the right role for this action!");
                }
            }
            var result = userService.Delete(id);
            if (result == null)
            {
                return NotFound("User with the given id is not found !");
            }
            return Ok(result);
        }


    }
}
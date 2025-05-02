using ExpenseManagment.Data;
using ExpenseManagment.Data.DataBaseEntities;
using ExpenseManagment.Filters;
using ExpenseManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagment.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ManageUserController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        public ManageUserController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            db = _db;
            _userManager = userManager;
        }

        [HttpGet("GetUsers")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetUsersAsync()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound("Unable to load current user.");
            }

            var users = await db.Users
                .Select(u => new UserViewModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Address = u.Address,
                    City = u.City,
                    ZipCode = u.ZipCode,
                    Email = u.Email,
                    Password = u.Password,
                    IsCompleted = !string.IsNullOrEmpty(u.Address) &&
                                  !string.IsNullOrEmpty(u.Name) &&
                                  !string.IsNullOrEmpty(u.City) &&
                                  !string.IsNullOrEmpty(u.ZipCode) &&
                                  u.Id == currentUser.Id
                })
                .ToListAsync();

            foreach (var user in users)
            {
                var userEntity = await _userManager.FindByIdAsync(user.Id);
                if (userEntity != null)
                {
                    userEntity.IsCompleted = user.IsCompleted;
                    await _userManager.UpdateAsync(userEntity);
                }
            }

            return Ok(users);
        }


        [AjaxExceptionFilter]
        [HttpPost("SaveUserData")]
        public async Task<IActionResult> SaveUserData([FromBody] UserViewModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid user data.");
            }
            var user = new ApplicationUser
            {
                UserName = model.Email.ToUpper(),
                Email = model.Email,
                EmailConfirmed = true, 
                Address = model.Address,
                Name = model.Name,
                City = model.City,
                ZipCode = model.ZipCode
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await db.SaveChangesAsync();
                return Ok(user);
            }

            var errorDescription = result.Errors.FirstOrDefault()?.Description ?? "An unknown error occurred.";
            return StatusCode(500, errorDescription);
        }



        [HttpGet("EditUserData/{id}")]
        public async Task<IActionResult> EditUserData(string id)
        {
            var user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userViewModel = new UserViewModel
            {
                Id = user.Id,
                Address = user.Address,
                Name = user.Name,
                City = user.City,
                Password = user.Password,
                ZipCode = user.ZipCode,
                Email = user.Email
            };

            return Ok(userViewModel);
        }

        [AjaxExceptionFilter]
        [HttpPut("EditUser")]
        public async Task<IActionResult> EditUser([FromBody] UserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            user.UserName = model.Email.ToUpper();
            user.Email = model.Email;
            user.Address = model.Address;
            user.Name = model.Name;
            user.City = model.City;
            user.Password = model.Password;
            user.ZipCode = model.ZipCode;
            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            return Ok(user);
        }

        [AjaxExceptionFilter]
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var result = await _userManager.DeleteAsync(user);
            return Ok(result);
        }

    }

}

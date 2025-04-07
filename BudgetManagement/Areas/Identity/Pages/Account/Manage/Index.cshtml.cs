using ExpenseManagment.Data.DataBaseEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ExpenseManagment.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        public string Username { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Password { get; set; }
        public string ProfilePicturePath { get; set; }
        public string ZipCode { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone Number")]
            [Required(ErrorMessage = "Required*")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Required*")]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Required*")]
            [Display(Name = "Name")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Required*")]
            [Display(Name = "City")]
            public string City { get; set; }
            [Required(ErrorMessage = "Required*")]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Required*")]
            [Display(Name = "Zip Code")]
            public string ZipCode { get; set; }

            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePicture { get; set; }
        }


        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var address = user.Address;
            var name = user.Name;
            var password = user.Password;
            var city = user.City;
            var zipCode = user.ZipCode;
            var profilePicturePath = user.ProfilePicturePath;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Address = address,
                Name = name,
                Password = password,
                City = city,
                ZipCode = zipCode
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            ProfilePicturePath = user.ProfilePicturePath;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (Input.ProfilePicture != null)
            {
                var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var fileName = $"{Guid.NewGuid().ToString()}-{Input.ProfilePicture.FileName}";
                var filePath = Path.Combine(uploadsDirectory, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePicture.CopyToAsync(stream);
                }

                // Remove the old profile picture if exists
                if (!string.IsNullOrEmpty(user.ProfilePicturePath))
                {
                    var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.ProfilePicturePath = $"/uploads/{fileName}";
            }

            user.Address = Input.Address;
            user.Name = Input.Name;
            user.Password = Input.Password;
            user.City = Input.City;
            user.ZipCode = Input.ZipCode;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }
}
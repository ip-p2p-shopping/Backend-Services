using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BackendService.Data;
using System.Text.RegularExpressions;
using System.Security.Claims;



namespace BackendService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ShoppingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountController(ShoppingDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (model == null)
                return BadRequest("Invalid request body");
            else if (model.OldPassword == null)
                return BadRequest("Old password is required");
            else if (model.NewPassword == null)
                return BadRequest("New password is required");

            // Check if OldPassword is correct with the one in the database
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (!await _userManager.CheckPasswordAsync(user, model.OldPassword))
                return BadRequest("Old password is incorrect");

            // Check if NewPassword meets the validation criteria
            if (model.NewPassword.Length < 6 ||
                !model.NewPassword.Any(char.IsLower) ||
                !model.NewPassword.Any(char.IsUpper) ||
                !model.NewPassword.Any(char.IsDigit))
                    return BadRequest("New password does not meet the requirements");

            // Change the user's password
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.First().Description);

            return Ok("Password changed successfully");
        }


        [HttpPost("updatecontactinfo")]
        public IActionResult UpdateContactInfo([FromBody] ContactInfoModel model)
        {
            if (model == null)
                return BadRequest("Invalid request body");

            // Obține utilizatorul curent autentificat
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound("User not found");

            // Actualizează numele, dacă este specificat
            if (!string.IsNullOrEmpty(model.FirstName))
                user.FirstName = model.FirstName;

            if (!string.IsNullOrEmpty(model.LastName))
                user.LastName = model.LastName;

            // Actualizează numărul de telefon, dacă este specificat și este valid
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                if (!Regex.IsMatch(model.PhoneNumber, @"^\d{10}$"))
                    return BadRequest("Invalid phone number format");
                user.PhoneNumber = model.PhoneNumber;
            }

            // Actualizează adresa de email, dacă este specificată și este validă
            if (!string.IsNullOrEmpty(model.Email))
            {
                if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    return BadRequest("Invalid email address format");
                user.Email = model.Email;
            }


            // Actualizează lista de magazine preferate, dacă este specificată și nu depășește limita maximă de 5
            if (model.FavoriteStores != null)
            {
                if (model.FavoriteStores.Count > 5)
                    return BadRequest("Favorite stores list exceeds maximum size of 5");
                user.FavoriteStores = model.FavoriteStores;
            }

            // Actualizează lista de produse preferate, dacă este specificată
            if (model.FavoriteProducts != null)
                user.FavoriteProducts = model.FavoriteProducts;

            // Salvează modificările în baza de date
            _context.SaveChanges();

            return Ok("Contact information updated successfully");
        }

        // [HttpGet("orderhistory")]
        // public IActionResult GetOrderHistory()
        // {
        //     // implementare afisare istoric comenzi
        // }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout successful");
        }

        [HttpPost("deleteaccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return BadRequest("User not found");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                return BadRequest("Failed to delete user");

            await _signInManager.SignOutAsync();
            return Ok("Account deleted successfully");
        }
    }
}

using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsWearAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _userService;

        public AdminUserController(IAdminUserService userService)
        {
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search)
        {
            var users = await _userService.SearchUsers(search);

            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);

            if (user == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(user);
        }


        [HttpPut("{id}/toggle-block")]
        public async Task<IActionResult> ToggleBlockUser(int id)
        {
            var result = await _userService.ToggleBlockUser(id);

            if (!result)
                return NotFound(new { message = "Customer not found" });

            var isBlocked = await _userService.IsUserBlocked(id);

            return Ok(new
            {
                message = isBlocked
                    ? "Customer blocked successfully"
                    : "Customer unblocked successfully",
                isBlocked
            });
        }

    }
}



using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidsWearAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminDashboardController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        
        [HttpGet("test")]
        public IActionResult TestAdminAccess()
        {
            return Ok(new
            {
                message = "Admin access granted successfully!"
            });
        }

        
        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardResponse>>
            GetDashboard()
        {
            var dashboard = await _adminDashboardService.GetDashboard();

            return Ok(dashboard);
        }
    }
}

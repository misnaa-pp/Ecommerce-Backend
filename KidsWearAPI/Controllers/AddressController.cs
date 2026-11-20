using KidsWearAPI.DTOs;
using KidsWearAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KidsWearAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses = await _addressService.GetAddresses(GetUserId());

            return Ok(addresses);
        }


        [HttpPost]
        public async Task<IActionResult> AddAddress(AddressRequest request)
        {
            var result = await _addressService.AddAddress(GetUserId(), request);

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(
            int id, AddressRequest request)
        {
            var result = await _addressService.UpdateAddress(
                GetUserId(), id, request);

            if (result == null)
                return NotFound("Address not found.");

            return Ok(result);
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var result = await _addressService.DeleteAddress(
                GetUserId(), id);

            if (!result)
                return NotFound("Address not found.");

            return Ok("Address deleted successfully.");
        }

        
        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var result = await _addressService.SetDefaultAddress(
                GetUserId(), id);

            if (!result)
                return NotFound("Address not found.");

            return Ok("Default address updated successfully.");
        }
    }
}
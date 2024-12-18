﻿using log4net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextStopAPIs.DTOs;
using NextStopAPIs.Services;
using System.Security.Claims;

namespace NextStopEndAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILog _logger;

        public UsersController(IUserService userService, ILog logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim is missing.");
                }

                var userId = int.Parse(userIdClaim.Value);

                var user = await _userService.GetUserById(userId);

                if (user == null)
                {
                    return NotFound("User not found.");  
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching current user", ex);
                return StatusCode(500, "An error occurred while fetching the user.");
            }
        }


        [Authorize(Roles = "admin")]
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsers();

                if (users == null || !users.Any())  
                {
                    return NotFound("No users found.");
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.Error("Error fetching all users", ex);
                return StatusCode(500, "An error occurred while fetching all users.");
            }
        }

        [Authorize]
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim is missing.");
                }

                var userId = int.Parse(userIdClaim.Value);

                //if (id != userId && !User.IsInRole("admin"))
                //{
                //    return Forbid(); 
                //}

                var user = await _userService.GetUserById(id);

                if (user == null)
                { 
                    return NotFound($"User with ID {id} not found.");
                }


                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error fetching user by ID {id}", ex); 
                return StatusCode(500, "An error occurred while fetching the user.");
            }
        }

        [Authorize]
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO userDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim is missing.");
                }

                var userId = int.Parse(userIdClaim.Value);

                if (id != userId && !User.IsInRole("admin"))
                {
                    return Forbid(); 
                }

                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                var updatedUser = await _userService.UpdateUser(id, userDTO);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating user {id}", ex);
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [Authorize]
        [HttpPost("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim is missing.");
                }

                var userId = int.Parse(userIdClaim.Value);

                if (id != userId && !User.IsInRole("admin"))
                {
                    return Forbid(); 
                }

                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                var deletedUser = await _userService.DeleteUser(id);
                return Ok(deletedUser);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user {id}", ex); // Log exception
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("reactivate/{id}")]
        public async Task<IActionResult> ReactivateUser(int id)
        {
            try
            {
                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                var reactivatedUser = await _userService.ReactivateUser(id);
                return Ok(reactivatedUser);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error reactivating user {id}", ex); 
                return StatusCode(500, "An error occurred while reactivating the user.");
            }
        }

        [Authorize]
        [HttpPut("reset-email/{id}")]
        public async Task<IActionResult> ResetEmail(int id, [FromBody] string newEmail)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim is missing.");
                }

                var userId = int.Parse(userIdClaim.Value);

                if (id != userId && !User.IsInRole("admin"))
                {
                    return Forbid(); // Admin users can reset any user's email, but regular users can only reset their own email
                }

                // Check if the new email is unique
                var isEmailUnique = await _userService.IsEmailUnique(newEmail);
                if (!isEmailUnique)
                {
                    return BadRequest("The new email is already in use by another user.");
                }

                var existingUser = await _userService.GetUserById(id);
                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found.");
                }

                await _userService.ResetEmail(id, newEmail);
                return Ok(new UserResponseDTO
                {
                    Success = true,
                    Message = "Email reset successfully."
                }); 
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resetting email for user {id}", ex); 
                return StatusCode(500, "An error occurred while resetting the email.");
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDto)
        {
            try
            {
                var userEmail = resetPasswordDto.Email;
                var newPassword = resetPasswordDto.NewPassword;

                if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(newPassword))
                {
                    return BadRequest("Email and new password must be provided.");
                }

                var existingUser = await _userService.GetUserByEmail(userEmail);
                if (existingUser == null)
                {
                    return NotFound($"User with email {userEmail} not found.");
                }

                await _userService.ResetPassword(userEmail, newPassword);

                return Ok(new UserResponseDTO
                {
                    Success = true,
                    Message = "Password reset successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resetting password for email {resetPasswordDto.Email}", ex);
                return StatusCode(500, "An error occurred while resetting the password.");
            }
        }

    }
}

using GigGarden.Models.Entities;
using GigGarden.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GigGarden.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UserController(IConfiguration config)
        {
            _userRepository = new UserRepository(config);
        }

        [HttpGet("TestConnection")]
        public IActionResult TestConnection()
        {
            var result = _userRepository.TestConnection();
            return Ok(result);
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers(bool includeDeleted = false, bool onlyDeleted = false)
        {
            var users = _userRepository.GetUsers(includeDeleted, onlyDeleted);
            return Ok(users);
        }

        [HttpGet("GetSingleUser/{userId}")]
        public IActionResult GetSingleUser(int userId)
        {
            var user = _userRepository.GetSingleUser(userId);
            return user != null ? Ok(user) : NotFound("User not found.");
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            bool success = _userRepository.EditUser(user);
            return success ? Ok() : BadRequest("No fields provided for update.");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(User user)
        {
            bool success = _userRepository.AddUser(user);
            return success ? Ok() : BadRequest("Failed to add user.");
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId, int deletedBy)
        {
            bool success = _userRepository.SoftDeleteUser(userId, deletedBy);
            return success ? Ok() : BadRequest("Failed to delete user.");
        }

        [HttpDelete("PermanentlyDeleteUser/{userId}")]
        public IActionResult PermanentlyDeleteUser(int userId)
        {
            bool success = _userRepository.PermanentlyDeleteUser(userId);
            return success ? Ok() : BadRequest("Failed to permanently delete user.");
        }
    }
}

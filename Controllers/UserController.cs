
using Dapper;
using GigGarden.Data;
using GigGarden.Helpers;
using GigGarden.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GigGarden.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        //[HttpGet("GetUsers")]
        //public IActionResult GetUsers()
        //{
        //    try
        //    {
        //        string sql = @"
        //    SELECT [UserId], [UserName], [GivenName], [Email], [ProfilePictureUrl], [Description] 
        //    FROM dbo.Users";  // Use dbo.[User] instead of [User]

        //        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        //        return Ok(users);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error retrieving users: {ex.Message}");
        //    }
        //}
        //[HttpGet("GetUsers")]
        //public IEnumerable<User> GetUsers()
        //{
        //    string sql = @"
        //        SELECT [UserId],
        //            [UserName],
        //            [GivenName],
        //            [Email],
        //            [ProfilePictureUrl],
        //            [Description] 
        //        FROM dbo.[User]";
        //    IEnumerable<User> users = _dapper.LoadData<User>(sql);
        //    return users;
        //}

        [HttpGet("GetUsers")]
        public IEnumerable<User> GetUsers()
        {
            string sql = @"
                SELECT 
                    UserId, 
                    UserName, 
                    GivenName, 
                    Email, 
                    ProfilePictureUrl, 
                    Description,
                    CreatedAt,
                    CreatedOffset,
                    CreatedBy,
                    UpdatedAt, 
                    UpdatedOffset, 
                    UpdatedBy, 
                    DeletedAt, 
                    DeletedOffset, 
                    DeletedBy
                FROM dbo.[User]";

            IEnumerable<User> users = _dapper.LoadData<User>(sql);
            return users;
        }


        [HttpGet("GetSingleUser/{userId}")]
        public User GetSingleUser(int userId)
        {
            string sql = @"
                SELECT  
                    UserId, 
                    UserName, 
                    GivenName, 
                    Email, 
                    ProfilePictureUrl, 
                    Description,
                    CreatedAt,
                    CreatedOffset,
                    CreatedBy,
                    UpdatedAt, 
                    UpdatedOffset, 
                    UpdatedBy, 
                    DeletedAt, 
                    DeletedOffset, 
                    DeletedBy
                FROM dbo.[User]
                    WHERE UserId = " + userId.ToString();
            User user = _dapper.LoadDataSingle<User>(sql);
            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            string sql = @"
                UPDATE dbo.[User]
                    SET [UserName] = '" + user.UserName +
                        "', [GivenName] = '" + user.GivenName +
                        "', [Email] = '" + user.Email +
                        "', [ProfilePictureUrl] = '" + user.ProfilePictureUrl +
                        "', [Description] = '" + user.Description +
                    "' WHERE UserId = " + user.UserId;

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Update User");
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(User user)
        {
            var (timestamp, offset) = TimeHelper.GetTimestampWithOffset();

            string sql = @"
                INSERT INTO dbo.[User] (
                    UserName, 
                    GivenName, 
                    Email, 
                    ProfilePictureUrl, 
                    Description, 
                    CreatedBy, 
                    CreatedAt, 
                    CreatedOffset
                ) VALUES (
                    @UserName, 
                    @GivenName, 
                    @Email, 
                    @ProfilePictureUrl, 
                    @Description, 
                    @CreatedBy, 
                    @CreatedAt, 
                    @CreatedOffset
                )";

            var parameters = new
            {
                user.UserName,
                user.GivenName,
                user.Email,
                user.ProfilePictureUrl,
                user.Description,
                user.CreatedBy, // Placeholder for actual user ID
                CreatedAt = timestamp,
                CreatedOffset = offset
            };

            bool success = _dapper.ExecuteSqlWithParameters(sql, parameters);
            if (success)
            {
                return Ok();
            }

            throw new Exception("Failed to Add User");
        }



        //[HttpPost("AddUser")]
        //public IActionResult AddUser(User user)
        //{
        //    var (timestamp, offset) = TimeHelper.GetTimestampWithOffset();

        //    var parameters = new
        //    {
        //        CreatedAt = timestamp,
        //        CreatedOffset = offset,
        //        /*CreatedBy = userId */// Replace with actual user when authentication is set up
        //    };

        //    string sql = @"
        //    INSERT INTO dbo.[User](
        //        [UserName],
        //        [GivenName],
        //        [Email],
        //        [ProfilePictureUrl],
        //        [Description],
        //        [CreatedBy],
        //        [CreatedAt],
        //        [CreatedOffset]
        //    ) VALUES (" +
        //            "'" + user.UserName +
        //            "', '" + user.GivenName +
        //            "', '" + user.Email +
        //            "', '" + user.ProfilePictureUrl +
        //            "', '" + user.Description +
        //            "', '" + user.CreatedBy +
        //            "', '" + parameters.CreatedAt +
        //            "', '" + parameters.CreatedOffset +
        //        "')";

        //    Console.WriteLine(sql);

        //    if (_dapper.ExecuteSql(sql))
        //    {
        //        return Ok();
        //    }

        //    throw new Exception("Failed to Add User");
        //}

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId)
        {
            string sql = @"
            DELETE FROM dbo.[User] 
                WHERE UserId = " + userId.ToString();

            Console.WriteLine(sql);

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Failed to Delete User");
        }

    } 
}


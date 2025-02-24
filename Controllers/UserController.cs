
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
        public IEnumerable<User> GetUsers(bool includeDeleted = false, bool onlyDeleted = false)
        {
            string sql = @"
        SELECT UserId, UserName, GivenName, Email, ProfilePictureUrl, Description, 
               CreatedAt, CreatedOffset, CreatedBy, 
               UpdatedAt, UpdatedOffset, UpdatedBy, 
               DeletedAt, DeletedOffset, DeletedBy
        FROM dbo.[User]";

            if(onlyDeleted)
            {
                sql += " WHERE DeletedAt IS NOT NULL";
            }

            else if (!includeDeleted)
            {
                sql += " WHERE DeletedAt IS NULL";
            }

            return _dapper.LoadData<User>(sql);
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

        //[HttpPut("EditUser")]
        //public IActionResult EditUser(User user)
        //{
        //    string sql = @"
        //        UPDATE dbo.[User]
        //            SET [UserName] = '" + user.UserName +
        //                "', [GivenName] = '" + user.GivenName +
        //                "', [Email] = '" + user.Email +
        //                "', [ProfilePictureUrl] = '" + user.ProfilePictureUrl +
        //                "', [Description] = '" + user.Description +
        //            "' WHERE UserId = " + user.UserId;

        //    Console.WriteLine(sql);

        //    if (_dapper.ExecuteSql(sql))
        //    {
        //        return Ok();
        //    }

        //    throw new Exception("Failed to Update User");
        //}

        [HttpPut("EditUser")]
        public IActionResult EditUser(User user)
        {
            var (timestamp, offset) = TimeHelper.GetTimestampWithOffset();

            List<string> updateFields = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(user.UserName))
            {
                updateFields.Add("[UserName] = @UserName");
                parameters.Add("@UserName", user.UserName);
            }
            if (!string.IsNullOrEmpty(user.GivenName))
            {
                updateFields.Add("[GivenName] = @GivenName");
                parameters.Add("@GivenName", user.GivenName);
            }
            if (!string.IsNullOrEmpty(user.Email))
            {
                updateFields.Add("[Email] = @Email");
                parameters.Add("@Email", user.Email);
            }
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
            {
                updateFields.Add("[ProfilePictureUrl] = @ProfilePictureUrl");
                parameters.Add("@ProfilePictureUrl", user.ProfilePictureUrl);
            }
            if (!string.IsNullOrEmpty(user.Description))
            {
                updateFields.Add("[Description] = @Description");
                parameters.Add("@Description", user.Description);
            }

            // Ensure at least one field is being updated
            if (!updateFields.Any())
            {
                return BadRequest("No fields provided for update.");
            }

            // Always update the timestamp and updater info
            updateFields.Add("[UpdatedAt] = @UpdatedAt");
            updateFields.Add("[UpdatedOffset] = @UpdatedOffset");
            updateFields.Add("[UpdatedBy] = @UpdatedBy");

            parameters.Add("@UpdatedAt", timestamp);
            parameters.Add("@UpdatedOffset", offset);
            parameters.Add("@UpdatedBy", user.UpdatedBy); // Replace with actual user when authentication is set up

            // Build dynamic SQL query
            string sql = $@"
                UPDATE dbo.[User]
                SET {string.Join(", ", updateFields)}
                WHERE UserId = @UserId";

            parameters.Add("@UserId", user.UserId);

            bool success = _dapper.ExecuteSqlWithParameters(sql, parameters);

            if (success)
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
        public IActionResult DeleteUser(int userId, int deletedBy)
        {
            var (timestamp, offset) = TimeHelper.GetTimestampWithOffset();

            string sql = @"
                UPDATE dbo.[User]
                SET DeletedAt = @DeletedAt,
                    DeletedOffset = @DeletedOffset,
                    DeletedBy = @DeletedBy
                WHERE UserId = @UserId";

            var parameters = new
            {
                DeletedAt = timestamp,
                DeletedOffset = offset,
                DeletedBy = deletedBy, // Replace with authenticated user when ready
                UserId = userId
            };

            bool success = _dapper.ExecuteSqlWithParameters(sql, parameters);

            if (success)
            {
                return Ok();
            }

            throw new Exception("Failed to Soft Delete User");
        }


        [HttpDelete("PermanentlyDeleteUser/{userId}")]
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


using Dapper;
using GigGarden.Data;
using GigGarden.Helpers;
using GigGarden.Models.Entities;
using Microsoft.Data.SqlClient;

namespace GigGarden.Repositories
{
    public class UserRepository
    {
        private readonly DataContextDapper _dapper;

        public UserRepository(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        public IEnumerable<User> GetUsers(bool includeDeleted, bool onlyDeleted)
        {
            string sql = @"
                SELECT UserId, UserName, GivenName, Email, ProfilePictureUrl, Description, 
                       CreatedAt, CreatedOffset, CreatedBy, 
                       UpdatedAt, UpdatedOffset, UpdatedBy, 
                       DeletedAt, DeletedOffset, DeletedBy
                FROM dbo.[User]";

            if (onlyDeleted)
            {
                sql += " WHERE DeletedAt IS NOT NULL";
            }
            else if (!includeDeleted)
            {
                sql += " WHERE DeletedAt IS NULL";
            }

            return _dapper.LoadData<User>(sql);
        }

        public User GetSingleUser(int userId)
        {
            string sql = @"
                SELECT UserId, UserName, GivenName, Email, ProfilePictureUrl, Description,
                       CreatedAt, CreatedOffset, CreatedBy, 
                       UpdatedAt, UpdatedOffset, UpdatedBy, 
                       DeletedAt, DeletedOffset, DeletedBy
                FROM dbo.[User]
                WHERE UserId = @UserId";

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            try
            {
                return _dapper.LoadDataSingleWithParameters<User>(sql, parameters);
            }
            catch (InvalidOperationException)
            {
                // No user found, return null
                return null;
            }
        }

        public bool EditUser(User user)
        {
            var (updatedAt, updatedOffset) = TimeHelper.GetCurrentTimestamp();
            List<string> updateFields = new List<string>();
            var parameters = new DynamicParameters();

            // Fetch existing user data
            var existingUser = GetSingleUser(user.UserId);
            if (existingUser == null) return false; // User not found

            // Compare fields before updating
            if (!string.IsNullOrEmpty(user.UserName) && user.UserName != existingUser.UserName)
            {
                updateFields.Add("[UserName] = @UserName");
                parameters.Add("@UserName", user.UserName);
            }
            if (!string.IsNullOrEmpty(user.GivenName) && user.GivenName != existingUser.GivenName)
            {
                updateFields.Add("[GivenName] = @GivenName");
                parameters.Add("@GivenName", user.GivenName);
            }
            if (!string.IsNullOrEmpty(user.Email) && user.Email != existingUser.Email)
            {
                updateFields.Add("[Email] = @Email");
                parameters.Add("@Email", user.Email);
            }
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && user.ProfilePictureUrl != existingUser.ProfilePictureUrl)
            {
                updateFields.Add("[ProfilePictureUrl] = @ProfilePictureUrl");
                parameters.Add("@ProfilePictureUrl", user.ProfilePictureUrl);
            }
            if (!string.IsNullOrEmpty(user.Description) && user.Description != existingUser.Description)
            {
                updateFields.Add("[Description] = @Description");
                parameters.Add("@Description", user.Description);
            }

            // If no actual changes, return false
            if (!updateFields.Any()) return false;

            // Always update the timestamp and updater info if something is modified
            updateFields.Add("[UpdatedAt] = @UpdatedAt");
            updateFields.Add("[UpdatedOffset] = @UpdatedOffset");
            updateFields.Add("[UpdatedBy] = @UpdatedBy");

            parameters.Add("@UpdatedAt", updatedAt);
            parameters.Add("@UpdatedOffset", updatedOffset);
            parameters.Add("@UpdatedBy", user.UpdatedBy);
            parameters.Add("@UserId", user.UserId);

            string sql = $"UPDATE dbo.[User] SET {string.Join(", ", updateFields)} WHERE UserId = @UserId";

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool AddUser(User user)
        {
            var (createdAt, createdOffset) = TimeHelper.GetCurrentTimestamp();

            string sql = @"
                INSERT INTO dbo.[User] (UserName, GivenName, Email, ProfilePictureUrl, Description, 
                                        CreatedBy, CreatedAt, CreatedOffset)
                VALUES (@UserName, @GivenName, @Email, @ProfilePictureUrl, @Description, 
                        @CreatedBy, @CreatedAt, @CreatedOffset)";

            var parameters = new
            {
                user.UserName,
                user.GivenName,
                user.Email,
                user.ProfilePictureUrl,
                user.Description,
                user.CreatedBy,
                CreatedAt = createdAt,
                CreatedOffset = createdOffset
            };

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool SoftDeleteUser(int userId, int deletedBy)
        {
            var (deletedAt, deletedOffset) = TimeHelper.GetCurrentTimestamp();

            string sql = @"
                UPDATE dbo.[User]
                SET DeletedAt = @DeletedAt,
                    DeletedOffset = @DeletedOffset,
                    DeletedBy = @DeletedBy
                WHERE UserId = @UserId";

            var parameters = new
            {
                DeletedAt = deletedAt,
                DeletedOffset = deletedOffset,
                DeletedBy = deletedBy,
                UserId = userId
            };

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool PermanentlyDeleteUser(int userId)
        {
            string sql = "DELETE FROM dbo.[User] WHERE UserId = @UserId";
            return _dapper.ExecuteSqlWithParameters(sql, new { UserId = userId });
        }
    }
}

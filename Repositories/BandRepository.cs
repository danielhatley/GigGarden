using Dapper;
using GigGarden.Data;
using GigGarden.Helpers;
using GigGarden.Models.Entities;
using Microsoft.Data.SqlClient;

namespace GigGarden.Repositories
{
    public class BandRepository
    {
        private readonly DataContextDapper _dapper;

        public BandRepository(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        public IEnumerable<Band> GetBands(bool includeDeleted, bool onlyDeleted)
        {
            string sql = @"
                SELECT BandId, BandName, Genre, FoundedYear, FoundingTown, Description,
                       CreatedAt, CreatedOffset, CreatedBy, 
                       UpdatedAt, UpdatedOffset, UpdatedBy, 
                       DeletedAt, DeletedOffset, DeletedBy
                FROM dbo.[Band]";

            if (onlyDeleted)
            {
                sql += " WHERE DeletedAt IS NOT NULL";
            }
            else if (!includeDeleted)
            {
                sql += " WHERE DeletedAt IS NULL";
            }

            return _dapper.LoadData<Band>(sql);
        }

        public Band GetSingleBand(int bandId)
        {
            string sql = @"
                SELECT BandId, BandName, Genre, FoundedYear, FoundingTown, Description,
                       CreatedAt, CreatedOffset, CreatedBy, 
                       UpdatedAt, UpdatedOffset, UpdatedBy, 
                       DeletedAt, DeletedOffset, DeletedBy
                FROM dbo.[Band]
                WHERE BandId = @BandId";

            var parameters = new DynamicParameters();
            parameters.Add("@BandId", bandId);

            try
            {
                return _dapper.LoadDataSingleWithParameters<Band>(sql, parameters);
            }
            catch (InvalidOperationException)
            {
                return null; // No band found
            }
        }

        public bool EditBand(Band band)
        {
            var (updatedAt, updatedOffset) = TimeHelper.GetCurrentTimestamp();
            List<string> updateFields = new List<string>();
            var parameters = new DynamicParameters();

            var existingBand = GetSingleBand(band.BandId);
            if (existingBand == null) return false; // Band not found

            if (!string.IsNullOrEmpty(band.BandName) && band.BandName != existingBand.BandName)
            {
                updateFields.Add("[BandName] = @BandName");
                parameters.Add("@BandName", band.BandName);
            }
            if (!string.IsNullOrEmpty(band.Genre) && band.Genre != existingBand.Genre)
            {
                updateFields.Add("[Genre] = @Genre");
                parameters.Add("@Genre", band.Genre);
            }
            if (band.FoundedYear.HasValue && band.FoundedYear != existingBand.FoundedYear)
            {
                updateFields.Add("[FoundedYear] = @FoundedYear");
                parameters.Add("@FoundedYear", band.FoundedYear);
            }
            if (!string.IsNullOrEmpty(band.FoundingTown) && band.FoundingTown != existingBand.FoundingTown)
            {
                updateFields.Add("[FoundingTown] = @FoundingTown");
                parameters.Add("@FoundingTown", band.FoundingTown);
            }
            if (!string.IsNullOrEmpty(band.Description) && band.Description != existingBand.Description)
            {
                updateFields.Add("[Description] = @Description");
                parameters.Add("@Description", band.Description);
            }

            if (!updateFields.Any()) return false;

            updateFields.Add("[UpdatedAt] = @UpdatedAt");
            updateFields.Add("[UpdatedOffset] = @UpdatedOffset");
            updateFields.Add("[UpdatedBy] = @UpdatedBy");

            parameters.Add("@UpdatedAt", updatedAt);
            parameters.Add("@UpdatedOffset", updatedOffset);
            parameters.Add("@UpdatedBy", band.UpdatedBy);
            parameters.Add("@BandId", band.BandId);

            string sql = $"UPDATE dbo.[Band] SET {string.Join(", ", updateFields)} WHERE BandId = @BandId";

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool AddBand(Band band)
        {
            var (createdAt, createdOffset) = TimeHelper.GetCurrentTimestamp();

            string sql = @"
                INSERT INTO dbo.[Band] (BandName, Genre, FoundedYear, FoundingTown, Description, 
                                        CreatedBy, CreatedAt, CreatedOffset)
                VALUES (@BandName, @Genre, @FoundedYear, @FoundingTown, @Description, 
                        @CreatedBy, @CreatedAt, @CreatedOffset)";

            var parameters = new
            {
                band.BandName,
                band.Genre,
                band.FoundedYear,
                band.FoundingTown,
                band.Description,
                band.CreatedBy,
                CreatedAt = createdAt,
                CreatedOffset = createdOffset
            };

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool SoftDeleteBand(int bandId, int deletedBy)
        {
            var (deletedAt, deletedOffset) = TimeHelper.GetCurrentTimestamp();

            string sql = @"
                UPDATE dbo.[Band]
                SET DeletedAt = @DeletedAt,
                    DeletedOffset = @DeletedOffset,
                    DeletedBy = @DeletedBy
                WHERE BandId = @BandId";

            var parameters = new
            {
                DeletedAt = deletedAt,
                DeletedOffset = deletedOffset,
                DeletedBy = deletedBy,
                BandId = bandId
            };

            return _dapper.ExecuteSqlWithParameters(sql, parameters);
        }

        public bool PermanentlyDeleteBand(int bandId)
        {
            string sql = "DELETE FROM dbo.[Band] WHERE BandId = @BandId";
            return _dapper.ExecuteSqlWithParameters(sql, new { BandId = bandId });
        }
    }
}

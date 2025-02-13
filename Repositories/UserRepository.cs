

using GigGarden.Data;
using GigGarden.Models;

namespace GigGarden.Repositories;

public class UserRepository
{
    private readonly DataContextDapper _dapper;

    public UserRepository(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    public async Task<List<User>> GetUser(int? userId, bool showDeleted = false)
    {

        return new List<User>();
    }
}


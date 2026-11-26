using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management.model
{
    public class User
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

    }

    //public async Task<bool> RegisterUser(User user)
    //{
    //    try
    //    {
    //        user.CreatedAt = DateTime.UtcNow;
    //        user.IsActive = true;
    //        await _users.InsertOneAsync(user);
    //        return true;
    //    }
    //    catch { return false; }
    //}

    //public async Task<User> LoginUser(string username, string password)
    //{
    //    var filter = Builders<User>.Filter.And(
    //        Builders<User>.Filter.Eq(u => u.Username, username),
    //        Builders<User>.Filter.Eq(u => u.Password, password)
    //    );
    //    return await _users.Find(filter).FirstOrDefaultAsync();
    //}
}

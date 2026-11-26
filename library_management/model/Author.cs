using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management.model
{
    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Biography { get; set; }
        //public async Task<bool> CreateAuthor(Author author)
        //{
        //    try
        //    {
        //        author.CreatedAt = DateTime.UtcNow;
        //        author.UpdatedAt = DateTime.UtcNow;
        //        await _authors.InsertOneAsync(author);
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public async Task<List<Author>> GetAllAuthors()
        //{
        //    return await _authors.Find(_ => true).ToListAsync();
        //}

        //public async Task<bool> UpdateAuthor(Author author)
        //{
        //    try
        //    {
        //        author.UpdatedAt = DateTime.UtcNow;
        //        var filter = Builders<Author>.Filter.Eq(a => a.Id, author.Id);
        //        await _authors.ReplaceOneAsync(filter, author);
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public async Task<bool> DeleteAuthor(ObjectId id)
        //{
        //    try
        //    {
        //        var filter = Builders<Author>.Filter.Eq(a => a.Id, id);
        //        await _authors.DeleteOneAsync(filter);
        //        return true;
        //    }
        //    catch { return false; }
        //}
    }
}

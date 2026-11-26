using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library_management.model
{
    public class Category

    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //    public async Task<bool> CreateCategory(Category category) 
        //    {
        //        try
        //        {
        //            category.CreatedAt = DateTime.UtcNow;
        //            category.UpdatedAt = DateTime.UtcNow;
        //            await _categories.InsertOneAsync(category);
        //            return true;
        //        }
        //        catch { return false; }
        //    }

        //public async Task<List<Category>> GetAllCategories()
        //{
        //    return await _categories.Find(_ => true).ToListAsync();
        //}

        //    public async Task<bool> UpdateCategory(Category category)
        //    {
        //        try
        //        {
        //            category.UpdatedAt = DateTime.UtcNow;
        //            var filter = Builders<Category>.Filter.Eq(c => c.Id, category.Id);
        //            await _categories.ReplaceOneAsync(filter, category);
        //            return true;
        //        }
        //        catch { return false; }
        //    }

        //    public async Task<bool> DeleteCategory(ObjectId id)
        //    {
        //        try
        //        {
        //            var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
        //            await _categories.DeleteOneAsync(filter);
        //            return true;
        //        }
        //        catch { return false; }
        //    }
    }
}

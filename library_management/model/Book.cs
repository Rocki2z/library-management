using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace library_management.model
{
    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string AuthorId { get; set; }
        public string PublishDate { get; set; }
        public int Copies { get; set; } = 1;
        public int AvailableCopies { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }


        //public async Task<bool> CreateBook(Book book)
        //{
        //    try
        //    {
        //        book.CreatedAt = DateTime.UtcNow;
        //        book.UpdatedAt = DateTime.UtcNow;
        //        book.IsAvailable = true;
        //        await _books.InsertOneAsync(book);
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public async Task<List<Book>> GetAllBooks()
        //{
        //    return await _books.Find(_ => true).ToListAsync();
        //}

        //public async Task<List<Book>> SearchBooks(string searchTerm)
        //{
        //    var filter = Builders<Book>.Filter.Regex(b => b.Title, new BsonRegularExpression(searchTerm, "i"));
        //    return await _books.Find(filter).ToListAsync();
        //}

        //public async Task<bool> UpdateBook(Book book)
        //{
        //    try
        //    {
        //        book.UpdatedAt = DateTime.UtcNow;
        //        var filter = Builders<Book>.Filter.Eq(b => b.Id, book.Id);
        //        await _books.ReplaceOneAsync(filter, book);
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public async Task<bool> DeleteBook(ObjectId id)
        //{
        //    try
        //    {
        //        var filter = Builders<Book>.Filter.Eq(b => b.Id, id);
        //        await _books.DeleteOneAsync(filter);
        //        return true;
        //    }
        //    catch { return false; }
        //}

        //public async Task<Category> GetCategoryById(ObjectId id)
        //{
        //    return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
        //}

        //public async Task<Author> GetAuthorById(ObjectId id)
        //{
        //    return await _authors.Find(a => a.Id == id).FirstOrDefaultAsync();
        //}
    }
}


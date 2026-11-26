using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Npgsql;

namespace library_management.model
{
    /// <summary>PostgreSQL access for the library application.</summary>
    public static class DatabaseService
    {
        public static NpgsqlConnection Open()
        {
            return LibraryDbContext.OpenConnection();
        }

        public static void Initialize()
        {
            using (var c = Open())
            using (var cmd = new NpgsqlCommand(@"
CREATE TABLE IF NOT EXISTS users (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), username text NOT NULL UNIQUE, password_hash text NOT NULL, password_salt text NOT NULL, role text NOT NULL DEFAULT 'user');
CREATE TABLE IF NOT EXISTS authors (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), name text NOT NULL, biography text NOT NULL DEFAULT '');
CREATE TABLE IF NOT EXISTS categories (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), name text NOT NULL UNIQUE, description text NOT NULL DEFAULT '');
CREATE TABLE IF NOT EXISTS books (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), title text NOT NULL, description text NOT NULL DEFAULT '', category_id uuid REFERENCES categories(id) ON DELETE RESTRICT, author_id uuid REFERENCES authors(id) ON DELETE RESTRICT, publish_date date, copies integer NOT NULL DEFAULT 1 CHECK (copies > 0));
CREATE TABLE IF NOT EXISTS members (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), name text NOT NULL, email text NOT NULL UNIQUE, phone text NOT NULL DEFAULT '');
CREATE TABLE IF NOT EXISTS loans (id uuid PRIMARY KEY DEFAULT gen_random_uuid(), book_id uuid NOT NULL REFERENCES books(id) ON DELETE RESTRICT, member_id uuid NOT NULL REFERENCES members(id) ON DELETE RESTRICT, borrowed_at date NOT NULL DEFAULT CURRENT_DATE, due_at date NOT NULL, returned_at date NULL);
CREATE INDEX IF NOT EXISTS ix_loans_active_book ON loans(book_id) WHERE returned_at IS NULL;
", c)) cmd.ExecuteNonQuery();
        }

        private static string Hash(string password, byte[] salt)
        {
            using (var derive = new Rfc2898DeriveBytes(password, salt, 120000)) return Convert.ToBase64String(derive.GetBytes(32));
        }

        public static bool RegisterUser(string username, string password)
        {
            var salt = new byte[16]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(salt);
            using (var c = Open()) using (var cmd = new NpgsqlCommand("INSERT INTO users(username,password_hash,password_salt) VALUES (@u,@h,@s) ON CONFLICT (username) DO NOTHING", c))
            {
                cmd.Parameters.AddWithValue("u", username.Trim()); cmd.Parameters.AddWithValue("h", Hash(password, salt)); cmd.Parameters.AddWithValue("s", Convert.ToBase64String(salt));
                return cmd.ExecuteNonQuery() == 1;
            }
        }

        public static User Login(string username, string password)
        {
            using (var c = Open()) using (var cmd = new NpgsqlCommand("SELECT id, username, role, password_hash, password_salt FROM users WHERE lower(username)=lower(@u)", c))
            {
                cmd.Parameters.AddWithValue("u", username.Trim()); using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var salt = Convert.FromBase64String(r.GetString(4));
                    if (!SlowEquals(Hash(password, salt), r.GetString(3))) return null;
                    return new User { Id = r.GetGuid(0).ToString(), Username = r.GetString(1), Role = r.GetString(2) };
                }
            }
        }
        private static bool SlowEquals(string a, string b) { var x = Convert.FromBase64String(a); var y = Convert.FromBase64String(b); if (x.Length != y.Length) return false; int d = 0; for (int i = 0; i < x.Length; i++) d |= x[i] ^ y[i]; return d == 0; }

        public static List<Author> Authors(string search = "") { var x = new List<Author>(); using (var c = Open()) using (var q = new NpgsqlCommand("SELECT id,name,biography FROM authors WHERE @s='' OR name ILIKE @p OR biography ILIKE @p ORDER BY name", c)) { q.Parameters.AddWithValue("s", search ?? ""); q.Parameters.AddWithValue("p", "%" + (search ?? "") + "%"); using (var r=q.ExecuteReader()) while(r.Read()) x.Add(new Author{Id=r.GetGuid(0).ToString(),Name=r.GetString(1),Biography=r.GetString(2)}); } return x; }
        public static void SaveAuthor(Author a) { using(var c=Open()) using(var q=new NpgsqlCommand(a.Id==null?"INSERT INTO authors(name,biography) VALUES(@n,@b)":"UPDATE authors SET name=@n,biography=@b WHERE id=@id",c)){q.Parameters.AddWithValue("n",a.Name);q.Parameters.AddWithValue("b",a.Biography??"");if(a.Id!=null)q.Parameters.AddWithValue("id",Guid.Parse(a.Id));q.ExecuteNonQuery();} }
        public static void DeleteAuthor(string id) { using(var c=Open()) using(var q=new NpgsqlCommand("DELETE FROM authors WHERE id=@id",c)){q.Parameters.AddWithValue("id",Guid.Parse(id));q.ExecuteNonQuery();} }
        public static List<Category> Categories(string search = "") { var x=new List<Category>();using(var c=Open())using(var q=new NpgsqlCommand("SELECT id,name,description FROM categories WHERE @s='' OR name ILIKE @p OR description ILIKE @p ORDER BY name",c)){q.Parameters.AddWithValue("s",search??"");q.Parameters.AddWithValue("p","%"+(search??"")+"%");using(var r=q.ExecuteReader())while(r.Read())x.Add(new Category{Id=r.GetGuid(0).ToString(),Name=r.GetString(1),Description=r.GetString(2)});}return x; }
        public static void SaveCategory(Category a) { using(var c=Open())using(var q=new NpgsqlCommand(a.Id==null?"INSERT INTO categories(name,description) VALUES(@n,@d)":"UPDATE categories SET name=@n,description=@d WHERE id=@id",c)){q.Parameters.AddWithValue("n",a.Name);q.Parameters.AddWithValue("d",a.Description??"");if(a.Id!=null)q.Parameters.AddWithValue("id",Guid.Parse(a.Id));q.ExecuteNonQuery();} }
        public static void DeleteCategory(string id) { using(var c=Open())using(var q=new NpgsqlCommand("DELETE FROM categories WHERE id=@id",c)){q.Parameters.AddWithValue("id",Guid.Parse(id));q.ExecuteNonQuery();} }
        public static List<Book> Books(string search = "") { var x=new List<Book>(); using(var c=Open())using(var q=new NpgsqlCommand("SELECT b.id,b.title,b.description,b.category_id,b.author_id,COALESCE(to_char(b.publish_date,'YYYY-MM-DD'),''),b.copies,b.copies-(SELECT count(*) FROM loans l WHERE l.book_id=b.id AND l.returned_at IS NULL),COALESCE(a.name,'Unknown'),COALESCE(c.name,'Unknown') FROM books b LEFT JOIN authors a ON a.id=b.author_id LEFT JOIN categories c ON c.id=b.category_id WHERE @s='' OR b.title ILIKE @p OR b.description ILIKE @p OR a.name ILIKE @p ORDER BY b.title",c)){q.Parameters.AddWithValue("s",search??"");q.Parameters.AddWithValue("p","%"+(search??"")+"%");using(var r=q.ExecuteReader())while(r.Read())x.Add(new Book{Id=r.GetGuid(0).ToString(),Title=r.GetString(1),Description=r.GetString(2),Category=r.IsDBNull(3)?null:r.GetGuid(3).ToString(),AuthorId=r.IsDBNull(4)?null:r.GetGuid(4).ToString(),PublishDate=r.GetString(5),Copies=r.GetInt32(6),AvailableCopies=r.GetInt32(7),AuthorName=r.GetString(8),CategoryName=r.GetString(9)});}return x; }
        public static void SaveBook(Book b) { using(var c=Open())using(var q=new NpgsqlCommand(b.Id==null?"INSERT INTO books(title,description,category_id,author_id,publish_date,copies) VALUES(@t,@d,@c,@a,@p,@copies)":"UPDATE books SET title=@t,description=@d,category_id=@c,author_id=@a,publish_date=@p,copies=@copies WHERE id=@id AND @copies >= (SELECT count(*) FROM loans WHERE book_id=@id AND returned_at IS NULL)",c)){q.Parameters.AddWithValue("t",b.Title);q.Parameters.AddWithValue("d",b.Description??"");q.Parameters.AddWithValue("c",Guid.Parse(b.Category));q.Parameters.AddWithValue("a",Guid.Parse(b.AuthorId));DateTime dt; q.Parameters.Add("p",NpgsqlTypes.NpgsqlDbType.Date).Value=DateTime.TryParse(b.PublishDate,out dt)?(object)dt.Date:DBNull.Value;q.Parameters.AddWithValue("copies",b.Copies);if(b.Id!=null)q.Parameters.AddWithValue("id",Guid.Parse(b.Id));if(q.ExecuteNonQuery()==0&&b.Id!=null)throw new InvalidOperationException("Cannot reduce copy count below the number currently checked out, or the book no longer exists.");} }
        public static void DeleteBook(string id) { using(var c=Open())using(var q=new NpgsqlCommand("DELETE FROM books WHERE id=@id",c)){q.Parameters.AddWithValue("id",Guid.Parse(id));q.ExecuteNonQuery();} }

        public static List<Member> Members(string search="") { var x=new List<Member>();using(var c=Open())using(var q=new NpgsqlCommand("SELECT id,name,email,phone FROM members WHERE @s='' OR name ILIKE @p OR email ILIKE @p ORDER BY name",c)){q.Parameters.AddWithValue("s",search??"");q.Parameters.AddWithValue("p","%"+(search??"")+"%");using(var r=q.ExecuteReader())while(r.Read())x.Add(new Member{Id=r.GetGuid(0).ToString(),Name=r.GetString(1),Email=r.GetString(2),Phone=r.GetString(3)});}return x; }
        public static void SaveMember(Member m) { using(var c=Open())using(var q=new NpgsqlCommand(m.Id==null?"INSERT INTO members(name,email,phone) VALUES(@n,@e,@p)":"UPDATE members SET name=@n,email=@e,phone=@p WHERE id=@id",c)){q.Parameters.AddWithValue("n",m.Name);q.Parameters.AddWithValue("e",m.Email);q.Parameters.AddWithValue("p",m.Phone??"");if(m.Id!=null)q.Parameters.AddWithValue("id",Guid.Parse(m.Id));q.ExecuteNonQuery();} }
        public static List<Loan> Loans(bool activeOnly=false) { var x=new List<Loan>();using(var c=Open())using(var q=new NpgsqlCommand("SELECT l.id,l.book_id,l.member_id,b.title,m.name,l.borrowed_at,l.due_at,l.returned_at FROM loans l JOIN books b ON b.id=l.book_id JOIN members m ON m.id=l.member_id WHERE NOT @active OR l.returned_at IS NULL ORDER BY l.borrowed_at DESC",c)){q.Parameters.AddWithValue("active",activeOnly);using(var r=q.ExecuteReader())while(r.Read())x.Add(new Loan{Id=r.GetGuid(0).ToString(),BookId=r.GetGuid(1).ToString(),MemberId=r.GetGuid(2).ToString(),BookTitle=r.GetString(3),MemberName=r.GetString(4),BorrowedAt=r.GetDateTime(5),DueAt=r.GetDateTime(6),ReturnedAt=r.IsDBNull(7)?(DateTime?)null:r.GetDateTime(7)});}return x; }
        public static void Checkout(string bookId,string memberId,DateTime dueAt) { using(var c=Open())using(var tx=c.BeginTransaction()) { int copies; using(var q=new NpgsqlCommand("SELECT copies FROM books WHERE id=@id FOR UPDATE",c,tx)){q.Parameters.AddWithValue("id",Guid.Parse(bookId));var value=q.ExecuteScalar();if(value==null)throw new InvalidOperationException("Book no longer exists.");copies=Convert.ToInt32(value);} long active; using(var q=new NpgsqlCommand("SELECT count(*) FROM loans WHERE book_id=@id AND returned_at IS NULL",c,tx)){q.Parameters.AddWithValue("id",Guid.Parse(bookId));active=Convert.ToInt64(q.ExecuteScalar());}if(copies-active<1)throw new InvalidOperationException("No available copies of this book."); using(var q=new NpgsqlCommand("INSERT INTO loans(book_id,member_id,due_at) VALUES(@b,@m,@d)",c,tx)){q.Parameters.AddWithValue("b",Guid.Parse(bookId));q.Parameters.AddWithValue("m",Guid.Parse(memberId));q.Parameters.AddWithValue("d",dueAt.Date);q.ExecuteNonQuery();} tx.Commit();} }
        public static void ReturnBook(string loanId) { using(var c=Open())using(var q=new NpgsqlCommand("UPDATE loans SET returned_at=CURRENT_DATE WHERE id=@id AND returned_at IS NULL",c)){q.Parameters.AddWithValue("id",Guid.Parse(loanId));if(q.ExecuteNonQuery()==0)throw new InvalidOperationException("This loan has already been returned or no longer exists.");} }
    }
}

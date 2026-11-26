using System;
namespace library_management.model
{
    public class Loan
    {
        public string Id { get; set; }
        public string BookId { get; set; }
        public string MemberId { get; set; }
        public string BookTitle { get; set; }
        public string MemberName { get; set; }
        public DateTime BorrowedAt { get; set; }
        public DateTime DueAt { get; set; }
        public DateTime? ReturnedAt { get; set; }
    }
}

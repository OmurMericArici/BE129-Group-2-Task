namespace App.Models.DTO
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public byte StarCount { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentCreateDto
    {
        public int ProductId { get; set; }
        public string Text { get; set; } = null!;
        public byte StarCount { get; set; }
    }
}
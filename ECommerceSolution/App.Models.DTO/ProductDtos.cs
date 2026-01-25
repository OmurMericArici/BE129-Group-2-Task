namespace App.Models.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public byte StockAmount { get; set; }
        public string? Details { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int SellerId { get; set; }
        public bool Enabled { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockAmount { get; set; }
        public int CategoryId { get; set; }
        public string? Details { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockAmount { get; set; }
        public int CategoryId { get; set; }
        public string? Details { get; set; }
        public string? ImageUrl { get; set; }
    }
}
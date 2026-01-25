namespace App.Models.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = string.Empty;
    }

    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = string.Empty;
    }

    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string IconCssClass { get; set; } = string.Empty;
    }
}
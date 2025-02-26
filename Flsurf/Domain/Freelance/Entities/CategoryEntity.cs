namespace Flsurf.Domain.Freelance.Entities
{
    public class CategoryEntity : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid? ParentCategoryId { get; set; }
        public CategoryEntity ParentCategory { get; set; } = null!;
        public ICollection<CategoryEntity> SubCategories { get; set; } = []; 
    }
}

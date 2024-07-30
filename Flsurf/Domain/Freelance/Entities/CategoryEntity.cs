namespace Flsurf.Domain.Freelance.Entities
{
    public class CategoryEntity : BaseAuditableEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Tags { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public CategoryEntity ParentCategory { get; set; }
        public ICollection<CategoryEntity> SubCategories { get; set; }
    }
}

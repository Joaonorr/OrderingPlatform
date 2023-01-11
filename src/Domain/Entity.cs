using OrderingPlatform.Domain.Products;

namespace OrderingPlatform.Domain;

public abstract class Entity
{
    Entity()
    {
        Id= Guid.NewGuid();
    }

    public Guid Id { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string EditedBy { get; set; }
    public DateTime EditedOn { get; set; }
}

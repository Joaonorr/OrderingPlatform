namespace OrderingPlatform.Domain.Products;

public class Category
{
    public int Guid { get; set; }
    public string MyProperty { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string EditedBy { get; set; }
    public DateTime EditedOn { get; set;}
}

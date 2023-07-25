namespace WebThree.Shared.Data
{
    public interface IDataObject
    {
        Guid ID { get; set; }
        Guid CreatedBy { get; set; }
        DateTime CreatedDate { get; set; }
        Guid? UpdatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        bool Deleted { get; set; }
    }
}

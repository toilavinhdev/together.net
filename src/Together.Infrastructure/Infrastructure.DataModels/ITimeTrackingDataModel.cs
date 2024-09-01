namespace Infrastructure.DataModels;

public interface ITimeTrackingDataModel<TKey> : IDataModel<TKey>
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset ModifiedAt { get; set; }

    void MarkCreated();

    void MarkModified();
}
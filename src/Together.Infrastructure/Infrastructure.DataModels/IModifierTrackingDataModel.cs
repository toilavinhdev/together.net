namespace Infrastructure.DataModels;

public interface IModifierTrackingDataModel<TKey, TModifierKey> : ITimeTrackingDataModel<TKey>
{
    TModifierKey CreatedById { get; set; }
    
    TModifierKey ModifiedById { get; set; }

    void MarkUserCreated(TModifierKey createdById);

    void MarkUserModified(TModifierKey modifiedById);
}
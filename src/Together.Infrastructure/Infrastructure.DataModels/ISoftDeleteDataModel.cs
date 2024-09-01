namespace Infrastructure.DataModels;

public interface ISoftDeleteDataModel<TModifierKey>
{
    TModifierKey? DeletedById { get; set; }
    
    DateTimeOffset? DeletedAt { get; set; }
}
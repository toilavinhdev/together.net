using Infrastructure.DataModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Infrastructure.MongoDB;

public abstract class BaseDocument : IDataModel<string>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
}

public abstract class TimeTrackingDocument : BaseDocument, ITimeTrackingDataModel<string>
{
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset CreatedAt { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public DateTimeOffset ModifiedAt { get; set; }
    
    public void MarkCreated()
    {
        CreatedAt = DateTimeOffset.UtcNow;
        ModifiedAt = DateTimeOffset.UtcNow;
    }

    public void MarkModified()
    {
        ModifiedAt = DateTimeOffset.UtcNow;
    }
}

public abstract class ModifierTrackingDocument : TimeTrackingDocument, IModifierTrackingDataModel<string, Guid>
{
    public Guid CreatedById { get; set; }

    public Guid ModifiedById { get; set; }

    public void MarkUserCreated(Guid createdById)
    {
        MarkCreated();
        CreatedById = createdById;
        ModifiedById = createdById;
    }

    public void MarkUserModified(Guid modifiedById)
    {
        MarkModified();
        ModifiedById = modifiedById;
    }
}
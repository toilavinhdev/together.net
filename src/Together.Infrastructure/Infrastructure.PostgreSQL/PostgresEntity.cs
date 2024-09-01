using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.DataModels;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.PostgreSQL;

[PrimaryKey(nameof(Id))]
[Index(nameof(SubId), IsUnique = true)]
public abstract class BaseEntity : IDataModel<Guid>
{
    public Guid Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long SubId { get; set; }
}

public abstract class TimeTrackingEntity : BaseEntity, ITimeTrackingDataModel<Guid>
{
    public DateTimeOffset CreatedAt { get; set; }
    
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

public abstract class ModifierTrackingEntity : TimeTrackingEntity, IModifierTrackingDataModel<Guid, Guid>
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
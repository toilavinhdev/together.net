namespace Infrastructure.SharedKernel.BusinessObjects;

public class GeneralUser
{
    public Guid Id { get; set; }
    
    public long SubId { get; set; }

    public string UserName { get; set; } = default!;

    public string? Avatar { get; set; }
    
    public bool IsOfficial { get; set; }
}
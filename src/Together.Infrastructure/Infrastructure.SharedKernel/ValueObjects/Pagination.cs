using FluentValidation;

namespace Infrastructure.SharedKernel.ValueObjects;

public interface IPaginationRequest
{
    public int PageIndex { get; set; }
    
    public int PageSize { get; set; }
}

public sealed class PaginationValidator : AbstractValidator<IPaginationRequest>
{
    public PaginationValidator()
    {
        RuleFor(x => x.PageIndex)
            .NotNull()
            .GreaterThan(0);
        
        RuleFor(x => x.PageSize)
            .NotNull()
            .GreaterThan(0);
    }
}

public class Pagination(int pageIndex, int pageSize, long totalRecord)
{
    public int PageIndex { get; set; } = pageIndex;

    public int PageSize { get; set; } = pageSize;

    public long TotalRecord { get; set; } = totalRecord;

    public long TotalPage => (long)Math.Ceiling(TotalRecord / (double)PageSize);

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPage;
}

public class PaginationResult<T>
{
    public Pagination Pagination { get; set; } = default!;

    public List<T> Result { get; set; } = default!;
}
using System.Linq.Expressions;

namespace Infrastructure.SharedKernel.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> input)
    {
        return input.Select((item, index) => (item, index));
    }
    
    public static IQueryable<T> Paging<T>(this IQueryable<T> query, int pageIndex, int pageSize)
    {
        return query.Skip(pageSize * (pageIndex - 1)).Take(pageSize);
    }

    public static IQueryable<T> Sort<T>(this IQueryable<T> query, Expression<Func<T, object>> sortExpression, bool asc = true)
    {
        return asc ? query.OrderBy(sortExpression) : query.OrderByDescending(sortExpression);
    }

    public static string GetFieldName<TClass, TField>(this Expression<Func<TClass, TField>> expression)
    {
        var memberExpression = (MemberExpression) expression.Body;
        return memberExpression.Member.Name;
    }

    public static (string fieldName, TField value) GetFieldNameAndValue<TClass, TField>(this Expression<Func<TClass, TField>> expression)
    {
        var binaryExpression = (BinaryExpression) expression.Body;
        var left = (MemberExpression) binaryExpression.Left;
        var fieldName = left.Member.Name;
        var right = (ConstantExpression) binaryExpression.Right;
        var fieldValue = (TField) right.Value!;
        return (fieldName, fieldValue);
    }
}
namespace Infrastructure.DataModels;

public interface IDataModel<TKey>
{
    TKey Id { get; set; }
}
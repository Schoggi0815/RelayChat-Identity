namespace RelayChat_Identity.Models.Dtos;

public interface IPaginationDto<out T>
{
    public int Offset { get; set; }
    public int Take { get; set; }
    public IEnumerable<T> Items { get; }
    public bool HasMore { get; set; }
}

public class PaginationDto<T> : IPaginationDto<T>
{
    public int Offset { get; set; }
    public int Take { get; set; }
    public required IEnumerable<T> Items { get; set; }
    public bool HasMore { get; set; }
}

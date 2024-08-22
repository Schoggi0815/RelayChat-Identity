namespace RelayChat_Identity.Models.Dtos;

public interface IDtoAble<out T>
{
    public T ToDto();
}

public static class DtoAbleExtensions
{
    public static IEnumerable<TDto> ToDtos<TDto>(this IEnumerable<IDtoAble<TDto>> models)
    {
        return models.Select(model => model.ToDto());
    }
}

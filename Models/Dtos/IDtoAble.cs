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
    
    public static IPaginationDto<TDto> ToDtos<TDto>(this IPaginationDto<IDtoAble<TDto>> paginationDto)
    {
        return new PaginationDto<TDto>
        {
            Offset = paginationDto.Offset,
            Take = paginationDto.Take,
            Items = paginationDto.Items.ToDtos().ToList(),
            HasMore = paginationDto.HasMore,
        };
    }
}

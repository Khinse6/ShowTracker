using Microsoft.EntityFrameworkCore;
using ShowTracker.Api.Dtos;

namespace ShowTracker.Api.Mappings;

/// <summary>
/// Provides extension methods for creating paginated responses from IQueryable sources.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Asynchronously creates a paginated response DTO from an IQueryable source.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity in the query.</typeparam>
    /// <typeparam name="TDto">The type of the data transfer object.</typeparam>
    /// <typeparam name="TSortBy">The enum type for sorting options.</typeparam>
    /// <param name="query">The IQueryable source.</param>
    /// <param name="parameters">The query parameters for pagination.</param>
    /// <param name="mapper">A function to map a list of entities to a list of DTOs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated response DTO.</returns>
    public static async Task<PaginatedResponseDto<TDto>> ToPaginatedDtoAsync<TEntity, TDto, TSortBy>(
        this IQueryable<TEntity> query,
        QueryParameters<TSortBy> parameters,
        Func<List<TEntity>, List<TDto>> mapper) where TSortBy : Enum
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((parameters.Page - 1) * parameters.PageSize).Take(parameters.PageSize).ToListAsync();

        return new PaginatedResponseDto<TDto>
        {
            Items = mapper(items),
            PageNumber = parameters.Page,
            PageSize = parameters.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
        };
    }

    /// <summary>
    /// Asynchronously creates a paginated response DTO containing all items from an IQueryable source, intended for file exports.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity in the query.</typeparam>
    /// <typeparam name="TDto">The type of the data transfer object.</typeparam>
    /// <param name="query">The IQueryable source.</param>
    /// <param name="mapper">A function to map a list of entities to a list of DTOs.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a paginated response DTO with all items on a single page.</returns>
    public static async Task<PaginatedResponseDto<TDto>> ToExportResponseAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        Func<List<TEntity>, List<TDto>> mapper)
    {
        var allItems = await query.ToListAsync();
        var allDtos = mapper(allItems);
        return new PaginatedResponseDto<TDto>
        {
            Items = allDtos,
            PageNumber = 1,
            PageSize = allDtos.Any() ? allDtos.Count : 1, // Avoid PageSize of 0
            TotalCount = allDtos.Count,
            TotalPages = 1
        };
    }
}

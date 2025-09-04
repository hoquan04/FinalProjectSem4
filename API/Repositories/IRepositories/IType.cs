using API.Repositories.RestAPI;
namespace API.Repositories.IRepositories
{
    public interface IType<T>
    {
        Task<APIRespone<IEnumerable<T>>> GetAllAsync();
        Task<APIRespone<T>> GetByIdAsync(int id); 
        Task<APIRespone<T>> AddAsync(T entity);
        Task<APIRespone<T>> UpdateAsync(int id, T entity); 
        Task<APIRespone<bool>> DeleteAsync(int id); 
        Task<APIRespone<PagedResponse<T>>> GetPageAsync(int pageNow, int pageSize);
    }
}

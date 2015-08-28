using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public interface IRepository<T>
    {
        IQueryable<T> All { get; }

        int Add(T item);
        Task<int> AddAsync(T item);
        
        int Update(T item);
        Task<int> UpdateAsync(T item);

        T Get(int id);

        int Delete(int id);
        Task<int> DeleteAsync(T item);
    }
}

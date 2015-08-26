using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public interface IRepository<T>
    {
        IQueryable<T> All { get; }

        Task<int> AddAsync(T item);

        Task<int> UpdateAsync(T item);

        T GetById(int id);

        Task<int> DeleteAsync(int id);
    }
}

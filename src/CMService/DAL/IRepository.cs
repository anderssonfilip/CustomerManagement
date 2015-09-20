using Neo4jClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public enum Persistence { SQL, Graph}

    public interface IRepository<T>
    {
        Persistence Persistence { get; }

        IQueryable<T> All { get; }

        int Add(T item);
        Task<int> AddAsync(T item);

        T Get(int id);

        int Delete(int id);
        Task<int> DeleteAsync(T item);

        int Update(T item);
        Task<int> UpdateAsync(T item);

        // Only for Graph

        IGraphClient GraphClient { get; }

        IEnumerable<object> MatchCategories { get; }

        IEnumerable<object> MatchLocations { get; }
    }
}

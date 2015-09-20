using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMService.DAL
{
    public interface IGraph<T>
    {
        IEnumerable<T> MatchCategories { get; }

    }
}

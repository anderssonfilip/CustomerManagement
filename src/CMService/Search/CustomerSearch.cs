using System.Collections.Generic;
using System.Linq;

namespace CMService.Search
{
    public class CustomerSearch
    {
        struct Match
        {
            public string key;
            public int score;
        }

        private readonly IDistanceAlgorithm _algorithm;

        public CustomerSearch(IDistanceAlgorithm searchAlgorithm)
        {
            _algorithm = searchAlgorithm;
        }

        /// <summary>
        /// Find the 'n' closest matches to 'value' in 'keys'
        /// </summary>
        /// <param name="key">the search key</param>
        /// <param name="values">the search possibilities</param>
        /// <param name="n">number of results to return</param>
        /// <returns>ordered matches from keys</returns>
        public IEnumerable<string> FindClosestMatches(string key, IEnumerable<string> values, uint n)
        {
            if (n == 0)
            {
                return new List<string>();
            }

            return values.Select(k => new Match { key = k, score = _algorithm.Compute(key, k) })
                       .Where(m => m.score >= 0)  // only return positive scores as a negative score indicates no match in certain algoritms
                       .OrderBy(i => i.score)
                       .Select(i => i.key)
                       .Take((int)n);
        }
    }
}

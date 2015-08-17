using System.Collections.Generic;

namespace CMClient.Models
{
    public class Search
    {
        public Search()
        {
            Result = new List<KeyValuePair<int, string>>();
        }

        public string Query { get; set; }

        public IList<KeyValuePair<int, string>> Result { get; set; }

        public string ServiceURI { get; set; }

    }
}

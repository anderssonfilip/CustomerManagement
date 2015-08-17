namespace CMService.Search
{
    public class HammingDistance : IDistanceAlgorithm
    {
        public int Compute(string s, string t)
        {
            if (s.Length != t.Length)
                return -1;

            int distance = 0;
            for (int i = 0, length = s.Length; i < length; i++)
            {
                if (s[i] != t[i])
                    distance++;
            }

            return distance;
        }
    }
}
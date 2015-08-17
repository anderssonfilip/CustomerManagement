namespace CMService.Search
{
    /// <summary>
    /// A distance algorithm that satiesfies the criteria that more similar string return a lower integer
    /// </summary>
    public interface IDistanceAlgorithm
    {
        int Compute(string s, string t);
    }
}

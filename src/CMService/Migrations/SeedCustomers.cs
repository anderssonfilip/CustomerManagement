using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CMService.Migrations
{
    public class SeedCustomers
    {
        internal static void Seed(string connectionString)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://docs.google.com/spreadsheets/d/1UZQLqByd8AZR3wM5Cb0qmX_0eScrwfwee3iAONk3O5A/export?format=csv");
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand("DELETE FROM CustomerUpdate", connection);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("DELETE FROM Customer", connection);
                    cmd.ExecuteNonQuery();

                    var line = sr.ReadLine();

                    while (line != null)
                    {
                        line = sr.ReadLine(); // skip the header line

                        if (line == null)
                            break;

                        var csvRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                        var fields = csvRegex.Split(line);

                        cmd = new SqlCommand("INSERT INTO Customer values (@name, @gender, @house_number, @address_line_1, @state, @country, @category, @dob)", connection);
                        cmd.Parameters.AddWithValue("name", fields[0].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("gender", fields[1].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("house_number", fields[2].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("address_line_1", fields[3].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("state", fields[4].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("country", fields[5].Trim(' ', '"'));
                        cmd.Parameters.AddWithValue("category", fields[6].Trim(' ', '"').Split(' ')[1]); // remove Category prefix
                        cmd.Parameters.AddWithValue("dob", fields[7].Trim(' ', '"'));

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}

using CMService.DAL;
using System.Linq;
using System.Net;

namespace CMService.Test
{
    public class CustomerControllerTest
    {
        private readonly string _connectionString;
        private readonly string _serviceURI;

        public CustomerControllerTest(string connectionString, string serviceURI)
        {
            _connectionString = connectionString;
            _serviceURI = serviceURI;
        }

        /// <summary>
        /// Test delete of customer by making a DELETE request
        /// </summary>
        public void TestDelete()
        {
            using (var customerDbContext = new CustomerDbContext(_connectionString))
            {
                var id = customerDbContext.Customers.First().Id;

                var request = (HttpWebRequest)WebRequest.Create(_serviceURI + id);
                request.Method = "DELETE";

                using (WebResponse response = request.GetResponse())
                {

                }
            }
        }

        /// <summary>
        /// Test add customer by making a POST request
        /// </summary>
        public void TestPost()
        {

        }

        /// <summary>
        /// Test update customer by making a PUT request
        /// </summary>
        public void TestPut()
        {

        }
    }
}

using System;

namespace CMService.Test
{
    public class Program
    {
        public void Main(string[] args)
        {
            var testRunner = new CustomerControllerTest(
                "Server = localhost, 1433; Database = CM; User ID = WebsiteLaps; Password = WebsiteLaps",
                "http://localhost:52988/"
            );

            testRunner.TestPost();

            Console.WriteLine("All tests completed");
            Console.ReadLine();
        }
    }
}

using System;

namespace CMService.Test
{
    public class Program
    {
        public void Main(string[] args)
        {
            var customerControllerTest = new CustomerControllerTest(
                "Data Source=localhost;Integrated Security=True;Connect Timeout=15;Database=cm;",
                "http://localhost:52988/api/"
            );

            Console.WriteLine("CustomerControllerTest.Get() {0}",
                customerControllerTest.Get() ? "succeeded" : "failed");

            Console.WriteLine("CustomerControllerTest.Search() {0}", 
                customerControllerTest.Search() ? "succeeded" : "failed");

            Console.WriteLine("CustomerControllerTest.Post() {0}",
                customerControllerTest.Post() ? "succeeded" : "failed");

            Console.WriteLine("CustomerControllerTest.Put() {0}",
                customerControllerTest.Put() ? "succeeded" : "failed");

            Console.WriteLine("CustomerControllerTest.Delete() {0}",
                customerControllerTest.Delete() ? "succeeded" : "failed");

            Console.WriteLine("All tests completed");
            Console.ReadLine();
        }
    }
}

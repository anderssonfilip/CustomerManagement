using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace CMClient.Test
{
    public class CustomerSearchTest
    {
        [Fact]
        public void Search()
        {
            var driverGC = new ChromeDriver(@"C:\Selenium\");

            driverGC.Navigate().GoToUrl("http://localhost:53074/");
            var form = driverGC.FindElement(By.Id("searchForm"));
            Assert.NotNull(form);
        }
    }
}

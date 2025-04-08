using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.MemoQCMS.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.MemoQCMS.Base;

namespace Tests.MemoQCMS
{
    [TestClass]
    public class DataSourceTests : TestBase
    {
        [TestMethod]
        public async Task GetTargetLanguages_ReturnsValue()
        {
            var dataSourceHandler = new TargetLanguageDataSourceHandler(InvocationContext);
            var context = new DataSourceContext{ SearchString = ""};
            var data = await dataSourceHandler.GetDataAsync(context, CancellationToken.None);

            foreach (var item in data)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
                Assert.IsTrue(data.Count > 0);
            }
           
        }

        [TestMethod]
        public async Task GetOrderHandler_ReturnsValue()
        {
            var dataSourceHandler = new OrderDataSourceHandler(InvocationContext);
            var context = new DataSourceContext { SearchString = "" };
            var data = await dataSourceHandler.GetDataAsync(context, CancellationToken.None);

            foreach (var item in data)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
                Assert.IsTrue(data.Count > 0);
            }

        }
    }
}

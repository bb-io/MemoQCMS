using Apps.MemoQCMS.Actions;
using Apps.MemoQCMS.Models.Identifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.MemoQCMS.Base;

namespace Tests.MemoQCMS
{
    [TestClass]
    public class OrderTests :TestBase
    {
        [TestMethod]
        public async Task GetOrder_IsSuccess()
        {
            var action = new OrderActions(InvocationContext);

            var order = await action.GetOrder(new OrderIdentifier
            {
                OrderId = "250326-0XL"
            });

            PropertyInfo[] properties = order.GetType().GetProperties();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(order);
                Console.WriteLine($"{prop.Name}: {value}");
            }

            Assert.IsNotNull(order);
        }
    }
}

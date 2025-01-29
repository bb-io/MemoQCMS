using Apps.MemoQCMS.Connections;
using Blackbird.Applications.Sdk.Common.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.MemoQCMS.Base;

namespace Tests.MemoQCMS
{
    [TestClass]
    public class ValidatorTests : TestBase
    {
        [TestMethod]
        public async Task ValidatesCorrectConnection() // This works if you include the "/memoqservercmsgateway/v1" or not, however other tests fail without url building, weird.
        {
            var validator = new ConnectionValidator();

            var result = await validator.ValidateConnection(Creds, CancellationToken.None);
            Console.WriteLine(result.Message);
            Assert.IsTrue(result.IsValid);
        }

        [TestMethod]
        public async Task DoesNotValidateIncorrectConnection()
        {
            var validator = new ConnectionValidator();

            var newCreds = Creds.Select(x => new AuthenticationCredentialsProvider(x.KeyName, x.Value + "_incorrect"));
            var result = await validator.ValidateConnection(newCreds, CancellationToken.None);
            Console.WriteLine(result.Message);
            Assert.IsFalse(result.IsValid);
        }
    }
}

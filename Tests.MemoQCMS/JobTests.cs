using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.MemoQCMS.Base;
using Apps.MemoQCMS.Actions;
using Apps.MemoQCMS.Models.Identifiers;
using Apps.MemoQCMS.Models.Requests.Jobs;

namespace Tests.MemoQCMS
{
    [TestClass]
    public class JobTests:TestBase
    {
        [TestMethod]
        public async Task ListJob_OrderIdExists_Works()
        {
            var actions = new JobActions(InvocationContext, FileManager);

            var orderidentifier = new OrderIdentifier() { OrderId = "241204-FVY" };

            var result = await actions.ListJobs(orderidentifier);
            Console.WriteLine(result.Jobs);
            Assert.IsNotNull(result.Jobs);
        }

        [TestMethod]
        public async Task CreateJob_TargetLanguageNotValid_MisconfigurationException()
        {
            var actions = new JobActions(InvocationContext, FileManager);

            var orderidentifier = new OrderIdentifier() { OrderId = "241204-FVY" };

            var input = new CreateJobRequest() { Name = "testname",SourceLanguage = "eng", TargetLanguage = "agsd" };
            var file = new Apps.MemoQCMS.Models.FileWrapper() { File = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "testfile.txt", ContentType = "text/plain" } };

            await Throws.MisconfigurationException(() => actions.CreateJob(orderidentifier,input, file));

        }

        [TestMethod]
        public async Task CreateJob_TargetLanguageNull_MisconfigurationException()
        {
            var actions = new JobActions(InvocationContext, FileManager);

            var orderidentifier = new OrderIdentifier() { OrderId = "241204-FVY" };

            var input = new CreateJobRequest() { Name = "testname", SourceLanguage = "eng", TargetLanguage = null };
            var file = new Apps.MemoQCMS.Models.FileWrapper() { File = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "testfile.txt", ContentType = "text/plain" } };

            await Throws.MisconfigurationException(() => actions.CreateJob(orderidentifier, input, file));

        }
    }
}

﻿using System;
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

            var orderidentifier = new OrderIdentifier() { OrderId = "250326-0XL" };

            var result = await actions.ListJobs(orderidentifier);
            Assert.IsNotNull(result.Jobs);

            foreach (var job in result.Jobs)
            {
                Console.WriteLine($"Job ID: {job.TranslationJobId}");
                Console.WriteLine($"Name: {job.Name}");
                Console.WriteLine($"Url: {job.Url}");
                Console.WriteLine($"Source language: {job.SourceLang}");
                Console.WriteLine($"Target language: {job.TargetLang}");
                Console.WriteLine($"File type: {job.FileType}");
                Console.WriteLine($"Status: {job.Status}");
                Console.WriteLine($"Order ID: {job.OrderId}");
                Console.WriteLine(new string('-', 50));
            }
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


        [TestMethod]
        public async Task CreateJob_IsSuccess()
        {
            var actions = new JobActions(InvocationContext, FileManager);

            var orderidentifier = new OrderIdentifier() { OrderId = "250506-0O3" };

            var input = new CreateJobRequest() { Name = "/real-rivian-adventures", SourceLanguage = "eng", TargetLanguage = "ukr" };
            var file = new Apps.MemoQCMS.Models.FileWrapper() { File = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "/testfile.txt", ContentType = "text/plain" } };

           var response = await actions.CreateJob(orderidentifier, input, file);

            Console.WriteLine($"Job ID: {response.TranslationJobId}");
            Assert.IsNotNull(response);

        }
    }
}

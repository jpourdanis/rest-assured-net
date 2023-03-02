// <copyright file="MultiPartFormDataTests.cs" company="On Test Automation">
// Copyright 2019 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
namespace RestAssured.Tests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using RestAssured.Request.Exceptions;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class MultiPartFormDataTests : TestBase
    {
        private readonly string fileName = @"ToDoItems.txt";

        private readonly string todoItem = "Watch Office Space";

        /// <summary>
        /// Creates the file to be uploaded in these tests.
        /// </summary>
        /// <returns>The asynchronous test result.</returns>
        [SetUp]
        public async Task CreateFileToUpload()
        {
            await File.WriteAllLinesAsync(this.fileName, new string[] { this.todoItem });
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multipart form data with the default 'file' control name
        /// in the request.
        /// </summary>
        [Test]
        public void MultiPartFormDataWithDefaultControlNameCanBeSupplied()
        {
            this.CreateStubForSimpleMultiPartFormData();

            Given()
                .MultiPart(this.fileName)
                .When()
                .Post("http://localhost:9876/simple-multipart-form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for including
        /// multipart form data with a custom control name in the request.
        /// </summary>
        [Test]
        public void MultiPartFormDataWithCustomControlNameCanBeSupplied()
        {
            this.CreateStubForSimpleMultiPartFormData();

            Given()
                .MultiPart("customControl", this.fileName)
                .When()
                .Post("http://localhost:9876/simple-multipart-form-data")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for verifying
        /// that trying to upload a nonexistent file throws the expected
        /// exception.
        /// </summary>
        [Test]
        public void UploadingNonExistentFileThrowsTheExpectedException()
        {
            this.CreateStubForSimpleMultiPartFormData();

            var rce = Assert.Throws<RequestCreationException>(() =>
            {
                Given()
                .MultiPart("customControl", @"DoesNotExist.txt")
                .When()
                .Post("http://localhost:9876/simple-multipart-form-data")
                .Then()
                .StatusCode(201);
            });

            Assert.That(rce?.Message, Does.Contain("Could not find file"));
        }

        /// <summary>
        /// Deletes the file created for test execution.
        /// </summary>
        [TearDown]
        public void DeleteFile()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            File.Delete(this.fileName);
        }

        /// <summary>
        /// Creates the stub response for the form data example.
        /// </summary>
        private void CreateStubForSimpleMultiPartFormData()
        {
            this.Server?.Given(Request.Create().WithPath("/simple-multipart-form-data").UsingPost()
                .WithHeader("Content-Type", new RegexMatcher("multipart/form-data; boundary=.*"))
                .WithBody(new RegexMatcher($".*{this.todoItem}.*")))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}
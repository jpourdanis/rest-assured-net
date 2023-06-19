// <copyright file="CustomJsonSerializerSettingsTests.cs" company="On Test Automation">
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
    using Newtonsoft.Json;
    using NUnit.Framework;
    using RestAssured.Request.Builders;
    using WireMock.Matchers;
    using WireMock.RequestBuilders;
    using WireMock.ResponseBuilders;
    using static RestAssured.Dsl;

    /// <summary>
    /// Examples of RestAssuredNet usage.
    /// </summary>
    [TestFixture]
    public class CustomJsonSerializerSettingsTests : TestBase
    {
        private readonly string expectedSerializedObjectWithCustomSettings = "{\"Id\":1,\"Title\":\"My post title\",\"Date\":\"1999-12-31\"}";

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// an anonymous type to JSON using custom JsonSerializerSettings
        /// specified in the test body.
        /// </summary>
        [Test]
        public void CustomJsonSerializerSettingsCanBeSuppliedInTestBody()
        {
            this.CreateStubForObjectSerializationWithCustomSettings();

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.DateFormatString = "yyyy-MM-dd";

            var post = new
            {
                Id = 1,
                Title = "My post title",
                Date = new DateTime(1999, 12, 31, 23, 59, 59, 0),
            };

            Given()
                .JsonSerializerSettings(jsonSerializerSettings)
                .Body(post)
                .When()
                .Post("http://localhost:9876/object-serialization-custom-settings")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// A test demonstrating RestAssuredNet syntax for serializing
        /// an anonymous type to JSON using custom JsonSerializerSettings
        /// specified as part of a RequestSpecification.
        /// </summary>
        [Test]
        public void CustomJsonSerializerSettingsCanBeSuppliedInRequestSpecification()
        {
            this.CreateStubForObjectSerializationWithCustomSettings();

            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.DateFormatString = "yyyy-MM-dd";

            RequestSpecification requestSpecification = new RequestSpecBuilder()
                .WithJsonSerializerSettings(jsonSerializerSettings)
                .Build();

            var post = new
            {
                Id = 1,
                Title = "My post title",
                Date = new DateTime(1999, 12, 31, 23, 59, 59, 0),
            };

            Given()
                .Spec(requestSpecification)
                .Body(post)
                .When()
                .Post("http://localhost:9876/object-serialization-custom-settings")
                .Then()
                .StatusCode(201);
        }

        /// <summary>
        /// Creates the stub response for the object serialization example using custom JsonSerializerSettings.
        /// </summary>
        private void CreateStubForObjectSerializationWithCustomSettings()
        {
            this.Server?.Given(Request.Create().WithPath("/object-serialization-custom-settings").UsingPost()
                .WithBody(new JsonMatcher(this.expectedSerializedObjectWithCustomSettings)))
                .RespondWith(Response.Create()
                .WithStatusCode(201));
        }
    }
}
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using FluentAssertions;

namespace WebApplication1.Tests
{

    public class IntegrationTests
    {
        HttpClient httpClient;

        //Ideally integration tests should be run with a test database
        public IntegrationTests()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            httpClient = webAppFactory.CreateDefaultClient();
        }


        //Ideally I shouldn't rely on existing database entries and should also try creating my own etc.
        [Fact]
        public async Task Diff_MissingContent()
        {
            var response = await httpClient.GetAsync("v1/diff/1");
            var result = await response.Content.ReadAsStringAsync();

            result.Should().BeOfType<string>();
            result.Should().Contain("\"status\":404");

        }

        [Fact]
        public async Task Diff_SizeDoNotMatch()
        {
            var response = await httpClient.GetAsync("v1/diff/112");
            var result = await response.Content.ReadAsStringAsync();

            result.Should().BeOfType<string>();
            result.Should().Contain("\"diffResultType\":\"SizeDoNotMatch\"");

        }

    }
}

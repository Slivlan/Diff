using WebApplication1.Controllers;
using WebApplication1.Interfaces;
using FluentAssertions;
using FakeItEasy;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApplication1.Tests
{
    public class UnitTests
    {
        private DiffController dc;
        private IDiffRepository dr;

        public UnitTests()
        {
            dr = A.Fake<IDiffRepository>();
            dc = new DiffController(dr);
        }

        [Fact]
        public void GetDiff_TestEquals()
        {
            var diffMock = new Diff(1);
            diffMock.Left = new byte[]{ 0, 0, 0, 0};
            diffMock.Right = new byte[] { 0, 0, 0, 0 };
            A.CallTo(() => dr.GetDiff(1)).Returns(diffMock);

            var res = dc.GetDiff(1);

            res.Should().BeOfType<OkObjectResult>();
            OkObjectResult resOk = res.As<OkObjectResult>();
            resOk.Value.Should().BeOfType<JsonResult>();
            JsonResult jr = resOk.Value as JsonResult;
            string json = JsonConvert.SerializeObject(jr.Value);
            DiffResultType? jsonData = JsonConvert.DeserializeObject<DiffResultType>(json);
            jsonData.diffResultType.Should().Be("Equals");

        }

        [Fact]
        public void GetDiff_TestSizeDoNotMatch() 
        {
            var diffMock = new Diff(1);
            diffMock.Left = new byte[] { 0, 0, 0, 0 };
            diffMock.Right = new byte[] { 0, 0, 0 };
            A.CallTo(() => dr.GetDiff(1)).Returns(diffMock);

            var res = dc.GetDiff(1);

            res.Should().BeOfType<OkObjectResult>();
            OkObjectResult resOk = res.As<OkObjectResult>();
            resOk.Value.Should().BeOfType<JsonResult>();
            JsonResult jr = resOk.Value as JsonResult;
            string json = JsonConvert.SerializeObject(jr.Value);
            DiffResultType? jsonData = JsonConvert.DeserializeObject<DiffResultType>(json);
            jsonData.diffResultType.Should().Be("SizeDoNotMatch");
        }

        [Fact]
        public void GetDiff_TestDifferent1()
        {
            var diffMock = new Diff(1);
            diffMock.Left = new byte[] { 0, 0, 0, 0 };
            diffMock.Right = new byte[] { 0, 0, 0, 1};
            A.CallTo(() => dr.GetDiff(1)).Returns(diffMock);

            var res = dc.GetDiff(1);

            res.Should().BeOfType<OkObjectResult>();
            OkObjectResult resOk = res.As<OkObjectResult>();
            resOk.Value.Should().BeOfType<JsonResult>();
            JsonResult jr = resOk.Value as JsonResult;
            string json = JsonConvert.SerializeObject(jr.Value);
            DiffResultTypeDiffs? jsonData = JsonConvert.DeserializeObject<DiffResultTypeDiffs>(json);
            jsonData.diffResultType.Should().Be("ContentDoNotMatch");
            jsonData.diffs.Should().HaveCount(1);
            jsonData.diffs[0].offset.Should().Be(3);
            jsonData.diffs[0].length.Should().Be(1);

        }

        [Fact]
        public void GetDiff_TestDifferent2()
        {
            var diffMock = new Diff(1);
            diffMock.Left = new byte[] { 1, 0, 1, 0 };
            diffMock.Right = new byte[] { 0, 0, 0, 1 };
            A.CallTo(() => dr.GetDiff(1)).Returns(diffMock);

            var res = dc.GetDiff(1);

            res.Should().BeOfType<OkObjectResult>();
            OkObjectResult resOk = res.As<OkObjectResult>();
            resOk.Value.Should().BeOfType<JsonResult>();
            JsonResult jr = resOk.Value as JsonResult;
            string json = JsonConvert.SerializeObject(jr.Value);
            DiffResultTypeDiffs? jsonData = JsonConvert.DeserializeObject<DiffResultTypeDiffs>(json);
            jsonData.diffResultType.Should().Be("ContentDoNotMatch");
            jsonData.diffs.Should().HaveCount(2);
            jsonData.diffs[0].offset.Should().Be(0);
            jsonData.diffs[0].length.Should().Be(1);
            jsonData.diffs[1].offset.Should().Be(2);
            jsonData.diffs[1].length.Should().Be(2);
        }

        [Fact]
        public void GetDiff_TestNoEntryInDatabase()
        {
            A.CallTo(() => dr.GetDiff(1)).Returns(null);

            var res = dc.GetDiff(1);

            res.Should().BeOfType<NotFoundResult>();
        }
    }

    //Classes neccessary for deserialization of Json objects
    public class DiffResultType
    {
        public string diffResultType;
    }

    public class DiffResultTypeDiffs
    {
        public string diffResultType;
        public OffsetLengthPair[] diffs;
    }
}
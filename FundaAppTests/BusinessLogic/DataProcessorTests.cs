using System;
using System.Collections.Generic;
using FundaApp.BusinessLogic;
using FundaApp.DataAccess;
using Moq;
using Xunit;

namespace FundaAppTests.BusinessLogic
{
    /// <summary>
    /// Contains DataProcessor tests
    /// </summary>
    public class DataProcessorTests
    {
        private Mock<IFundaServiceAgent> fundaServiceAgentMock;

        public DataProcessorTests()
        {
            fundaServiceAgentMock = new Mock<IFundaServiceAgent>();
        }

        [Fact]
        public void GetTop10Makelaars_OK()
        {
            // Setup
            List<string> jsonContentPages = new List<string>();
            jsonContentPages.Add(ResourceTestData.JsonResponseOK);
            jsonContentPages.Add(ResourceTestData.JsonResponseOK);
            jsonContentPages.Add(ResourceTestData.JsonResponseOK);
            jsonContentPages.Add(ResourceTestData.JsonResponseOK);
            jsonContentPages.Add(ResourceTestData.JsonResponseOK);

            fundaServiceAgentMock.Setup(x => x.GetSearchResultsPages(It.IsAny<string>())).Returns(jsonContentPages);
            var dataProcessor = new DataProcessor(fundaServiceAgentMock.Object);

            //// Execute
            var results = dataProcessor.GetTop10Makelaars("Amsterdam");

            //// Test
            Assert.Equal(2, results.Count);
            Assert.Equal("11 Makelaars", results[0].Name);
            Assert.Equal("Admiraal makelaardij", results[1].Name);
            Assert.Equal(5, results[0].Count);
            Assert.Equal(5, results[1].Count);
        }

        [Fact]
        public void GetTop10MakelaarsNoPagesFound()
        {
            // Setup
            List<string> jsonContentPages = new List<string>();

            fundaServiceAgentMock.Setup(x => x.GetSearchResultsPages(It.IsAny<string>())).Returns(jsonContentPages);
            var dataProcessor = new DataProcessor(fundaServiceAgentMock.Object);

            //// Execute and Test
            var results = dataProcessor.GetTop10Makelaars("Amsterdam");
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public void GetTop10Makelaars_Exception_InvalidJSON()
        {
            // Setup
            List<string> jsonContentPages = new List<string>();
            jsonContentPages.Add("{{{{invalidJSON Structure}");
            jsonContentPages.Add("abc*&^*&^&dsdsd*^*&^*&^*&^*&^*&^*&");
            jsonContentPages.Add("<a>dsdsds</sdsd>");

            fundaServiceAgentMock.Setup(x => x.GetSearchResultsPages(It.IsAny<string>())).Returns(jsonContentPages);
            var dataProcessor = new DataProcessor(fundaServiceAgentMock.Object);

            //// Execute and Test
            var exception = Assert.Throws<ApplicationException>(() => dataProcessor.GetTop10Makelaars("Amsterdam"));
            Assert.Equal("Er is een fout opgetreden tijdens het parsen en coverteren van JSON data naar een XML structuur.", exception.Message);
        }



        [Fact]
        public void GetTop10Makelaars_Exception_EmptyPagesFound()
        {
            // Setup
            List<string> jsonContentPages = new List<string>();
            jsonContentPages.Add(string.Empty);
            jsonContentPages.Add(string.Empty);
            jsonContentPages.Add(string.Empty);
            
            fundaServiceAgentMock.Setup(x => x.GetSearchResultsPages(It.IsAny<string>())).Returns(jsonContentPages);
            var dataProcessor = new DataProcessor(fundaServiceAgentMock.Object);

            //// Execute and Test
            var exception = Assert.Throws<ApplicationException>(() => dataProcessor.GetTop10Makelaars("Amsterdam"));
            Assert.Equal("Er is een fout opgetreden tijdens het parsen en coverteren van JSON data naar een XML structuur.", exception.Message);
        }
    }
}

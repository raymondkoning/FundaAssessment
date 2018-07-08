using System;
using System.Threading.Tasks;
using FundaApp.DataAccess;
using Moq;
using Xunit;

namespace FundaAppTests.DataAccess
{
    /// <summary>
    /// Contains FundaServiceAgent tests
    /// </summary>
    public class FundaServiceAgentTests
    {
        private Mock<IHttpClientWrapper> httpClientMock;
        
        public FundaServiceAgentTests()
        {
            httpClientMock = new Mock<IHttpClientWrapper>();
        }

        [Fact]
        public void GetSearchResultsPages_OK()
        {
            // Setup
            httpClientMock.Setup(x => x.GetStringAsync(It.IsAny<Uri>())).Returns(Task.FromResult(ResourceTestData.JsonResponseOK));
            FundaServiceAgent fundaServiceAgent = new FundaServiceAgent(httpClientMock.Object);

            // Execute
            var contentPages = fundaServiceAgent.GetSearchResultsPages(@"/Amsterdam");

            // Test
            Assert.Equal(354, contentPages.Count);
        }

        [Fact]
        public void GetSearchResultsPages_Exception_NoPagingClass()
        {
            // Setup
            httpClientMock.Setup(x => x.GetStringAsync(It.IsAny<Uri>())).Returns(Task.FromResult(ResourceTestData.JsonResponseNoPagingClass));
            FundaServiceAgent fundaServiceAgent = new FundaServiceAgent(httpClientMock.Object);

            // Execute & test
            var exception = Assert.Throws<ApplicationException>(() => fundaServiceAgent.GetSearchResultsPages(@"/Amsterdam")); 
            Assert.Equal("Error while determining the number of pages to retrieve. Details : Paging Json class not found.", exception.Message);
        }

        [Fact]
        public void GetSearchResultsPages_Exception_ByHttpClientCall()
        {
            // Setup
            httpClientMock.Setup(x => x.GetStringAsync(It.IsAny<Uri>())).Throws(new Exception("Foutje!"));
            FundaServiceAgent fundaServiceAgent = new FundaServiceAgent(httpClientMock.Object);

            // Execute & test
            var exception = Assert.Throws<ApplicationException>(() => fundaServiceAgent.GetSearchResultsPages(@"/Amsterdam"));
            Assert.Equal("Er is iets fout gegaan tijdens de aanroep van de API met endpoint : 'http://partnerapi.funda.nl/feeds/Aanbod.svc/JSON/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/Amsterdam&page=1&pagesize=100'", exception.Message);
        }
        
        [Fact]
        public void GetSearchResultsPages_AgreggatedException_ByHttpClientCallInParallelForeachLoop()
        {
            // Setup
            // -- First call of 'GetStringAsync' --
            httpClientMock.Setup(x => x.GetStringAsync(new Uri("http://partnerapi.funda.nl/feeds/Aanbod.svc/JSON/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/Amsterdam&page=1&pagesize=100"))).Returns(Task.FromResult(ResourceTestData.JsonResponseOK));

            // -- Second call of 'GetStringAsync' from within the Parallel Foreach loop --
            httpClientMock.Setup(x => x.GetStringAsync(new Uri("http://partnerapi.funda.nl/feeds/Aanbod.svc/JSON/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo=/Amsterdam&page=2&pagesize=100"))).Throws(new Exception("Foutje!"));

            FundaServiceAgent fundaServiceAgent = new FundaServiceAgent(httpClientMock.Object);

            // Execute & test
            var exception = Assert.Throws<ApplicationException>(() => fundaServiceAgent.GetSearchResultsPages(@"/Amsterdam"));
            Assert.Equal("Er zijn meerdere fouten opgetreden tijdens de parallele aanroep van de funda API!", exception.Message);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundaApp.DataAccess
{
    /// <summary>
    /// This class is the client connecter to the remote funda API service. It contains 1 function specifcally used to query 'for sale'-objects. 
    /// It also contain functionality used to retieve all query results in one single method unsing the paging functionality provided by the funda API.
    /// </summary>
    public class FundaServiceAgent : IFundaServiceAgent
    {
        #region Fields

        private IHttpClientWrapper httpClient;
        private Object contentPagesLock = new Object();
        private const string CFundaApiUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/JSON/ac1b0b1572524640a0ecc54de453ea9f/?type=koop&zo={0}&page={1}&pagesize=100";
        private const string CPagingMatchString = "\"Paging\":{\"AantalPaginas\":";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="httpClient">The httpClient used to send the request</param>
        public FundaServiceAgent(IHttpClientWrapper httpClient)
        {
            this.httpClient = httpClient;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retrieving objects using a searchQuery. 
        /// </summary>
        /// <param name="searchQuery">The 'zo' search query</param>
        /// <returns>A list of JSON content pages in the form of a list of string </returns>
        public List<string> GetSearchResultsPages(string searchQuery)
        {

            var contentPages = new List<string>();

            // Do the first call to retieve the first page AND therefor the Paging info.
            var firstPageContent = this.GetSearchDataAsJson(string.Format(CFundaApiUrl, searchQuery, 1));
            contentPages.Add(firstPageContent);

            // Determine the number of pages of the resultset.
            int nrOfPages = DetermineNrOfPages(firstPageContent);
            var indexesToRetrieve = Enumerable.Range(2, nrOfPages - 1).ToList();

            // Retrieve the pages usinng 20 parallel threads
            try
            {
                Parallel.ForEach(indexesToRetrieve, new ParallelOptions { MaxDegreeOfParallelism = 20 },
                    (currentIndex) =>
                        {
                            // retrieve query result as a JSON string
                            var pageContent = this.GetSearchDataAsJson(string.Format(CFundaApiUrl, searchQuery, currentIndex));

                            // Lock critical section 
                            lock (contentPagesLock)
                            {
                                contentPages.Add(pageContent);
                            }
                        });
            }
            catch (AggregateException ex)
            {
                throw new ApplicationException("Er zijn meerdere fouten opgetreden tijdens de parallele aanroep van de funda API!", ex);
            }

            // Return the JSON content pages
            return contentPages;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Function that determines the number of total pages the results exitst of. 
        /// </summary>
        /// <param name="content">The page content in which the paging info is embedded.</param>
        /// <returns></returns>
        private int DetermineNrOfPages(string content)
        {
            try
            {
                int indexOfPagingPart = content.LastIndexOf(CPagingMatchString, StringComparison.Ordinal);

                if (indexOfPagingPart > 0)
                {
                    var pagingPart = content.Substring(indexOfPagingPart + CPagingMatchString.Length);
                    var nrOfPages = pagingPart.Split(new[] { ',' });
                    return int.Parse(nrOfPages.First());
                }
                throw new ApplicationException("Paging Json class not found.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error while determining the number of pages to retrieve. Details : {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Function that calls the async method 'GetStringAsync' which returns the http response body as string.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetSearchDataAsJson(string url)
        {
            try
            {
                return this.httpClient.GetStringAsync(new Uri(url)).Result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Er is iets fout gegaan tijdens de aanroep van de API met endpoint : '{url}'", ex);
            }
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using FundaApp.DataAccess;
using FundaApp.DomainEntities;

namespace FundaApp.BusinessLogic
{
    /// <summary>
    /// This class contains logic for querying', grouping and filtering the input data provided by the fundaServiceAgent'
    /// </summary>
    public class DataProcessor : IDataProcessor
    {
        #region Fields

        private IFundaServiceAgent fundaServiceAgent;
        
        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor 
        /// </summary>
        public DataProcessor() : this(new FundaServiceAgent(new HttpClientWrapper()))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fundaServiceAgent">The datasource whcih provides the data input.</param>
        public DataProcessor(IFundaServiceAgent fundaServiceAgent)
        {
            this.fundaServiceAgent = fundaServiceAgent;
        }

        #endregion

        #region public methods

        /// <summary>
        /// This function returns the top 10 of 'Makelaars' from a specific city which have the most objects for sale. 
        /// Optional filtering kan be done on objects with a garden. 
        /// </summary>
        /// <param name="city">The city to search the objects</param>
        /// <param name="withGarden">Optional parameter indicating whether or not to search for object with a garden.</param>
        /// <returns>A list of makelaar-names grouped by the number objects they have for sale.</returns>
        public List<SearchResultItem> GetTop10Makelaars(string city, bool withGarden = false)
        {
            // build the search query (zoekOpdracht)
            var serachQuery = withGarden ? $"/{city}/Tuin/" : $"/{city}/";

            // Retrieve the JSON formatted page-content of each Paging Page 
            var contentPages = this.fundaServiceAgent.GetSearchResultsPages(serachQuery);

            // Create and populate the PageParsers used for parsing and filtering content.
            List<PageParser> pageParsers = new List<PageParser>();
            contentPages.ForEach(x => pageParsers.Add(new PageParser(x)));

            // Filter out the 'Objects' elements from the parsed content pages and select al 'MakelaarNaam' elements.
            var combinedGroupItemsList = pageParsers.SelectMany(x => x.ObjectsElements.Select(y => new GroupingItem()
            {
                Name = y.Elements("MakelaarNaam").FirstOrDefault()?.Value
            }));

            // Group the 'Makelaarnaam' element values by the number of occurences.
            var groups = combinedGroupItemsList.GroupBy(x => x.Name).Select(g => new { g.Key, Count = g.Count() });

            // Select the top 10 of the ´Names´ whcih occur the most.
            var top10SearchItems = groups.Select(x => new SearchResultItem() { Count = x.Count, Name = x.Key }).OrderByDescending(y => y.Count).Take(10);

            // return the list
            return top10SearchItems.ToList();
        }

        #endregion

    }
}

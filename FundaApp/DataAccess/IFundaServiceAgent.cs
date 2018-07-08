using System;
using System.Collections.Generic;
using System.Text;

namespace FundaApp.DataAccess
{
    /// <summary>
    /// The FundaServiceAgent interface definition
    /// </summary>
    public interface IFundaServiceAgent
    {
        /// <summary>
        /// Retrieving objects using a searchQuery. 
        /// </summary>
        /// <param name="searchQuery">The 'zo' search query</param>
        /// <returns>A list of JSON content pages in the form of a list of string </returns>
        List<string> GetSearchResultsPages(string searchQuery);
    }
}

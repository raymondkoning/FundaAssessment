using System;
using System.Collections.Generic;
using System.Text;

using FundaApp.DomainEntities;

namespace FundaApp.BusinessLogic
{
    /// <summary>
    /// The DataProcessor interface definition
    /// </summary>
    public interface IDataProcessor
    {
        /// <summary>
        /// This function returns the top 10 of 'Makelaars' from a specific city which have the most objects for sale. 
        /// Optional filtering kan be done on objects with a garden. 
        /// </summary>
        /// <param name="city">The city to search the objects</param>
        /// <param name="withGarden">Optional parameter indicating whether or not to search for object with a garden.</param>
        /// <returns>A list of makelaar-names grouped by the number objects they have for sale.</returns>
        List<SearchResultItem> GetTop10Makelaars(string city, bool withGarden = false);
    }
}

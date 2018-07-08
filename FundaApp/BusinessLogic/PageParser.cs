using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace FundaApp.BusinessLogic
{
    /// <summary>
    /// This class converts a JSON string to XML and loads it into an XDocument class. With this class it is easy 
    /// to query the XML structure.
    /// </summary>
    public class PageParser
    {
        #region Fields

        private XDocument document;

        #endregion

        #region  public properties

        /// <summary>
        /// Return all 'Objects' elements within the parsed data
        /// </summary>
        public IEnumerable<XElement> ObjectsElements => this.document.Descendants("Objects");

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jsonPageContent">The JSON data representing a single data pages</param>
        public PageParser(string jsonPageContent)
        {
            try
            {
                var xmlString = "{ \"rootNode\" : " + jsonPageContent + "}";
                XmlDocument xmlDocument = JsonConvert.DeserializeXmlNode(xmlString);
                document = XDocument.Parse(xmlDocument.OuterXml);

            }
            catch (Exception)
            {
                throw new ApplicationException("Er is een fout opgetreden tijdens het parsen en coverteren van JSON data naar een XML structuur.");
            }
        }

        #endregion
    }
}

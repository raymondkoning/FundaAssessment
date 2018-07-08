using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FundaApp.DataAccess
{
    /// <summary>
    /// This class wraps the HttpClient class. By using Interface inheritance it can now be used to mock/stub the HttpClient.  
    /// </summary>
    public class HttpClientWrapper : IHttpClientWrapper
    {
        #region Fields

        private readonly HttpClient client;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor 
        /// </summary>
        public HttpClientWrapper()
        {
            client = new HttpClient();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Asunc gets of the response body as string 
        /// </summary>
        /// <param name="requestUri">The target URI</param>
        /// <returns>The Task returnig a string result</returns>
        public Task<string> GetStringAsync(Uri requestUri)
        {
            return client.GetStringAsync(requestUri);
        }

        #endregion
    }
}

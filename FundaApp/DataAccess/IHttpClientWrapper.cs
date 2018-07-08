using System;
using System.Threading.Tasks;

namespace FundaApp.DataAccess
{
    /// <summary>
    /// The HttpClientWrapper interface definition
    /// </summary>
    public interface IHttpClientWrapper
    {
        /// <summary>
        /// Asunc gets of the response body as string 
        /// </summary>
        /// <param name="requestUri">The target URI</param>
        /// <returns>The Task returnig a string result</returns>
        Task<string> GetStringAsync(Uri requestUri);
    }
}

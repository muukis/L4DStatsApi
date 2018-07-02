using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Default OPTIONS method implementation.
        /// </summary>
        /// <returns>The created Microsoft.AspNet.Mvc.StatusCodeResult for the response.</returns>
        protected virtual StatusCodeResult Options(string methods)
        {
            Response.Headers.Add("Access-Control-Allow-Methods", methods);
            return Ok();
        }

        /// <summary>
        /// Creates an Microsoft.AspNetCore.Mvc.StatusCodeResult object that produces an empty InternalServerError (500) response.
        /// </summary>
        /// <returns>The created Microsoft.AspNetCore.Mvc.StatusCodeResult for the response.</returns>
        protected StatusCodeResult Error()
        {
            return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Creates an Microsoft.AspNetCore.Mvc.ObjectResult object that produces an InternalServerError (500) response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <param name="statusCode"></param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.ObjectResult for the response.</returns>
        protected ObjectResult Error(object value, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new ObjectResult(value)
            {
                StatusCode = (int)statusCode
            };
        }
    }
}

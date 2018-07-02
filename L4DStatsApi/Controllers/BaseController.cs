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
            return new StatusCodeResult((int)System.Net.HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Creates an Microsoft.AspNetCore.Mvc.ObjectResult object that produces an InternalServerError (500) response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created Microsoft.AspNetCore.Mvc.ObjectResult for the response.</returns>
        protected ObjectResult Error(object value)
        {
            return new ObjectResult(value)
            {
                StatusCode = (int)System.Net.HttpStatusCode.InternalServerError
            };
        }
    }
}

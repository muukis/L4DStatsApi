using System;
using System.Net;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using L4DStatsApi.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace L4DStatsApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IIdentityService service;

        public IdentityController(IConfiguration configuration, IIdentityService service)
        {
            this.configuration = configuration;
            this.service = service;
        }

#if DEBUG
        /// <summary>
        /// Create TESTING identity token
        /// </summary>
        /// <returns><see cref="BearerTokenResult"/> object</returns>
        [HttpGet]
        [SwaggerOperation("CreateTestingBearerToken")]
        [SwaggerResponse(200, typeof(string), "Returns testing identity token for the stats API.")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Invalid login")]
        public async Task<IActionResult> CreateTestingBearerToken()
        {
            try
            {
                var token = await service.CreateBearerToken(new LoginBody
                {
                    GameServerGroupPrivateKey = Guid.Parse("66edfde5-54d6-4a4d-91b6-40209eb9414c"),
                    GameServerPrivateKey = Guid.Parse("4b12123c-896c-4e01-b966-a2cf57b63357")
                });

                if (token == null)
                {
                    return Error(new ErrorResult
                    {
                        Code = 900,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Invalid login information"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(token);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating identity token"
                });
            }
        }
#endif

        /// <summary>
        /// Create identity token
        /// </summary>
        /// <returns><see cref="BearerTokenResult"/> object</returns>
        [HttpPost]
        [SwaggerOperation("CreateBearerToken")]
        [SwaggerResponse(200, typeof(string), "Returns identity token for the stats API.")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Invalid login")]
        public async Task<IActionResult> CreateBearerToken([FromBody] LoginBody login)
        {
            try
            {
                var token = await service.CreateBearerToken(login);

                if (token == null)
                {
                    return Error(new ErrorResult
                    {
                        Code = 900,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Invalid login information"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(token);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating identity token"
                });
            }
        }
    }
}
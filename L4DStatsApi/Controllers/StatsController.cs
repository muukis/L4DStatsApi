using System;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using L4DStatsApi.Services;
using L4DStatsApi.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace L4DStatsApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    [Authorize(Policy = "GameServer")]
    public class StatsController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;

        public StatsController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;
        }

        /// <summary>
        /// Test
        /// </summary>
        [HttpGet]
        [SwaggerOperation("Test")]
        [SwaggerResponse(200, typeof(string), "Returns identity token for L4D custom player statistics API.")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> Test()
        {
            try
            {
                return Ok();
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
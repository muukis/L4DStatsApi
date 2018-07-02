using System;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
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
        /// Save game statistics
        /// </summary>
        [HttpPost]
        [SwaggerOperation("SaveGameStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> SaveGameStats([FromBody] GameStatsBody gameStats)
        {
            try
            {
                await service.SaveGameStats(gameStats);
                return Ok();
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed saving game statistics"
                });
            }
        }
    }
}
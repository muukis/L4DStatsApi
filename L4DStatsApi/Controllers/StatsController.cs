using System;
using System.Net;
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

        /// <summary>
        /// Get player game statistics
        /// </summary>
        [HttpGet]
        [Route("player/{steamId}")]
        [SwaggerOperation("GetPlayerStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetPlayerStats([FromRoute] string steamId)
        {
            try
            {
                var playerStats = await service.GetPlayerStats(steamId);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
                        Code = 900,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Player not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(playerStats);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }
    }
}
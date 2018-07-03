using System;
using System.Linq;
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
        /// Start a match.
        /// </summary>
        /// <param name="matchStart">Match starting properties. <see cref="MatchStatsBody"/></param>
        /// <returns><see cref="MatchStartedResult"/> object.</returns>
        [HttpPost]
        [Route("match/start")]
        [SwaggerOperation("StartMatch")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> StartMatch([FromBody] MatchStartBody matchStart)
        {
            try
            {
                return Ok(await service.StartMatch(GetGameServerIdentifier(), matchStart));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed to start match"
                });
            }
        }

        /// <summary>
        /// Save match statistics. You can save match statistics as many times as you want, as long as the match has not ended.
        /// </summary>
        /// <param name="matchStats">Match statistics. <see cref="MatchStatsBody"/></param>
        [HttpPost]
        [Route("match")]
        [SwaggerOperation("SaveMatchStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> SaveMatchStats([FromBody] MatchStatsBody matchStats)
        {
            try
            {
                await service.SaveMatchStats(GetGameServerIdentifier(), matchStats);
                return Ok();
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed saving match statistics"
                });
            }
        }

        /// <summary>
        /// Start a match.
        /// </summary>
        /// <param name="matchEnd">Match ending properties. <see cref="MatchEndBody"/></param>
        [HttpPost]
        [Route("match/end")]
        [SwaggerOperation("EndMatch")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> EndMatch([FromBody] MatchEndBody matchEnd)
        {
            try
            {
                await service.EndMatch(GetGameServerIdentifier(), matchEnd);
                return Ok();
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed to end match"
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
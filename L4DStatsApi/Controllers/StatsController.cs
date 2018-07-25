using System;
using System.Threading.Tasks;
using L4DStatsApi.Helpers.Database;
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
        /// Get API user identity.
        /// </summary>
        /// <returns><see cref="GameServerIdentityContainer"/> object</returns>
        protected GameServerIdentityContainer GetApiUserIdentityContainer()
        {
            return new GameServerIdentityContainer
            {
                GameServerIdentifier = Guid.Parse(GetApiUserClaimValue("GameServerIdentifier")),
                GameServerGroupIdentifier = Guid.Parse(GetApiUserClaimValue("GameServerGroupIdentifier"))
            };
        }

        /// <summary>
        /// Start a match. Starting a match will all game server matches that has not ended!
        /// </summary>
        /// <param name="matchStart">Match starting properties. <see cref="MatchStatsBody"/></param>
        /// <returns><see cref="MatchStartedResult"/> object.</returns>
        [HttpPost]
        [Route("match/start")]
        [SwaggerOperation("StartMatch")]
        [SwaggerResponse(200, typeof(MatchStartedResult), "Match ID")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> StartMatch([FromBody] MatchStartBody matchStart)
        {
            try
            {
                return Ok(await service.StartMatch(GetApiUserIdentityContainer(), matchStart));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed to start match"
                });
            }
        }

        /// <summary>
        /// Append match statistics. You can save match statistics as many times as you want, as long as the match has not ended. All stats saved are appended to existing match stats.
        /// </summary>
        /// <param name="matchStats">Match statistics. <see cref="MatchStatsBody"/></param>
        [HttpPost]
        [Route("match")]
        [SwaggerOperation("AppendMatchStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> AppendMatchStats([FromBody] MatchStatsBody matchStats)
        {
            try
            {
                await service.AppendMatchStats(GetApiUserIdentityContainer(), matchStats);
                return Ok();
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
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
                await service.EndMatch(GetApiUserIdentityContainer(), matchEnd);
                return Ok();
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed to end match"
                });
            }
        }
    }
}
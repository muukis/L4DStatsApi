using System;
using System.Net;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
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
    public class PublicStatsController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;

        public PublicStatsController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;
        }

        /// <summary>
        /// Get player game statistics globally.
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

        /// <summary>
        /// Get player game statistics from a group of game servers.
        /// </summary>
        [HttpGet]
        [Route("player/{steamId}/gameservergroup/{gameServerGroupPublicKey}")]
        [SwaggerOperation("GetGameServerGroupPlayerStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayerStats([FromRoute] string steamId, [FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStats(steamId, mp => mp.Match.GameServer.Group.PublicKey == gameServerGroupPublicKey);

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

        /// <summary>
        /// Get player game statistics from a single game server.
        /// </summary>
        [HttpGet]
        [Route("player/{steamId}/gameserver/{gameServerPublicKey}")]
        [SwaggerOperation("GetGameServerPlayerStats")]
        [SwaggerResponse(200, typeof(void))]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayerStats([FromRoute] string steamId, [FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStats(steamId, mp => mp.Match.GameServer.PublicKey == gameServerPublicKey);

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
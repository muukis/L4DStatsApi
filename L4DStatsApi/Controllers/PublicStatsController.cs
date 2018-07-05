using System;
using System.Collections.Generic;
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
        private readonly int maxPageSize;

        public PublicStatsController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;

            this.maxPageSize = int.Parse(this.configuration["MaxPageSize"]);
        }

        /// <summary>
        /// Get player game statistics globally.
        /// </summary>
        [HttpGet]
        [Route("player/{steamId}")]
        [SwaggerOperation("GetPlayerStats")]
        [SwaggerResponse(200, typeof(PlayerStatsResult), "Player statistics")]
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
        [SwaggerResponse(200, typeof(PlayerStatsResult), "Player statistics")]
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
        [SwaggerResponse(200, typeof(PlayerStatsResult), "Player statistics")]
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

        /// <summary>
        /// Get player game statistics globally.
        /// </summary>
        [HttpGet]
        [Route("player/{startingIndex}/{pageSize}")]
        [SwaggerOperation("GetPlayers")]
        [SwaggerResponse(200, typeof(List<PlayerStatsResult>), "List of player statistics")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Players not found")]
        public async Task<IActionResult> GetPlayersStats([FromRoute] int startingIndex, [FromRoute] int pageSize)
        {
            try
            {
                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})");
                }

                var playerStats = await service.GetPlayers(startingIndex, pageSize);

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
        [Route("player/{startingIndex}/{pageSize}/gameservergroup/{gameServerGroupPublicKey}")]
        [SwaggerOperation("GetGameServerGroupPlayersStats")]
        [SwaggerResponse(200, typeof(List<PlayerStatsResult>), "List of player statistics")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayersStats([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})");
                }

                var playerStats = await service.GetPlayers(startingIndex, pageSize, mp => mp.Match.GameServer.Group.PublicKey == gameServerGroupPublicKey);

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
        [Route("player/{startingIndex}/{pageSize}/gameserver/{gameServerPublicKey}")]
        [SwaggerOperation("GetGameServerPlayersStats")]
        [SwaggerResponse(200, typeof(List<PlayerStatsResult>), "List of player statistics")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayersStats([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})");
                }

                var playerStats = await service.GetPlayers(startingIndex, pageSize, mp => mp.Match.GameServer.PublicKey == gameServerPublicKey);

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
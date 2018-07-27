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
        /// Get player basic game statistics globally.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("player/{steamId}/basic")]
        [SwaggerOperation("GetPlayerStatsBasic")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "Player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetPlayerStatsBasic([FromRoute] string steamId)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsBasic(m => m.SteamId == steamId);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player basic statistics"
                });
            }
        }

        /// <summary>
        /// Get player weapon game statistics globally.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <returns><see cref="MultiplePlayerStatsWeaponResult"/> object.</returns>
        [HttpGet]
        [Route("player/{steamId}/weapon")]
        [SwaggerOperation("GetPlayerStatsWeapon")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsWeaponResult), "Player weapon statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetPlayerStatsWeapon([FromRoute] string steamId)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsWeapon(m => m.SteamId == steamId);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player weapon statistics"
                });
            }
        }

        /// <summary>
        /// Get player basic game statistics from a group of game servers.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerGroupPublicKey">Game server group public key.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("gameservergroup/{gameServerGroupPublicKey}/player/{steamId}")]
        [SwaggerOperation("GetGameServerGroupPlayerStats")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "Player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayerStatsBasic([FromRoute] string steamId, [FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsBasic(m => m.SteamId == steamId && m.GameServerGroupPublicKey == gameServerGroupPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player basic game statistics from a single game server.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("gameserver/{gameServerPublicKey}/player/{steamId}")]
        [SwaggerOperation("GetGameServerPlayerStatsBasic")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "Player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayerStatsBasic([FromRoute] string steamId, [FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsBasic(m => m.SteamId == steamId && m.GameServerPublicKey == gameServerPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player weapon game statistics from a group of game servers.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerGroupPublicKey">Game server group public key.</param>
        /// <returns><see cref="MultiplePlayerStatsWeaponResult"/> object.</returns>
        [HttpGet]
        [Route("gameservergroup/{gameServerGroupPublicKey}/player/{steamId}")]
        [SwaggerOperation("GetGameServerGroupPlayerStatsWeapon")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsWeaponResult), "Player weapon statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayerStatsWeapon([FromRoute] string steamId, [FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsWeapon(m => m.SteamId == steamId && m.GameServerGroupPublicKey == gameServerGroupPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player weapon game statistics from a single game server.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MultiplePlayerStatsWeaponResult"/> object.</returns>
        [HttpGet]
        [Route("gameserver/{gameServerPublicKey}/player/{steamId}")]
        [SwaggerOperation("GetGameServerPlayerStatsWeapon")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsWeaponResult), "Player weapon statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayerStatsWeapon([FromRoute] string steamId, [FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                var playerStats = await service.GetPlayerStatsWeapon(m => m.SteamId == steamId && m.GameServerPublicKey == gameServerPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player basic game statistics globally.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">List sort order.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("player/{startingIndex}/{pageSize}/{sortOrder}")]
        [SwaggerOperation("GetPlayers")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Players not found")]
        public async Task<IActionResult> GetPlayersStats([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
        {
            try
            {
                if (startingIndex < 0)
                {
                    throw new ArgumentException($"Invalid starting index value ({startingIndex})", nameof(startingIndex));
                }

                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})", nameof(pageSize));
                }

                var playerStats = await service.GetPlayerStatsBasic(startingIndex, pageSize, sortOrder, m => true);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
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
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player basicgame statistics from a group of game servers.
        /// </summary>
        /// <param name="gameServerGroupPublicKey">Game server group public key</param>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">List sort order.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("gameservergroup/{gameServerGroupPublicKey}/players/{startingIndex}/{pageSize}/{sortOrder}")]
        [SwaggerOperation("GetGameServerGroupPlayersStats")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayersStats([FromRoute] Guid gameServerGroupPublicKey, [FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
        {
            try
            {
                if (startingIndex < 0)
                {
                    throw new ArgumentException($"Invalid starting index value ({startingIndex})", nameof(startingIndex));
                }

                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})", nameof(pageSize));
                }

                var playerStats = await service.GetPlayerStatsBasic(startingIndex, pageSize, sortOrder, m => m.GameServerGroupPublicKey == gameServerGroupPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Players not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(playerStats);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get player basic game statistics from a single game server.
        /// </summary>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">List sort order.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("gameserver/{gameServerPublicKey}/players/{startingIndex}/{pageSize}/{sortOrder}")]
        [SwaggerOperation("GetGameServerPlayersStats")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayersStats([FromRoute] Guid gameServerPublicKey, [FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
        {
            try
            {
                if (startingIndex < 0)
                {
                    throw new ArgumentException($"Invalid starting index value ({startingIndex})", nameof(startingIndex));
                }

                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})", nameof(pageSize));
                }

                var playerStats = await service.GetPlayerStatsBasic(startingIndex, pageSize, sortOrder, m => m.GameServerPublicKey == gameServerPublicKey);

                if (playerStats == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Players not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(playerStats);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting player statistics"
                });
            }
        }

        /// <summary>
        /// Get match statistics.
        /// </summary>
        /// <param name="matchId">Match identifier.</param>
        /// <returns><see cref="MatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("match/{matchId}")]
        [SwaggerOperation("GetMatchStats")]
        [SwaggerResponse(200, typeof(MatchStatsWithPlayersResult), "Match statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Match not found")]
        public async Task<IActionResult> GetMatchStats([FromRoute] Guid matchId)
        {
            try
            {
                var matchStatsResult = await service.GetMatchStatsWithPlayers(matchId);

                if (matchStatsResult == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Match not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(matchStatsResult);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting match statistics"
                });
            }
        }

        /// <summary>
        /// Get game server match statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MultipleMatchStatsResult"/> object.</returns>
        [HttpGet]
        [Route("gameserver/{gameServerPublicKey}/matches/{startingIndex}/{pageSize}")]
        [SwaggerOperation("GetGameServerMatchStats")]
        [SwaggerResponse(200, typeof(MultipleMatchStatsResult), "List of match statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server matches not found")]
        public async Task<IActionResult> GetGameServerMatchStats([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                if (startingIndex < 0)
                {
                    throw new ArgumentException($"Invalid starting index value ({startingIndex})", nameof(startingIndex));
                }

                if (pageSize > this.maxPageSize)
                {
                    throw new ArgumentException($"Maximum page size exceeded ({maxPageSize})", nameof(pageSize));
                }

                var gameServerMatchStatsResult = await service.GetGameServerMatchStats(startingIndex, pageSize, gameServerPublicKey);

                if (gameServerMatchStatsResult == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server matches not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(gameServerMatchStatsResult);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting game server match statistics"
                });
            }
        }

        /// <summary>
        /// Get game server group game servers.
        /// </summary>
        /// <param name="gameServerGroupPublicKey">Game server group public key.</param>
        /// <returns>List of <see cref="GameServerResult"/> objects.</returns>
        [HttpGet]
        [Route("gameservergroup/{gameServerGroupPublicKey}")]
        [SwaggerOperation("GetGameServerGroupGameServers")]
        [SwaggerResponse(200, typeof(List<GameServerResult>), "List of game servers")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server matches not found")]
        public async Task<IActionResult> GetGameServerGroupGameServers([FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                var gameServerGroupGameServers = await service.GetGameServerGroupGameServers(gameServerGroupPublicKey);

                if (gameServerGroupGameServers == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server group not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(gameServerGroupGameServers);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting game server group game servers"
                });
            }
        }

        /// <summary>
        /// Get game server latest match statistics.
        /// </summary>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("gameserver/{gameServerPublicKey}/latestmatch")]
        [SwaggerOperation("GetGameServerLatestMatch")]
        [SwaggerResponse(200, typeof(MatchStatsWithPlayersResult), "Match statistics with players")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server matches not found")]
        public async Task<IActionResult> GetGameServerLatestMatch([FromRoute] Guid gameServerPublicKey)
        {
            try
            {
                var latestMatch = await service.GetGameServerLatestMatch(gameServerPublicKey);

                if (latestMatch == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server latest match not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(latestMatch);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting game server latest match"
                });
            }
        }

        /// <summary>
        /// Get ongoing match statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns><see cref="MultipleMatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("match/ongoing/{startingIndex}/{pageSize}")]
        [SwaggerOperation("GetOngoingMatches")]
        [SwaggerResponse(200, typeof(MultipleMatchStatsWithPlayersResult), "Match statistics with players")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetOngoingMatches([FromRoute] int startingIndex, [FromRoute] int pageSize)
        {
            try
            {
                return Ok(await service.GetOngoingMatches(startingIndex, pageSize));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting game server latest match"
                });
            }
        }

        /// <summary>
        /// Get ongoing match statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="gameServerGroupPublicKey">Game server group public key.</param>
        /// <returns><see cref="MultipleMatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("gameservergroup/{gameServerGroupPublicKey}/match/ongoing/{startingIndex}/{pageSize}")]
        [SwaggerOperation("GetGameServerGroupOngoingMatches")]
        [SwaggerResponse(200, typeof(MultipleMatchStatsWithPlayersResult), "Match statistics with players")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server group not found")]
        public async Task<IActionResult> GetGameServerGroupOngoingMatches([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerGroupPublicKey)
        {
            try
            {
                var matches = await service.GetGameServerGroupOngoingMatches(startingIndex, pageSize, gameServerGroupPublicKey);

                if (matches == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server group not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(matches);
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting game server latest match"
                });
            }
        }
    }
}
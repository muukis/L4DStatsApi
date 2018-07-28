using System;
using System.Net;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Results;
using L4DStatsApi.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace L4DStatsApi.Controllers.Public
{
    /// <summary>
    /// Player controller. (Public)
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;
        private readonly int maxPageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="service"></param>
        public PlayerController(IConfiguration configuration, IStatsService service)
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
        [Route("basic/{steamId}")]
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
        [Route("weapon/{steamId}")]
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
        /// Get player basic game statistics globally.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">List sort order.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("basic/{startingIndex}/{pageSize}/{sortOrder}")]
        [SwaggerOperation("GetPlayerStatsBasic")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Players not found")]
        public async Task<IActionResult> GetPlayerStatsBasic([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
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
        /// Get player basic game statistics from a single game server.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("basic/{steamId}/gameserver/{gameServerPublicKey}")]
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
        /// Get player weapon game statistics from a single game server.
        /// </summary>
        /// <param name="steamId">Player Steam ID.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MultiplePlayerStatsWeaponResult"/> object.</returns>
        [HttpGet]
        [Route("weapon/{steamId}/gameserver/{gameServerPublicKey}")]
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
        /// Get player basic game statistics from a group of game servers.
        /// </summary>
        /// <param name="gameServerGroupPublicKey">Game server group public key</param>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="sortOrder">List sort order.</param>
        /// <returns><see cref="MultiplePlayerStatsBasicResult"/> object.</returns>
        [HttpGet]
        [Route("basic/{startingIndex}/{pageSize}/{sortOrder}/gameservergroup/{gameServerGroupPublicKey}")]
        [SwaggerOperation("GetGameServerGroupPlayersStatsBasic")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerGroupPlayersStatsBasic([FromRoute] Guid gameServerGroupPublicKey, [FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
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
        [Route("basic/{startingIndex}/{pageSize}/{sortOrder}/gameserver/{gameServerPublicKey}")]
        [SwaggerOperation("GetGameServerPlayersStatsBasic")]
        [SwaggerResponse(200, typeof(MultiplePlayerStatsBasicResult), "List of player basic statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Player not found")]
        public async Task<IActionResult> GetGameServerPlayersStatsBasic([FromRoute] Guid gameServerPublicKey, [FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] PlayerSortOrder sortOrder)
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
    }
}
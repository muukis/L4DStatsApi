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

namespace L4DStatsApi.Controllers.Public
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MatchController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;
        private readonly int maxPageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="service"></param>
        public MatchController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;

            this.maxPageSize = int.Parse(this.configuration["MaxPageSize"]);
        }

        /// <summary>
        /// Get match statistics.
        /// </summary>
        /// <param name="matchId">Match identifier.</param>
        /// <returns><see cref="MatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("{matchId}")]
        [SwaggerOperation("GetMatchStats")]
        [SwaggerResponse(200, typeof(MatchStatsWithPlayersResult), "Match statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Match not found")]
        public async Task<IActionResult> GetMatchStats([FromRoute] Guid matchId)
        {
            try
            {
                var result = await service.GetMatchStatsWithPlayers(matchId);

                if (result == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Match not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(result);
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
        /// <returns><see cref="GameServerMatchStatsResult"/> object.</returns>
        [HttpGet]
        [Route("{startingIndex}/{pageSize}/gameserver/{gameServerPublicKey}")]
        [SwaggerOperation("GetGameServerMatchStats")]
        [SwaggerResponse(200, typeof(GameServerMatchStatsResult), "List of match statistics")]
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

                var result = await service.GetGameServerMatchStats(startingIndex, pageSize, gameServerPublicKey);

                if (result == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server matches not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(result);
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
        /// Get game server latest match statistics.
        /// </summary>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="MatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("latest/gameserver/{gameServerPublicKey}")]
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
        /// Get game server match latest statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="gameServerPublicKey">Game server public key.</param>
        /// <returns><see cref="GameServerMatchStatsResult"/> object.</returns>
        [HttpGet]
        [Route("latest/{startingIndex}/{pageSize}/gameserver/{gameServerPublicKey}")]
        [SwaggerOperation("GetGameServerLatestMatches")]
        [SwaggerResponse(200, typeof(GameServerMatchStatsResult), "List of match statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server matches not found")]
        public async Task<IActionResult> GetGameServerLatestMatches([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerPublicKey)
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

                var result = await service.GetGameServerLatestMatches(startingIndex, pageSize, gameServerPublicKey);

                if (result == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server matches not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(result);
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
        /// Get game server group latest match statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="gameServerGroupPublicKey">Game server group public key.</param>
        /// <returns><see cref="GameServerGroupMatchStatsResult"/> object.</returns>
        [HttpGet]
        [Route("latest/{startingIndex}/{pageSize}/gameservergroup/{gameServerGroupPublicKey}")]
        [SwaggerOperation("GetGameServerGroupLatestMatches")]
        [SwaggerResponse(200, typeof(GameServerGroupMatchStatsResult), "List of match statistics")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(404, typeof(ErrorResult), "Game server matches not found")]
        public async Task<IActionResult> GetGameServerGroupLatestMatches([FromRoute] int startingIndex, [FromRoute] int pageSize, [FromRoute] Guid gameServerGroupPublicKey)
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

                var result = await service.GetGameServerGroupLatestMatches(startingIndex, pageSize, gameServerGroupPublicKey);

                if (result == null)
                {
                    return Error(new ErrorResult
                    {
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Game server matches not found"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(result);
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
        /// Get ongoing match statistics.
        /// </summary>
        /// <param name="startingIndex">Starting index. (starting from zero)</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns><see cref="MultipleMatchStatsWithPlayersResult"/> object.</returns>
        [HttpGet]
        [Route("ongoing/{startingIndex}/{pageSize}")]
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
        [Route("ongoing/{startingIndex}/{pageSize}/gameservergroup/{gameServerGroupPublicKey}")]
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
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
    /// Server controller. (Public)
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ServerController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;
        private readonly int maxPageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="service"></param>
        public ServerController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;

            this.maxPageSize = int.Parse(this.configuration["MaxPageSize"]);
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
    }
}
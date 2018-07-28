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
    /// Weapon controller. (Public)
    /// </summary>
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class WeaponController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IStatsService service;
        private readonly int maxPageSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="service"></param>
        public WeaponController(IConfiguration configuration, IStatsService service)
        {
            this.configuration = configuration;
            this.service = service;

            this.maxPageSize = int.Parse(this.configuration["MaxPageSize"]);
        }

        /// <summary>
        /// Get all weapon names in the stats.
        /// </summary>
        /// <returns>List of <see cref="WeaponBaseResult"/> objects.</returns>
        [HttpGet]
        [Route("names")]
        [SwaggerOperation("GetWeaponNames")]
        [SwaggerResponse(200, typeof(List<WeaponBaseResult>), "List of weapon names")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetWeaponNames()
        {
            try
            {
                return Ok(await service.GetWeaponNames());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting weapon names"
                });
            }
        }

        /// <summary>
        /// Get all weapon lethalities in the stats.
        /// </summary>
        /// <returns>List of <see cref="WeaponLethalityResult"/> objects.</returns>
        [HttpGet]
        [Route("lethalities")]
        [SwaggerOperation("GetWeaponLethalities")]
        [SwaggerResponse(200, typeof(List<WeaponLethalityResult>), "List of weapon lethality")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetWeaponLethalities()
        {
            try
            {
                return Ok(await service.GetWeaponLethalities());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting weapon lethalities"
                });
            }
        }

        /// <summary>
        /// Get all weapon headshot per kill ratios in the stats.
        /// </summary>
        /// <returns>List of <see cref="WeaponHeadshotKillRatioResult"/> objects.</returns>
        [HttpGet]
        [Route("headshotratios")]
        [SwaggerOperation("GetWeaponHeadshotKillRatios")]
        [SwaggerResponse(200, typeof(List<WeaponHeadshotKillRatioResult>), "List of weapon headshot per kill ratio")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        public async Task<IActionResult> GetWeaponHeadshotKillRatios()
        {
            try
            {
                return Ok(await service.GetWeaponHeadshotKillRatios());
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed getting weapon headshot kill ratios"
                });
            }
        }
    }
}
﻿using System;
using System.Net;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
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
    public class IdentityController : BaseController
    {
        private readonly IConfiguration configuration;
        private readonly IIdentityService service;

        public IdentityController(IConfiguration configuration, IIdentityService service)
        {
            this.configuration = configuration;
            this.service = service;
        }

        /// <summary>
        /// Create identity token
        /// </summary>
        /// <returns><see cref="BearerTokenResult"/> object</returns>
        [HttpPost]
        [SwaggerOperation("CreateBearerToken")]
        [SwaggerResponse(200, typeof(string), "Returns identity token for the stats API.")]
        [SwaggerResponse(400, typeof(ErrorResult), "Invalid request")]
        [SwaggerResponse(500, typeof(ErrorResult), "Internal server error")]
        [SwaggerResponse(900, typeof(ErrorResult), "Invalid login")]
        public async Task<IActionResult> CreateBearerToken([FromBody] LoginBody login)
        {
            try
            {
                var token = await service.CreateBearerToken(login);

                if (token == null)
                {
                    return Error(new ErrorResult
                    {
                        Code = 900,
                        Classification = ErrorClassification.EntityNotFound,
                        Message = "Login information does not match database value"
                    }, HttpStatusCode.NotFound);
                }

                return Ok(new BearerTokenResult(token));
            }
            catch (Exception)
            {
                return Error(new ErrorResult
                {
                    Code = 500,
                    Classification = ErrorClassification.InternalError,
                    Message = "Failed creating identity token"
                });
            }
        }
    }
}
﻿using System.Threading.Tasks;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;

namespace L4DStatsApi.Interfaces
{
    public interface IIdentityService
    {
        Task<GameSeverIdentityResult> CreateGameServerIdentityToken(LoginBody login);
    }
}

using System;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Pages.Admin
{
    public class IndexModel : ExtendedPageModel
    {
        private readonly IConfiguration configuration;

        public IndexModel(IConfiguration configuration, StatsDbContext dbContext) : base(dbContext)
        {
            this.configuration = configuration;
        }

        public string ErrorMessage { get; set; }

        public int GetMaxGameServersPerGroup()
        {
            if (int.TryParse(configuration["MaxGameServersPerGroup"], out int retval))
            {
                return retval;
            }

            return 20;
        }

        public int GetUserGameServerCount()
        {
            var emailAddress = DbContext.GetUserEmailAddress(User);
            return DbContext.GameServer.Count(gs => 
                gs.IsValid && gs.Group.IsValid
                && gs.Group.EmailAddress == emailAddress);
        }

        private GameServerModel GetUserGameServerModel(Guid gameServerPrivateKey)
        {
            var emailAddress = DbContext.GetUserEmailAddress(User);
            return DbContext.GameServer.Single(gs =>
                gs.PrivateKey == gameServerPrivateKey
                && gs.IsValid && gs.Group.IsValid
                && gs.Group.EmailAddress == emailAddress);
        }

        public void OnPostDeleteGameServer(Guid deleteGameServerPrivateKey)
        {
            try
            {
                var userGameServer = GetUserGameServerModel(deleteGameServerPrivateKey);

                if (userGameServer == null)
                {
                    ErrorMessage = "Game server not found!";
                    return;
                }

                userGameServer.IsActive = false;
                userGameServer.IsValid = false;
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                ErrorMessage = "Creating new game server failed!";
            }
        }

        public void OnPostToggleGameServer(Guid toggleGameServerPrivateKey)
        {
            try
            {
                var userGameServer = GetUserGameServerModel(toggleGameServerPrivateKey);

                if (userGameServer == null)
                {
                    ErrorMessage = "Game server not found!";
                    return;
                }

                userGameServer.IsActive = !userGameServer.IsActive;
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                ErrorMessage = "Creating new game server failed!";
            }
        }

        public void OnPostNewGameServer(string newGameServerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(newGameServerName))
                {
                    ErrorMessage = "Invalid new game server name!";
                    return;
                }

                int maxGameServers = GetMaxGameServersPerGroup();

                if (GetUserGameServerCount() >= maxGameServers)
                {
                    ErrorMessage = $"Maximum game server count exceeded! ({maxGameServers})";
                    return;
                }

                var userGameServerGroup = DbContext.GetUserGameServerGroup(User).Result;

                if (userGameServerGroup == null)
                {
                    return;
                }

                var userGameServer = new GameServerModel
                {
                    GroupId = userGameServerGroup.Id,
                    Name = newGameServerName.Trim()
                };

                DbContext.GameServer.Add(userGameServer);
                DbContext.SaveChanges();
            }
            catch (Exception)
            {
                ErrorMessage = "Creating new game server failed!";
            }
        }
    }
}
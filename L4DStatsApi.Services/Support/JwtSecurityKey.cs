using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace L4DStatsApi.Support
{
    public static class JwtSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
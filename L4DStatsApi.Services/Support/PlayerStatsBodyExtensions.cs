using System;
using System.Collections.Generic;
using System.Text;
using L4DStatsApi.Requests;

namespace L4DStatsApi.Support
{
    public static class PlayerStatsBodyExtensions
    {
        private static readonly Encoding nameEncoding = Encoding.GetEncoding("ISO-8859-1");

        public static string GetBase64DecodedName(this PlayerStatsBody playerStats)
        {
            try
            {
                byte[] data = Convert.FromBase64String(playerStats.Base64EncodedName);
                return nameEncoding.GetString(data);
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
}

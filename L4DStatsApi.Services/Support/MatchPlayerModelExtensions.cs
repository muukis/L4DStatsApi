using System;
using System.Collections.Generic;
using System.Text;
using L4DStatsApi.Models;

namespace L4DStatsApi.Support
{
    public static class MatchPlayerModelExtensions
    {
        private static readonly Encoding nameEncoding = Encoding.GetEncoding("ISO-8859-1");

        public static string GetBase64EncodedName(this MatchPlayerModel matchPlayer)
        {
            try
            {
                byte[] data = nameEncoding.GetBytes(matchPlayer.Name);
                return Convert.ToBase64String(data);
            }
            catch
            {
                return "UNKNOWN";
            }
        }
    }
}

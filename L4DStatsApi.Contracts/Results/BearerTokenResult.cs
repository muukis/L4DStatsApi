namespace L4DStatsApi.Results
{
    public class BearerTokenResult
    {
        public BearerTokenResult()
        { }

        public BearerTokenResult(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}

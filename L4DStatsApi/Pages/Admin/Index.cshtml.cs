namespace L4DStatsApi.Pages.Admin
{
    public class IndexModel : ExtendedPageModel
    {
        public IndexModel(StatsDbContext dbContext) : base(dbContext)
        {
        }

        public void OnGet()
        {

        }
    }
}
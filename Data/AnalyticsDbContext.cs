using Microsoft.EntityFrameworkCore;
using Mi_punto_de_venta.Entities;

namespace Mi_punto_de_venta.Data


{
    public class AnalyticsDbContext:DbContext 
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) 
            : base(options) 
        { }

        public DbSet<SalesAnalytics> SalesAnalytics {  get; set; } 


    }
}

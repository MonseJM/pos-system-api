using Mi_punto_de_venta.Data;
using Microsoft.AspNetCore.Mvc;

namespace Mi_punto_de_venta.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : Controller
    {
        private readonly AnalyticsDbContext _analytics;

        public AnalyticsController(AnalyticsDbContext analytics)
        {
            _analytics = analytics;
        }

        [HttpGet("daily")]
        public IActionResult GetDailySales(DateTime? start, DateTime? end)
        {
            var query = _analytics.SalesAnalytics.AsQueryable();

            if (start.HasValue)
                query = query.Where(x => x.Date >= start);

            if (end.HasValue)
                query = query.Where(x => x.Date <= end);

            var data = query
                .GroupBy(x => x.Date.Date)
                .Select(g => new {
                    Date = g.Key,
                    Total = g.Sum(x => x.Total),
                    Tax = g.Sum(x => x.Tax),
                    Products = g.Sum(x => x.ProductsCount)
                })
                .OrderBy(x => x.Date)
                .ToList();

            return Ok(data);
        }

        [HttpGet("summary")]
        public IActionResult GetSummary()
        {
            var data = _analytics.SalesAnalytics;

            var result = new
            {
                TotalSales = data.Sum(x => x.Total),
                TotalTax = data.Sum(x => x.Tax),
                TotalProducts = data.Sum(x => x.ProductsCount)
            };

            return Ok(result);
        }
    }
}

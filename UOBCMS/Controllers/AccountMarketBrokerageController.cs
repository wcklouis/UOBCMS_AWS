using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountMarketBrokerageController : Controller
    {
        private readonly ApplicationDbContext _context;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AccountMarketBrokerageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("accountmarketccies/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var accountmktbkg = await _context.Cms_account_market_brokerages
                .Include(cl => cl.Cms_account_market_brokerage_details)
                    .Include(clm => clm.Cms_account_market)
                        .ThenInclude(cla => cla.Cms_account)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (accountmktbkg == null)
            {
                return NotFound();
            }
            return View(accountmktbkg);
        }
    }
}

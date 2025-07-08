using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Interface;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountMarketController : Controller
    {
        private readonly ApplicationDbContext _context;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AccountMarketController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("accountmarkets/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var accountmkt = await _context.Cms_account_markets
                .Include(cl => cl.Cms_account_market_limits)
                .Include(cli => cli.Cms_account_market_interests)
                .Include(cli => cli.Cms_account_market_brokerages)
                .Include(clp => clp.Cms_account_market_price_caps)
                .Include(clp => clp.Cms_account_market_inscats)
                .Include(clp => clp.Cms_account_market_cash_sis)
                .Include(clp => clp.Cms_account_market_inst_sis)
                .Include(clp => clp.Cms_account_market_confos)
                .Include(clp => clp.Cms_account_market_rebates)
                .Include(cla => cla.Cms_account)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (accountmkt == null)
            {
                return NotFound();
            }
            return View(accountmkt);
        }
    }
}

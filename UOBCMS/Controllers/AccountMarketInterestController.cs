using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountMarketInterestController : Controller
    {
        private readonly ApplicationDbContext _context;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AccountMarketInterestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("accountmarketccies/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var accountmktint = await _context.Cms_account_market_interests
                .Include(cl => cl.Cms_account_market_interest_details)
                    .Include(clm => clm.Cms_account_market)
                        .ThenInclude(cla => cla.Cms_account)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (accountmktint == null)
            {
                return NotFound();
            }
            return View(accountmktint);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Interface;
using UOBCMS.Models;
using UOBCMS.Models.api;
using UOBCMS.Models.dto;
using UOBCMS.Classes;
using System.Reflection;
using log4net;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AEController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IBOApplicationDbContext _iboContext;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AEController(IBOApplicationDbContext iboContext, ApplicationDbContext context, IInstrumentHoldingRepository insHoldingRepository)
        {
            _context = context;
            _iboContext = iboContext;
        }

        // GET: AE
        [HttpGet("api/aeByAccno/{accno}")]
        public async Task<IActionResult> GetAEbyAccno(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAEbyAccno";

            var account = await _context.Cms_accounts
                .Include(c => c.Cms_ae)
                .FirstOrDefaultAsync(a => a.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"AE of : {accno} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the account is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"AE of: {accno} is found", Logger.INFO);

            return Ok(account);
        }

    }
}

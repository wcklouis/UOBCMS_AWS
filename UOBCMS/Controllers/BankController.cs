using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Models.dto;

namespace UOBCMS.Controllers
{
    [Route("api/Banks")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        /*public IActionResult Index()
        {
            return View();
        }*/

        public BankController(ApplicationDbContext context)
        {
            _context = context;
        }        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankDto>>> GetBanks(string searchTerm = "", int pageNumber = 1, int pageSize = 100, string bankType = "0")
        {
            var query = _context.Cms_banks.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b => (b.Bank_code.Contains(searchTerm) || b.Bank_name.Contains(searchTerm)) && b.Type == bankType);
            }
            else
            {
                query = query.Where(b => b.Type == bankType);
            }

            var totalRecords = await query.CountAsync();

            var banks = await query
                                .OrderBy(b => b.Bank_code)
                                .Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .Select(b => new BankDto
                                {
                                    Id = b.Id,
                                    BankCode = b.Bank_code,
                                    BankName = b.Bank_name,
                                    BankSwift = b.Bank_swift
                                })
                                .ToListAsync();

            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Banks = banks
            });
        }
    }
}

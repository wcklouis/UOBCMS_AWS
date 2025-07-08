using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Models;
using UOBCMS.Models.api;
using UOBCMS.Models.dto;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstrumentController : Controller
    {
        private readonly IBOApplicationDbContext _iboContext;
        public InstrumentController(IBOApplicationDbContext iboContext)
        {
            _iboContext = iboContext;
        }

        [HttpGet("api/instrument/{mktCode}/{secCode}")]
        public async Task<IActionResult> GetInstrument(string mktCode, string secCode)
        {
            var ins = await _iboContext.Instruments
                .FirstOrDefaultAsync(c => (c.Market == mktCode && c.Instr == secCode));

            if (ins == null)
            {
                return NotFound(); // Return 404 if the instrument is not found
            }

            return Ok(ins);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstrumentDto>>> GetInstruments(string mkt = "", string searchTerm = "", int pageNumber = 1, int pageSize = 100)
        {
            var query = _iboContext.Instruments.AsQueryable();

            // Apply filtering 
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (mkt == "HK")
                    query = query.Where(b => ((b.Instr.Contains(searchTerm) || b.Name.Contains(searchTerm) || b.Market.Contains(searchTerm)) && b.Market == "HKG"));
                else if (mkt == "nonHK")
                    query = query.Where(b => ((b.Instr.Contains(searchTerm) || b.Name.Contains(searchTerm) || b.Market.Contains(searchTerm)) && b.Market != "HKG"));
                else
                    query = query.Where(b => b.Instr.Contains(searchTerm) || b.Name.Contains(searchTerm) || b.Market.Contains(searchTerm));
            }
            else
            {
                if (mkt == "HK")
                    query = query.Where(b => b.Market == "HKG");
                else if (mkt == "nonHK")
                    query = query.Where(b => b.Market != "HKG");
            }
            
            var totalRecords = await query.CountAsync();

            // sql server 2012 later
            // var instruments = await query
            //                    .OrderBy(b => b.Market)
             //                   .ThenBy(b => b.Instr)
               //                 .Skip((pageNumber - 1) * pageSize)
               //                 .Take(pageSize)
               //                 .Select(b => new InstrumentDto
               //                 {
                //                    Market = b.Market,
                //                    Instr = b.Instr,
                //                    Name = b.Name
                //                })
                //                .ToListAsync();

            // sql serve r2012 before
            var instruments = await query
                                .OrderBy(b => b.Market)
                                .ThenBy(b => b.Instr)
                                .Select(b => new
                                {
                                    Market = b.Market,
                                    Instr = b.Instr,
                                    Name = b.Name,
                                    Ccy = b.TradingCCY
                                })
                                .ToListAsync();

            var pagedInstruments = instruments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            // Return the results using CustomJsonResult
            return Ok(new
            {
                TotalRecords = totalRecords,
                Instruments = pagedInstruments
            });
        }
    }
}

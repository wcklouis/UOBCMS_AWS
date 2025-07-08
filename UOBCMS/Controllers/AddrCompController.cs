using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UOBCMS.Data;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddrCompController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AddrCompController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetProvinces/{country}")]
        public async Task<ActionResult<IEnumerable<string>>> GetProvinces(string country)
        {
            var opts = await _context.Cms_addr_comp_opts.Where(c => c.Country == country).ToListAsync();
            var optitems = opts.Select(c => new
            {
                Code = c.ProvinceEn,
                NameEn = c.ProvinceEn,
                NameZh = c.ProvinceZh
            }).ToList();

            return Ok(optitems); 
        }

        [HttpGet("GetDistricts/{country}")]        
        public async Task<ActionResult<IEnumerable<string>>> GetDistricts(string country)
        {
            var opts = await _context.Cms_addr_comp_opts.Where(c => c.Country == country).ToListAsync();

            var optitems = opts.Select(c => new
            {
                Code = c.DistrictEn,
                NameEn = c.DistrictEn,
                NameZh = c.DistrictZh
            }).ToList();

            return Ok(optitems);
        }
    }
}

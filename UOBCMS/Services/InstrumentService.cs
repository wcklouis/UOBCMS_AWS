using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;

namespace UOBCMS.Services
{
    public class InstrumentService
    {
        private readonly IBOApplicationDbContext _iboContext;

        public InstrumentService(IBOApplicationDbContext iboContext)
        {
            _iboContext = iboContext;
        }

        public string GetSecuritiesName(string mktcode, string seccode)
        {
            return _iboContext.Instruments
                .FirstOrDefault(c => (c.Market == mktcode && c.Instr == seccode))?.Name; // Adjust based on your actual property names
        }
    }
}

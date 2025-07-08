using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UOBCMS.Data;
using UOBCMS.Interface;
using UOBCMS.Models.IBO;

namespace UOBCMS.Repository
{
    public class InstrumentHoldingRespository : IInstrumentHoldingRepository
    {
        private readonly IBOApplicationDbContext _iboContext;

        public InstrumentHoldingRespository(IBOApplicationDbContext iboContext)
        {
            _iboContext = iboContext;
        }

        public async Task<List<InstrumentHolding>> CallStoredProcedureAsync(DateTime dt, String accno)
        {
            return await _iboContext.Set<InstrumentHolding>()
                .FromSqlRaw("EXEC sp_InstrumentBalance_ClosingASAT @AsAtDate, @Market, @Instrument, @Clnt, @BalanceType, @AcctType, @AccountNature",
                            new SqlParameter("@AsAtDate", dt.ToString("yyyy-MM-dd")),
                            new SqlParameter("@Market", ""),
                            new SqlParameter("@Instrument", ""),
                            new SqlParameter("@Clnt", accno),
                            new SqlParameter("@BalanceType", "O"),
                            new SqlParameter("@AcctType", ""),
                            new SqlParameter("@AccountNature", "Client"))
                .ToListAsync();
        }
    }
}

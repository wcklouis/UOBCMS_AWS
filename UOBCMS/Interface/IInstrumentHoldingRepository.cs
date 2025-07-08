using UOBCMS.Models.IBO;

namespace UOBCMS.Interface
{
    public interface IInstrumentHoldingRepository
    {
        Task<List<InstrumentHolding>> CallStoredProcedureAsync(DateTime dt, string accno);
    }
}

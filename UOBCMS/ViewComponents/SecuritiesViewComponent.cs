using Microsoft.AspNetCore.Mvc;
using UOBCMS.Services;

namespace UOBCMS.ViewComponents
{
    public class SecuritiesViewComponent : ViewComponent
    {
        private readonly InstrumentService _instrumentService;

        public SecuritiesViewComponent(InstrumentService instrumentService)
        {
            _instrumentService = instrumentService;
        }

        public IViewComponentResult Invoke(string mktCode, string secCode)
        {
            var secName = _instrumentService.GetSecuritiesName(mktCode, secCode);
            return Content(secName);
        }
    }
}

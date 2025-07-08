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
using UOBCMS.Models.IBO;

namespace UOBCMS.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IBOApplicationDbContext _iboContext;
        private readonly IInstrumentHoldingRepository _insHoldingRepository;

        /*public IActionResult Index()
        {
            return View();
        }*/

        public AccountController(IBOApplicationDbContext iboContext, ApplicationDbContext context, IInstrumentHoldingRepository insHoldingRepository)
        {
            _context = context;
            _iboContext = iboContext;
            _insHoldingRepository = insHoldingRepository;
        }

        [HttpGet("GetIBOAccountCPerson/{accno}/{type}")]
        public async Task<ActionResult<IEnumerable<ClntContact>>> GetIBOAccountCPerson(string accno, string type)
        {
            string className = GetType().Name;
            string methodName = "GetIBOAccountCPerson";
            Logger.LogErrorMessage(className, methodName, "", $"Account Code: {accno}", Logger.INFO);

            var clntContact = await _iboContext.ClntContact
                .FirstOrDefaultAsync(c => c.ClntCode == accno && c.ContactType == "CPerson" + type);

            return Ok(clntContact);
        }

        [HttpGet("GetIBOUserContact/{accno}")]
        public async Task<ActionResult<IEnumerable<UserIDContact>>> GetIBOUserContact(string accno)
        {
            string className = GetType().Name; 
            string methodName = "GetIBOUserContact";
            Logger.LogErrorMessage(className, methodName, "", $"Account Code: {accno}", Logger.INFO);

            var clnt = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (clnt == null)
            {
                string errMsg = $"Account: {accno} is not found.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return null;
            }

            var userIdContactList = await _iboContext.UserIDContact
                .Where(c => c.UserID == clnt.Aecode && c.Active == "Yes" && (c.ContactType == "SAemail" || c.ContactType == "AEemail")).ToListAsync();

            return Ok(userIdContactList);
        }


        [HttpGet("GetIBOAccountCRS/{accno}")]
        public async Task<ActionResult<IEnumerable<string>>> GetIBOAccountCRS(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetIBOAccountCRS";
            Logger.LogErrorMessage(className, methodName, "", $"Account Code: {accno}", Logger.INFO);

            var ClntCRS = await _iboContext.ClntCRS
                    .FirstOrDefaultAsync(c => c.ClntCode == accno);

            return Ok(ClntCRS);
        }

        [HttpGet("GetClientInsHoldings/{dt}/{accno}")]
        public async Task<ActionResult<IEnumerable<string>>> GetClientInsHoldings(DateTime dt, string accno)
        {
            string className = GetType().Name;
            string methodName = "GetClientInsHoldings";
            Logger.LogErrorMessage(className, methodName, "", $"Account Code: {accno}, Date: {dt.ToString()}", Logger.INFO);

            var insHoldings = await _insHoldingRepository.CallStoredProcedureAsync(dt, accno);
            return Ok(insHoldings);
        }


        [HttpPost("SaveMthSavingPlan")]
        public async Task<ActionResult> SaveMthSavingPlan([FromBody] List<body_cms_account_mthsaving_plan> bodyAccountMthSavingPlanList)
        {
            string className = GetType().Name;
            string methodName = "SaveMthSavingPlan";
            Logger.LogErrorMessage(className, methodName, "", bodyAccountMthSavingPlanList.ToString(), Logger.INFO);

            List<cms_account_mthsaving_plan> accountMthSavingPlanList = new List<cms_account_mthsaving_plan>();

            foreach (body_cms_account_mthsaving_plan bodyAccountMthSavingPlan in bodyAccountMthSavingPlanList)
            {
                if (bodyAccountMthSavingPlan == null)
                {
                    string errMsg = "Invalid Monthly Saving Plan data.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                var account = await _context.Cms_accounts
                    .FirstOrDefaultAsync(c => c.AccNo == bodyAccountMthSavingPlan.Accno);

                if (account == null)
                {
                    string errMsg = $"Account: {bodyAccountMthSavingPlan.Accno} is not found.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return null;
                }

                var accountMthSavingPlan = await _context.Cms_account_mthsaving_plans
                        .FirstOrDefaultAsync(q => (q.Acc_id == account.Id && q.Sec_code == bodyAccountMthSavingPlan.Sec_code && q.Eff_start_dt <= DateTime.Now.Date && q.Eff_end_dt >= DateTime.Now.Date));

                bodyAccountMthSavingPlan.Acc_id = account.Id;

                if (accountMthSavingPlan == null)
                {
                    accountMthSavingPlan = await AddMthSavingPlanSec(bodyAccountMthSavingPlan);
                    if (accountMthSavingPlan == null)
                    {
                        string errMsg = "Fail to create Securities for Monthly Saving Plan.";
                        Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                        return BadRequest(errMsg);
                    }
                }
                else
                {
                    accountMthSavingPlan = await ReplaceMthSavingPlanSec(bodyAccountMthSavingPlan);
                    if (accountMthSavingPlan == null)
                    {
                        string errMsg = "Fail to replace Securities for Monthly Saving Plan.";
                        Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                        return BadRequest(errMsg);
                    }
                }

                accountMthSavingPlanList.Add(accountMthSavingPlan);
            }

            await _context.SaveChangesAsync();

            return Ok(accountMthSavingPlanList);
        }

        private async Task<cms_account_mthsaving_plan> ReplaceMthSavingPlanSec(body_cms_account_mthsaving_plan bodyAccountMthSavingPlan)
        {
            if (bodyAccountMthSavingPlan == null)
                return null;

            var account = await _context.Cms_accounts
                    .FirstOrDefaultAsync(c => c.AccNo == bodyAccountMthSavingPlan.Accno);

            if (account == null)
                return null;

            var accountMthSavingPlan = await _context.Cms_account_mthsaving_plans
                    .FirstOrDefaultAsync(q => (q.Acc_id == account.Id && q.Sec_code == bodyAccountMthSavingPlan.Sec_code && q.Eff_start_dt <= DateTime.Now.Date && q.Eff_end_dt >= DateTime.Now.Date));

            if (accountMthSavingPlan == null)
                return null;

            // Set the Lastupdatedatetime to the current time
            accountMthSavingPlan.Acc_id = bodyAccountMthSavingPlan.Acc_id;
            //accountMthSavingPlan.Eff_start_dt = bodyAccountMthSavingPlan.Eff_start_dt;    // only need to update end day which is in case of termination
            accountMthSavingPlan.Eff_end_dt = bodyAccountMthSavingPlan.Eff_end_dt;
            accountMthSavingPlan.Mkt_code = bodyAccountMthSavingPlan.Mkt_code;
            accountMthSavingPlan.Sec_code = bodyAccountMthSavingPlan.Sec_code;
            accountMthSavingPlan.Ccy = bodyAccountMthSavingPlan.Ccy;
            accountMthSavingPlan.Invest_amt = bodyAccountMthSavingPlan.Invest_amt;
            accountMthSavingPlan.Lastupdatedatetime = DateTime.UtcNow;
            accountMthSavingPlan.Lastupdateuserid = accountMthSavingPlan.Lastupdateuserid;
            //accountMthSavingPlan.Version += 1;
            accountMthSavingPlan.Dbopr = "U";

            _context.Cms_account_mthsaving_plans.Update(accountMthSavingPlan);

            // Save changes to the database
            //await _context.SaveChangesAsync();

            return accountMthSavingPlan;
        }

        private async Task<cms_account_mthsaving_plan> AddMthSavingPlanSec(body_cms_account_mthsaving_plan bodyAccountMthSavingPlan)
        {
            if (bodyAccountMthSavingPlan == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountMthSavingPlan.Accno);

            if (account == null)
                return null;

            cms_account_mthsaving_plan accountMthSavingPlan = new cms_account_mthsaving_plan();

            // Set the Lastupdatedatetime to the current time
            accountMthSavingPlan.Acc_id = account.Id;
            accountMthSavingPlan.Eff_start_dt = bodyAccountMthSavingPlan.Eff_start_dt;
            accountMthSavingPlan.Eff_end_dt = bodyAccountMthSavingPlan.Eff_end_dt;
            accountMthSavingPlan.Mkt_code = bodyAccountMthSavingPlan.Mkt_code;
            accountMthSavingPlan.Sec_code = bodyAccountMthSavingPlan.Sec_code;
            accountMthSavingPlan.Ccy = bodyAccountMthSavingPlan.Ccy;
            accountMthSavingPlan.Invest_amt = bodyAccountMthSavingPlan.Invest_amt;
            accountMthSavingPlan.Lastupdatedatetime = DateTime.UtcNow;
            accountMthSavingPlan.Lastupdateuserid = accountMthSavingPlan.Lastupdateuserid;
            accountMthSavingPlan.Version = 1;
            accountMthSavingPlan.Dbopr = "I";
            _context.Cms_account_mthsaving_plans.Add(accountMthSavingPlan);

            // Save changes to the database
            //await _context.SaveChangesAsync();

            // Return a success response with the newly created record
            return accountMthSavingPlan;
        }

        [HttpPost("SaveQuoteService")]
        public async Task<ActionResult> SaveQuoteService([FromBody] body_cms_account_quoteservice bodyAccountQuoteService)
        {
            string className = GetType().Name;
            string methodName = "SaveQuoteService";
            Logger.LogErrorMessage(className, methodName, "", bodyAccountQuoteService.ToString(), Logger.INFO);

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountQuoteService.Accno);

            if (account == null)
                return null;

            // Validate the input
            if (bodyAccountQuoteService == null)
            {
                string errMsg = "Invalid Quote Service data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var accountQuoteService = await _context.Cms_account_quoteservices
                    .FirstOrDefaultAsync(q => q.Acc_id == account.Id);

            bodyAccountQuoteService.Acc_id = account.Id;

            if (accountQuoteService == null)
            {
                accountQuoteService = await AddQuoteService(bodyAccountQuoteService);
                if (accountQuoteService == null)
                {
                    string errMsg = "Fail to create Quote Service.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceQuoteService(bodyAccountQuoteService))
                    return BadRequest("Fail to replace Quote Service.");
            }

            return Ok(accountQuoteService);
        }



        private async Task<bool> ReplaceQuoteService(body_cms_account_quoteservice bodyAccountQuoteService)
        {
            if (bodyAccountQuoteService == null)
                return false;

            var accountQuoteService = await _context.Cms_account_quoteservices
                    .FirstOrDefaultAsync(q => q.Acc_id == bodyAccountQuoteService.Acc_id);

            if (accountQuoteService == null)
                return false;

            // Set the Lastupdatedatetime to the current time
            accountQuoteService.Acc_id = bodyAccountQuoteService.Acc_id;
            accountQuoteService.End_dt = bodyAccountQuoteService.End_dt;
            accountQuoteService.Lastupdatedatetime = DateTime.UtcNow;
            accountQuoteService.Lastupdateuserid = accountQuoteService.Lastupdateuserid;
            //accountQuoteService.Version += 1;
            accountQuoteService.Dbopr = "U";

            _context.Cms_account_quoteservices.Update(accountQuoteService);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<cms_account_quoteservice> AddQuoteService(body_cms_account_quoteservice bodyAccountQuoteService)
        {
            if (bodyAccountQuoteService == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountQuoteService.Accno);

            if (account == null)
                return null;

            cms_account_quoteservice accountQuoteService = new cms_account_quoteservice();

            // Set the Lastupdatedatetime to the current time
            accountQuoteService.Acc_id = account.Id;
            accountQuoteService.End_dt = bodyAccountQuoteService.End_dt;
            accountQuoteService.Lastupdatedatetime = DateTime.UtcNow;
            accountQuoteService.Lastupdateuserid = accountQuoteService.Lastupdateuserid;
            accountQuoteService.Version = 1;
            accountQuoteService.Dbopr = "I";
            _context.Cms_account_quoteservices.Add(accountQuoteService);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created w8
            return accountQuoteService;
        }

        [HttpPost("SaveHKIDR")]
        public async Task<ActionResult> SaveHKIDR([FromBody] body_cms_account_hkidr bodyAccountHKIDR)
        {
            string className = GetType().Name;
            string methodName = "SaveHKIDR";
            Logger.LogErrorMessage(className, methodName, "", bodyAccountHKIDR.ToString(), Logger.INFO);

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountHKIDR.Accno);

            if (account == null)
                return null;

            // Validate the input
            if (bodyAccountHKIDR == null)
            {
                string errMsg = "Invalid HKIDR data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var accountHKIDR = await _context.Cms_account_hkidrs
                    .FirstOrDefaultAsync(q => q.Acc_id == account.Id);

            bodyAccountHKIDR.Acc_id = account.Id;

            if (accountHKIDR == null)
            {
                accountHKIDR = await AddHKIDR(bodyAccountHKIDR);
                if (accountHKIDR == null)
                {
                    string errMsg = "Fail to create HKIDR.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceHKIDR(bodyAccountHKIDR))
                {
                    string errMsg = "Fail to replace HKIDR.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(accountHKIDR);
        }

        private async Task<bool> ReplaceHKIDR(body_cms_account_hkidr bodyAccountHKIDR)
        {
            if (bodyAccountHKIDR == null)
                return false;

            var accountHKIDR = await _context.Cms_account_hkidrs
                    .FirstOrDefaultAsync(q => q.Acc_id == bodyAccountHKIDR.Acc_id);

            if (accountHKIDR == null)
                return false;

            // Set the Lastupdatedatetime to the current time
            accountHKIDR.Acc_id = bodyAccountHKIDR.Acc_id;
            accountHKIDR.Dt = bodyAccountHKIDR.Dt;
            accountHKIDR.Lastupdatedatetime = DateTime.UtcNow;
            accountHKIDR.Lastupdateuserid = accountHKIDR.Lastupdateuserid;
            //accountHKIDR.Version += 1;
            accountHKIDR.Dbopr = "U";

            _context.Cms_account_hkidrs.Update(accountHKIDR);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<cms_account_hkidr> AddHKIDR(body_cms_account_hkidr bodyAccountHKIDR)
        {
            if (bodyAccountHKIDR == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountHKIDR.Accno);

            if (account == null)
                return null;

            // Add the new w8 to the context (account level)
            cms_account_hkidr accountHKIDR = new cms_account_hkidr();

            // Set the Lastupdatedatetime to the current time
            accountHKIDR.Acc_id = account.Id;
            accountHKIDR.Dt = bodyAccountHKIDR.Dt;
            accountHKIDR.Lastupdatedatetime = DateTime.UtcNow;
            accountHKIDR.Lastupdateuserid = accountHKIDR.Lastupdateuserid;
            accountHKIDR.Version = 1;
            accountHKIDR.Dbopr = "I";
            _context.Cms_account_hkidrs.Add(accountHKIDR);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created w8
            return accountHKIDR;
        }

        [HttpPost("SaveDKQ")]
        public async Task<ActionResult> SaveDKQ([FromBody] body_cms_account_dkq bodyAccountDKQ)
        {
            string className = GetType().Name;
            string methodName = "SaveDKQ";
            Logger.LogErrorMessage(className, methodName, "", bodyAccountDKQ.ToString(), Logger.INFO);

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountDKQ.Accno);

            if (account == null)
                return null;

            // Validate the input
            if (bodyAccountDKQ == null)
            {
                string errMsg = "Invalid DKQ data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var accountDKQ = await _context.Cms_account_dkqs
                    .FirstOrDefaultAsync(q => q.Acc_id == account.Id);

            bodyAccountDKQ.Acc_id = account.Id;

            if (accountDKQ == null)
            {
                accountDKQ = await AddDKQ(bodyAccountDKQ);
                if (accountDKQ == null)
                {
                    string errMsg = "Fail to create DKQ.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceDKQ(bodyAccountDKQ))
                {
                    string errMsg = "Fail to replace DKQ.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(accountDKQ);
        }

        private async Task<bool> ReplaceDKQ(body_cms_account_dkq bodyAccountDKQ)
        {
            if (bodyAccountDKQ == null)
                return false;

            var accountDKQ = await _context.Cms_account_dkqs
                    .FirstOrDefaultAsync(q => q.Acc_id == bodyAccountDKQ.Acc_id);

            if (accountDKQ == null)
                return false;

            // Set the Lastupdatedatetime to the current time
            accountDKQ.Acc_id = bodyAccountDKQ.Acc_id;
            accountDKQ.TrdExpOnexch_RelevantFinInst = bodyAccountDKQ.TrdExpOnexch_RelevantFinInst;
            accountDKQ.TrdExpOnexch_ProdType = bodyAccountDKQ.TrdExpOnexch_ProdType;
            accountDKQ.TrdExpOnexch_TradingPeriod = bodyAccountDKQ.TrdExpOnexch_TradingPeriod;
            accountDKQ.TrdExpOnexch_Yes = bodyAccountDKQ.TrdExpOnexch_Yes;
            accountDKQ.TrdExpOnexch_No = bodyAccountDKQ.TrdExpOnexch_No;
            accountDKQ.WkExpOnDerProd_Employer = bodyAccountDKQ.WkExpOnDerProd_Employer;
            accountDKQ.WkExpOnDerProd_Dept = bodyAccountDKQ.WkExpOnDerProd_Dept;
            accountDKQ.WkExpOnDerProd_Pos = bodyAccountDKQ.WkExpOnDerProd_Pos;
            accountDKQ.WkExpOnDerProd_WorkingYear = bodyAccountDKQ.WkExpOnDerProd_WorkingYear;
            accountDKQ.WkExpOnDerProd_Yes = bodyAccountDKQ.WkExpOnDerProd_Yes;
            accountDKQ.WkExpOnDerProd_No = bodyAccountDKQ.WkExpOnDerProd_No;
            accountDKQ.Training_CoursesName = bodyAccountDKQ.Training_CoursesName;
            accountDKQ.Training_Yes = bodyAccountDKQ.Training_Yes;
            accountDKQ.Training_No = bodyAccountDKQ.Training_No;
            accountDKQ.Training_OrgInstName = bodyAccountDKQ.Training_OrgInstName;
            accountDKQ.Training_AttendanceDate = bodyAccountDKQ.Training_AttendanceDate;
            accountDKQ.Whatisderivative_BankDeposit = bodyAccountDKQ.Whatisderivative_BankDeposit;
            accountDKQ.Whatisderivative_StkOrWithOwnership = bodyAccountDKQ.Whatisderivative_StkOrWithOwnership;
            accountDKQ.Whatisderivative_Loan = bodyAccountDKQ.Whatisderivative_Loan;
            accountDKQ.Whatisderivative_FinInstFmUndAsset = bodyAccountDKQ.Whatisderivative_FinInstFmUndAsset;
            accountDKQ.DerRiskfactors_CptyRisk = bodyAccountDKQ.DerRiskfactors_CptyRisk;
            accountDKQ.DerRiskfactors_LiquidityRisk = bodyAccountDKQ.DerRiskfactors_LiquidityRisk;
            accountDKQ.DerRiskfactors_MarketRisk = bodyAccountDKQ.DerRiskfactors_MarketRisk;
            accountDKQ.DerRiskfactors_All = bodyAccountDKQ.DerRiskfactors_All;
            accountDKQ.TypesOfFutures_IndFut = bodyAccountDKQ.TypesOfFutures_IndFut;
            accountDKQ.TypesOfFutures_ComFut = bodyAccountDKQ.TypesOfFutures_ComFut;
            accountDKQ.TypesOfFutures_CcyFut = bodyAccountDKQ.TypesOfFutures_CcyFut;
            accountDKQ.TypesOfFutures_All = bodyAccountDKQ.TypesOfFutures_All;
            accountDKQ.IslvgeDer_Yes = bodyAccountDKQ.IslvgeDer_Yes;
            accountDKQ.IslvgeDev_No = bodyAccountDKQ.IslvgeDev_No;
            accountDKQ.LossInExcessInitMarginFunds_Yes = bodyAccountDKQ.LossInExcessInitMarginFunds_Yes;
            accountDKQ.LossInExcessInitMarginFunds_No = bodyAccountDKQ.LossInExcessInitMarginFunds_No;
            accountDKQ.Lastupdatedatetime = DateTime.UtcNow;
            accountDKQ.Lastupdateuserid = accountDKQ.Lastupdateuserid;
            //accountDKQ.Version += 1;
            accountDKQ.Dbopr = "U";

            _context.Cms_account_dkqs.Update(accountDKQ);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<cms_account_dkq> AddDKQ(body_cms_account_dkq bodyAccountDKQ)
        {
            if (bodyAccountDKQ == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountDKQ.Accno);

            if (account == null)
                return null;

            // Add the new w8 to the context (account level)
            cms_account_dkq accountDKQ = new cms_account_dkq();

            // Set the Lastupdatedatetime to the current time
            accountDKQ.Acc_id = account.Id;
            accountDKQ.TrdExpOnexch_RelevantFinInst = bodyAccountDKQ.TrdExpOnexch_RelevantFinInst;
            accountDKQ.TrdExpOnexch_ProdType = bodyAccountDKQ.TrdExpOnexch_ProdType;
            accountDKQ.TrdExpOnexch_TradingPeriod = bodyAccountDKQ.TrdExpOnexch_TradingPeriod;
            accountDKQ.TrdExpOnexch_Yes = bodyAccountDKQ.TrdExpOnexch_Yes;
            accountDKQ.TrdExpOnexch_No = bodyAccountDKQ.TrdExpOnexch_No;
            accountDKQ.WkExpOnDerProd_Employer = bodyAccountDKQ.WkExpOnDerProd_Employer;
            accountDKQ.WkExpOnDerProd_Dept = bodyAccountDKQ.WkExpOnDerProd_Dept;
            accountDKQ.WkExpOnDerProd_Pos = bodyAccountDKQ.WkExpOnDerProd_Pos;
            accountDKQ.WkExpOnDerProd_WorkingYear = bodyAccountDKQ.WkExpOnDerProd_WorkingYear;
            accountDKQ.WkExpOnDerProd_Yes = bodyAccountDKQ.WkExpOnDerProd_Yes;
            accountDKQ.WkExpOnDerProd_No = bodyAccountDKQ.WkExpOnDerProd_No;
            accountDKQ.Training_CoursesName = bodyAccountDKQ.Training_CoursesName;
            accountDKQ.Training_Yes = bodyAccountDKQ.Training_Yes;
            accountDKQ.Training_No = bodyAccountDKQ.Training_No;
            accountDKQ.Training_OrgInstName = bodyAccountDKQ.Training_OrgInstName;
            accountDKQ.Training_AttendanceDate = bodyAccountDKQ.Training_AttendanceDate;
            accountDKQ.Whatisderivative_BankDeposit = bodyAccountDKQ.Whatisderivative_BankDeposit;
            accountDKQ.Whatisderivative_StkOrWithOwnership = bodyAccountDKQ.Whatisderivative_StkOrWithOwnership;
            accountDKQ.Whatisderivative_Loan = bodyAccountDKQ.Whatisderivative_Loan;
            accountDKQ.Whatisderivative_FinInstFmUndAsset = bodyAccountDKQ.Whatisderivative_FinInstFmUndAsset;
            accountDKQ.DerRiskfactors_CptyRisk = bodyAccountDKQ.DerRiskfactors_CptyRisk;
            accountDKQ.DerRiskfactors_LiquidityRisk = bodyAccountDKQ.DerRiskfactors_LiquidityRisk;
            accountDKQ.DerRiskfactors_MarketRisk = bodyAccountDKQ.DerRiskfactors_MarketRisk;
            accountDKQ.DerRiskfactors_All = bodyAccountDKQ.DerRiskfactors_All;
            accountDKQ.TypesOfFutures_IndFut = bodyAccountDKQ.TypesOfFutures_IndFut;
            accountDKQ.TypesOfFutures_ComFut = bodyAccountDKQ.TypesOfFutures_ComFut;
            accountDKQ.TypesOfFutures_CcyFut = bodyAccountDKQ.TypesOfFutures_CcyFut;
            accountDKQ.TypesOfFutures_All = bodyAccountDKQ.TypesOfFutures_All;
            accountDKQ.IslvgeDer_Yes = bodyAccountDKQ.IslvgeDer_Yes;
            accountDKQ.IslvgeDev_No = bodyAccountDKQ.IslvgeDev_No;
            accountDKQ.LossInExcessInitMarginFunds_Yes = bodyAccountDKQ.LossInExcessInitMarginFunds_Yes;
            accountDKQ.LossInExcessInitMarginFunds_No = bodyAccountDKQ.LossInExcessInitMarginFunds_No;
            accountDKQ.Lastupdatedatetime = DateTime.UtcNow;
            accountDKQ.Lastupdateuserid = accountDKQ.Lastupdateuserid;
            accountDKQ.Version = 1;
            accountDKQ.Dbopr = "I";
            _context.Cms_account_dkqs.Add(accountDKQ);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created w8
            return accountDKQ;
        }


        [HttpPost("SaveW8")]
        public async Task<ActionResult> SaveW8([FromBody] body_cms_account_w8 bodyAccountW8)
        {
            string className = GetType().Name;
            string methodName = "SaveW8";
            Logger.LogErrorMessage(className, methodName, "", bodyAccountW8.ToString(), Logger.INFO);

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountW8.Accno);

            if (account == null)
                return null;

            // Validate the input
            if (bodyAccountW8 == null)
            {
                string errMsg = "Invalid W8 Form data.";
                Logger.LogErrorMessage(className, methodName, "", bodyAccountW8.ToString(), Logger.ERROR);
                return BadRequest(errMsg);
            }

            var accountW8 = await _context.Cms_account_w8s
                    .FirstOrDefaultAsync(q => q.Acc_id == account.Id);

            bodyAccountW8.Acc_id = account.Id;

            if (accountW8 == null)
            {
                accountW8 = await AddW8(bodyAccountW8);
                if (accountW8 == null)
                {
                    string errMsg = "Fail to create W8 Form.";
                    Logger.LogErrorMessage(className, methodName, "", bodyAccountW8.ToString(), Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceW8(bodyAccountW8))
                {
                    string errMsg = "Fail to replace W8 Form.";
                    Logger.LogErrorMessage(className, methodName, "", bodyAccountW8.ToString(), Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(accountW8);
        }

        private async Task<bool> ReplaceW8(body_cms_account_w8 bodyAccountW8)
        {
            if (bodyAccountW8 == null)
                return false;

            var accountW8 = await _context.Cms_account_w8s
                    .FirstOrDefaultAsync(q => q.Acc_id == bodyAccountW8.Acc_id);

            if (accountW8 == null)
                return false;

            // Set the Lastupdatedatetime to the current time
            accountW8.Acc_id = bodyAccountW8.Acc_id;
            accountW8.Name_of_bene_owner = bodyAccountW8.Name_of_bene_owner;
            accountW8.Country_of_citizenship = bodyAccountW8.Country_of_citizenship;
            accountW8.Residence_addr1 = bodyAccountW8.Residence_addr1;
            accountW8.Residence_addr2 = bodyAccountW8.Residence_addr2;
            accountW8.Residence_country = bodyAccountW8.Residence_country;
            accountW8.Mailing_addr1 = bodyAccountW8.Mailing_addr1;
            accountW8.Mailing_addr2 = bodyAccountW8.Mailing_addr2;
            accountW8.Mailing_country = bodyAccountW8.Mailing_country;
            accountW8.Foreign_tax_idnos = bodyAccountW8.Foreign_tax_idnos;
            accountW8.Us_taxpayer_idnos = bodyAccountW8.Us_taxpayer_idnos;
            accountW8.Ref_nos = bodyAccountW8.Ref_nos;
            accountW8.Chk_FTIN_not_legally_req = bodyAccountW8.Chk_FTIN_not_legally_req;
            accountW8.Dob = bodyAccountW8.Dob;
            accountW8.Treaty_claim_tax_country = bodyAccountW8.Treaty_claim_tax_country;
            accountW8.Treaty_spec_rate_and_condition = bodyAccountW8.Treaty_spec_rate_and_condition;
            accountW8.Treaty_percent = bodyAccountW8.Treaty_percent;
            accountW8.Treaty_withholding_on = bodyAccountW8.Treaty_withholding_on;
            accountW8.Treaty_explain = bodyAccountW8.Treaty_explain;
            accountW8.Certify = bodyAccountW8.Certify;
            accountW8.Lastupdatedatetime = DateTime.UtcNow;
            accountW8.Lastupdateuserid = accountW8.Lastupdateuserid;
            //accountW8.Version += 1;
            accountW8.Dbopr = "U";

            _context.Cms_account_w8s.Update(accountW8);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<cms_account_w8> AddW8(body_cms_account_w8 bodyAccountW8)
        {
            if (bodyAccountW8 == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyAccountW8.Accno);

            if (account == null)
                return null;

            // Add the new w8 to the context (account level)
            cms_account_w8 accountW8 = new cms_account_w8();

            // Set the Lastupdatedatetime to the current time
            accountW8.Acc_id = account.Id;
            accountW8.Name_of_bene_owner = bodyAccountW8.Name_of_bene_owner;
            accountW8.Country_of_citizenship = bodyAccountW8.Country_of_citizenship;
            accountW8.Residence_addr1 = bodyAccountW8.Residence_addr1;
            accountW8.Residence_addr2 = bodyAccountW8.Residence_addr2;
            accountW8.Residence_country = bodyAccountW8.Residence_country;
            accountW8.Mailing_addr1 = bodyAccountW8.Mailing_addr1;
            accountW8.Mailing_addr2 = bodyAccountW8.Mailing_addr2;
            accountW8.Mailing_country = bodyAccountW8.Mailing_country;
            accountW8.Foreign_tax_idnos = bodyAccountW8.Foreign_tax_idnos;
            accountW8.Us_taxpayer_idnos = bodyAccountW8.Us_taxpayer_idnos;
            accountW8.Ref_nos = bodyAccountW8.Ref_nos;
            accountW8.Chk_FTIN_not_legally_req = bodyAccountW8.Chk_FTIN_not_legally_req;
            accountW8.Dob = bodyAccountW8.Dob;
            accountW8.Treaty_claim_tax_country = bodyAccountW8.Treaty_claim_tax_country;
            accountW8.Treaty_spec_rate_and_condition = bodyAccountW8.Treaty_spec_rate_and_condition;
            accountW8.Treaty_percent = bodyAccountW8.Treaty_percent;
            accountW8.Treaty_withholding_on = bodyAccountW8.Treaty_withholding_on;
            accountW8.Treaty_explain = bodyAccountW8.Treaty_explain;
            accountW8.Certify = bodyAccountW8.Certify;
            accountW8.Lastupdatedatetime = DateTime.UtcNow;
            accountW8.Lastupdateuserid = accountW8.Lastupdateuserid;
            accountW8.Version = 1;
            accountW8.Dbopr = "I";

            _context.Cms_account_w8s.Add(accountW8);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created w8
            return accountW8;
        }


        [HttpPost("SaveBulkPhone")]
        public async Task<ActionResult> SaveBulkPhone([FromBody] body_cms_client_phone bodyClientPhone)
        {
            string className = GetType().Name;
            string methodName = "SaveBulkPhone";
            Logger.LogErrorMessage(className, methodName, "", bodyClientPhone.ToString(), Logger.INFO);

            int clientPhoneId = -1;
            //int clientId = -1;

            // Validate the input
            if (bodyClientPhone == null)
            {

                string errMsg = "Invalid phone data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientPhone = await _context.Cms_client_phones
                    .FirstOrDefaultAsync(q => q.Id == bodyClientPhone.Id);

            if (clientPhone == null)
            {
                clientPhone = await AddPhone(bodyClientPhone);
                if (clientPhone == null)
                {
                    string errMsg = $"Fail to create phone for account {bodyClientPhone.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientPhoneId = clientPhone.Id;
            }
            else
            {
                if (!await ReplacePhone(bodyClientPhone))
                {
                    string errMsg = $"Fail to replace phone for account {bodyClientPhone.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientPhoneId = bodyClientPhone.Id;
            }


            return Ok(clientPhone);
        }


        [HttpPost("SaveBulkPhone_backup")]
        public async Task<ActionResult> SaveBulkPhone_backup([FromBody] body_cms_client_phone bodyClientPhone)
        {
            string className = GetType().Name;
            string methodName = "SaveBulkPhone";
            Logger.LogErrorMessage(className, methodName, "", bodyClientPhone.ToString(), Logger.INFO);

            int clientPhoneId = -1;
            int clientId = -1;

            // Validate the input
            if (bodyClientPhone == null)
            {

                string errMsg = "Invalid phone data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientPhone = await _context.Cms_client_phones
                    .FirstOrDefaultAsync(q => q.Id == bodyClientPhone.Id);

            if (clientPhone == null)
            {
                clientPhone = await AddPhone(bodyClientPhone);
                if (clientPhone == null)
                {
                    string errMsg = $"Fail to create phone for account {bodyClientPhone.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientPhoneId = clientPhone.Id;
                clientId = clientPhone.Client_id;
            }
            else
            {
                if (!await ReplacePhone(bodyClientPhone))
                {
                    string errMsg = $"Fail to replace phone for account {bodyClientPhone.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientPhoneId = bodyClientPhone.Id;
                clientId = clientPhone.Client_id;
            }

            
            return Ok(clientPhone);


            /*  
             *  applying changes to all accounts with the same 9-digit account number. Margin, Stock Options, and Futures account information under the same account will be updated accordingly (if applicable).
             *  e.g. changes apply to 116333-001, 116333-001M, 116333-001S, and 116333-001F.
             */
            string original_accno = bodyClientPhone.Accno;
            string accno = original_accno;

            int pos = accno.IndexOf("-");
            if (pos > 0)
            {
                string accTypeStr = accno.Substring(pos + 1, UOBCMS.Common.Constant.LEN_ACCTYPENO);
                if (accTypeStr == UOBCMS.Common.Constant.BULK_ACCTYPENO)
                {
                    var account_list = await _context.Cms_accounts
                        .Where(c => c.AccNo.Contains(accno.Substring(0, 10))).ToListAsync();

                    if (account_list != null)
                    {
                        foreach (cms_account account in account_list)
                        {
                            if (account.AccNo != original_accno)
                            {
                                bodyClientPhone.Accno = account.AccNo;

                                var client_account = await _context.Cms_client_accounts
                                                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);


                                if (client_account != null)
                                {
                                    List<cms_client_phone> client_phone_list = null;
                                    client_phone_list = await _context.Cms_client_phones
                                            .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientPhone.Type &&
                                            c.Value == bodyClientPhone.Value && c.Area_code == bodyClientPhone.Area_code &&
                                            c.Country_code == bodyClientPhone.Country_code).ToListAsync();

                                    bool replaced = false;

                                    foreach (cms_client_phone client_phone in client_phone_list)
                                    {
                                        var account_phone = await _context.Cms_account_phones
                                                .FirstOrDefaultAsync(c => c.Acc_id == account.Id && c.Client_phone_id == client_phone.Id);

                                        if (account_phone != null)
                                        {
                                            replaced = true;
                                        }
                                    }

                                    if (!replaced)
                                    {
                                        // remove the original Tel Type if exist
                                        var account_phone_list = await _context.Cms_account_phones
                                                    .Include(c => c.Cms_client_phone)
                                                    .Where(q => q.Acc_id == account.Id && q.Cms_client_phone.Type == bodyClientPhone.Type).ToListAsync();

                                        foreach (var account_phone in account_phone_list)
                                        {
                                            account_phone.Dbopr = "D";
                                            account_phone.Lastupdateuserid = clientPhone.Lastupdateuserid;
                                            account_phone.Lastupdatedatetime = clientPhone.Lastupdatedatetime;
                                        }

                                        cms_account_phone accountPhone = new cms_account_phone();
                                        accountPhone.Acc_id = account.Id;
                                        accountPhone.Client_phone_id = clientPhoneId;
                                        accountPhone.Lastupdateuserid = clientPhone.Lastupdateuserid;
                                        accountPhone.Lastupdatedatetime = clientPhone.Lastupdatedatetime;
                                        accountPhone.Version = 1;
                                        accountPhone.Dbopr = "I";

                                        _context.Cms_account_phones.Add(accountPhone);

                                        // Save changes to the database
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Ok(clientPhone);
        }

        [HttpPost("SavePhone")]
        public async Task<ActionResult> SavePhone([FromBody] body_cms_client_phone bodyClientPhone)
        {
            string className = GetType().Name;
            string methodName = "SavePhone";
            Logger.LogErrorMessage(className, methodName, "", bodyClientPhone.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientPhone == null)
            {
                string errMsg = "Invalid phone data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientPhone = await _context.Cms_client_phones
                    .FirstOrDefaultAsync(q => q.Id == bodyClientPhone.Id);

            if (clientPhone == null)
            {
                clientPhone = await AddPhone(bodyClientPhone);
                if (clientPhone == null)
                {
                    string errMsg = "Fail to create phone.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplacePhone(bodyClientPhone))
                {
                    string errMsg = "Fail to replace phone.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(clientPhone);
        }

        private async Task<bool> ReplacePhone(body_cms_client_phone bodyClientPhone)
        {
            if (bodyClientPhone == null)
                return false;

            var clientPhone = await _context.Cms_client_phones
                    .FirstOrDefaultAsync(q => q.Id == bodyClientPhone.Id);

            if (clientPhone == null)
                return false;

            cms_account account = await _context.Cms_accounts
                .Include(c => c.Cms_client_account)
                .FirstOrDefaultAsync(q => q.AccNo == bodyClientPhone.Accno);


            if (account == null)
                return false;

            // new to create a separate link with a new client_phone if the client_email is used by other accounts
            var accountPhone = await _context.Cms_account_phones
                .FirstOrDefaultAsync(q => q.Client_phone_id == bodyClientPhone.Id && q.Acc_id != account.Id);

            if (accountPhone == null)
            {
                // Set the Lastupdatedatetime to the current time
                clientPhone.Type = bodyClientPhone.Type;
                clientPhone.Sub_type = bodyClientPhone.Sub_type;
                clientPhone.Country_code = bodyClientPhone.Country_code;
                clientPhone.Area_code = bodyClientPhone.Area_code;
                clientPhone.Value = bodyClientPhone.Value;
                clientPhone.Lastupdatedatetime = bodyClientPhone.Lastupdatedatetime;
                clientPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
                clientPhone.Dbopr = "U";

                _context.Cms_client_phones.Update(clientPhone);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                cms_client_phone new_clientPhone = new cms_client_phone();
                new_clientPhone.Client_id = account.Cms_client_account.Client_id;
                new_clientPhone.Type = bodyClientPhone.Type;
                new_clientPhone.Sub_type = bodyClientPhone.Sub_type;
                new_clientPhone.Country_code = bodyClientPhone.Country_code;
                new_clientPhone.Area_code = bodyClientPhone.Area_code;
                new_clientPhone.Value = bodyClientPhone.Value;
                new_clientPhone.Lastupdatedatetime = bodyClientPhone.Lastupdatedatetime;
                new_clientPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
                new_clientPhone.Version = 1;
                new_clientPhone.Dbopr = "I";
                _context.Cms_client_phones.Add(new_clientPhone);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // remove the original link
                var old_account_phone_list = await _context.Cms_account_phones
                                                .Include(c => c.Cms_client_phone)
                                                .Where(q => q.Acc_id == account.Id && q.Cms_client_phone.Type == bodyClientPhone.Type).ToListAsync();

                foreach (cms_account_phone old_account_phone in old_account_phone_list)
                {
                    old_account_phone.Lastupdatedatetime = bodyClientPhone.Lastupdatedatetime;
                    old_account_phone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
                    old_account_phone.Version += 1;
                    old_account_phone.Dbopr = "D";
                }


                // create the link to the new client_phone
                cms_account_phone new_accountPhone = new cms_account_phone();
                new_accountPhone.Acc_id = account.Id;
                new_accountPhone.Client_phone_id = new_clientPhone.Id;
                new_accountPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
                new_accountPhone.Lastupdatedatetime = bodyClientPhone.Lastupdatedatetime;
                new_accountPhone.Version = 1;
                new_accountPhone.Dbopr = "I";

                _context.Cms_account_phones.Add(new_accountPhone);

                // Save changes to the database
                await _context.SaveChangesAsync();



                // Set the Lastupdatedatetime to the current time
                /*clientPhone.Type = bodyClientPhone.Type;
                clientPhone.Sub_type = bodyClientPhone.Sub_type;
                clientPhone.Country_code = bodyClientPhone.Country_code;
                clientPhone.Area_code = bodyClientPhone.Area_code;
                clientPhone.Value = bodyClientPhone.Value;
                clientPhone.Lastupdatedatetime = DateTime.UtcNow;
                clientPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
                clientPhone.Dbopr = "U";

                _context.Cms_client_phones.Update(clientPhone);

                // Save changes to the database
                await _context.SaveChangesAsync();*/
            }

            return true;
        }

        private async Task<bool> ReplacePhone_backup(body_cms_client_phone bodyClientPhone)
        {
            if (bodyClientPhone == null)
                return false;

            var clientPhone = await _context.Cms_client_phones
                    .FirstOrDefaultAsync(q => q.Id == bodyClientPhone.Id);

            if (clientPhone == null)
                return false;

            // Set the Lastupdatedatetime to the current time
            clientPhone.Type = bodyClientPhone.Type;
            clientPhone.Sub_type = bodyClientPhone.Sub_type;
            clientPhone.Country_code = bodyClientPhone.Country_code;
            clientPhone.Area_code = bodyClientPhone.Area_code;
            clientPhone.Value = bodyClientPhone.Value;
            clientPhone.Lastupdatedatetime = DateTime.UtcNow;
            //clientPhone.Lastupdateuserid = "louis";
            clientPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
            //clientPhone.Version += 1;
            clientPhone.Dbopr = "U";

            _context.Cms_client_phones.Update(clientPhone);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<cms_client_phone> AddPhone(body_cms_client_phone bodyClientPhone)
        {
            if (bodyClientPhone == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyClientPhone.Accno);

            if (account == null)
                return null;

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
                return null;

            // Add the new phone to the context (client level)
            cms_client_phone clientPhone = new cms_client_phone();

            // Set the Lastupdatedatetime to the current time
            clientPhone.Type = bodyClientPhone.Type;
            clientPhone.Sub_type = bodyClientPhone.Sub_type;
            clientPhone.Client_id = client_account.Client_id;
            clientPhone.Country_code = bodyClientPhone.Country_code;
            clientPhone.Area_code = bodyClientPhone.Area_code;
            clientPhone.Value = bodyClientPhone.Value;
            clientPhone.Lastupdatedatetime = DateTime.UtcNow;
            //clientPhone.Lastupdateuserid = "louis";
            clientPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
            clientPhone.Version = 1;
            clientPhone.Dbopr = "I";

            _context.Cms_client_phones.Add(clientPhone);

            // Save changes to the database
            await _context.SaveChangesAsync();


            // Add to account level
            cms_account_phone accountPhone = new cms_account_phone();
            accountPhone.Acc_id = account.Id;
            accountPhone.Client_phone_id = clientPhone.Id;
            //accountPhone.Lastupdateuserid = "louis";
            accountPhone.Lastupdateuserid = bodyClientPhone.Lastupdateuserid;
            accountPhone.Lastupdatedatetime = clientPhone.Lastupdatedatetime;
            accountPhone.Version = 1;
            accountPhone.Dbopr = "I";

            _context.Cms_account_phones.Add(accountPhone);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created email
            return clientPhone;
        }

        [HttpPost("SaveEmailByType")]
        public async Task<ActionResult> SaveEmailByType([FromBody] body_cms_client_email bodyClientEmail)
        {
            string className = GetType().Name;
            string methodName = "SaveEmailByType";
            Logger.LogErrorMessage(className, methodName, "", bodyClientEmail.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientEmail == null)
            {
                string errMsg = "Invalid email data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var account = await _context.Cms_accounts
                .Include(c => c.Cms_client_account)
                .FirstOrDefaultAsync(q => q.AccNo == bodyClientEmail.Accno);

            if (account == null)
            {
                string errMsg = "Invalid trading account number.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            bodyClientEmail.Acc_id = account.Id;

            var clientEmail = await _context.Cms_client_emails
                    .FirstOrDefaultAsync(q => (q.Client_id == account.Cms_client_account.Client_id && q.Type == bodyClientEmail.Type));

            if (clientEmail == null)
            {
                clientEmail = await AddEmail(bodyClientEmail);
                if (clientEmail == null)
                {
                    string errMsg = "Invalid trading account number.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                bodyClientEmail.Id = clientEmail.Id;
                if (!await ReplaceEmail(bodyClientEmail))
                {
                    string errMsg = "Fail to replace email.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(clientEmail);
        }

        [HttpPost("SaveBulkEmail")]
        public async Task<ActionResult> SaveBulkEmail([FromBody] body_cms_client_email bodyClientEmail)
        {
            string className = GetType().Name;
            string methodName = "SaveBulkEmail";
            Logger.LogErrorMessage(className, methodName, "", bodyClientEmail.ToString(), Logger.INFO);

            int clientEmailId = -1;

            // Validate the input
            if (bodyClientEmail == null)
            {
                string errMsg = "Invalid email data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientEmail = await _context.Cms_client_emails
                    .FirstOrDefaultAsync(q => q.Id == bodyClientEmail.Id);

            if (clientEmail == null)
            {
                clientEmail = await AddEmail(bodyClientEmail);
                if (clientEmail == null)
                {
                    string errMsg = "Fail to create email for account {bodyClientEmail.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientEmailId = clientEmail.Id;
            }
            else
            {
                if (!await ReplaceEmail(bodyClientEmail))
                {
                    string errMsg = "Fail to replace email for account {bodyClientEmail.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientEmailId = bodyClientEmail.Id;
            }

            if (bodyClientEmail.applyToAllAcc)
            {
                /*  
                 *  applying changes to all accounts with the same 9-digit account number. Margin, Stock Options, and Futures account information under the same account will be updated accordingly (if applicable).
                 *  e.g. changes apply to 116333-001, 116333-001M, 116333-001S, and 116333-001F.
                 */
                string original_accno = bodyClientEmail.Accno;
                string accno = original_accno;

                int pos = accno.IndexOf("-");
                if (pos > 0)
                {
                    string accTypeStr = accno.Substring(pos + 1, UOBCMS.Common.Constant.LEN_ACCTYPENO);
                    if (accTypeStr == UOBCMS.Common.Constant.BULK_ACCTYPENO)
                    {
                        var account_list = await _context.Cms_accounts
                            .Where(c => c.AccNo.Contains(accno.Substring(0, 10))).ToListAsync();

                        if (account_list != null)
                        {
                            foreach (cms_account account in account_list)
                            {
                                if (account.AccNo != original_accno)
                                {
                                    bodyClientEmail.Accno = account.AccNo;

                                    var client_account = await _context.Cms_client_accounts
                                                    .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

                                    if (client_account != null)
                                    {
                                        List<cms_client_email> client_email_list = null;
                                        client_email_list = await _context.Cms_client_emails
                                                .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientEmail.Type &&
                                                c.Email == bodyClientEmail.Email).ToListAsync();

                                        bool replaced = false;

                                        foreach (cms_client_email client_email in client_email_list)
                                        {
                                            var account_email = await _context.Cms_account_emails
                                                    .FirstOrDefaultAsync(c => c.Acc_id == account.Id && c.Client_email_id == client_email.Id);

                                            if (account_email != null)
                                            {
                                                // already linked to the client email with details have been updated by the original account
                                                replaced = true;
                                            }
                                        }

                                        if (!replaced)
                                        {
                                            // if account is not link to the same client email, remove the original link which linked to a different client email
                                            var account_email_list = await _context.Cms_account_emails
                                                    .Include(c => c.Cms_client_email)
                                                    .Where(q => q.Acc_id == account.Id && q.Cms_client_email.Type == bodyClientEmail.Type).ToListAsync();

                                            foreach (cms_account_email account_email in account_email_list)
                                            {
                                                List<cms_client_email> account_client_email_list = null;
                                                account_client_email_list = await _context.Cms_client_emails
                                                        .Where(c => c.Id == account_email.Client_email_id && c.Type == bodyClientEmail.Type).ToListAsync();

                                                foreach (cms_client_email account_client_email in account_client_email_list)
                                                {
                                                    account_email.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                                                    account_email.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                                                    account_email.Version += 1;
                                                    account_email.Dbopr = "D";
                                                }
                                            }

                                            // then create the link to link to the same client email with the original account
                                            cms_account_email accountEmail = new cms_account_email();
                                            accountEmail.Acc_id = account.Id;
                                            accountEmail.Client_email_id = clientEmailId;
                                            accountEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                                            accountEmail.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                                            accountEmail.Version = 1;
                                            accountEmail.Dbopr = "I";

                                            _context.Cms_account_emails.Add(accountEmail);


                                            // Save changes to the database
                                            await _context.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Ok(clientEmail);
        }

        [HttpPost("SaveEmail")]
        public async Task<ActionResult> SaveEmail([FromBody] body_cms_client_email bodyClientEmail)
        {
            string className = GetType().Name;
            string methodName = "SaveEmail";
            Logger.LogErrorMessage(className, methodName, "", bodyClientEmail.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientEmail == null)
            {
                string errMsg = "Invalid email data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientEmail = await _context.Cms_client_emails
                    .FirstOrDefaultAsync(q => q.Id == bodyClientEmail.Id);

            if (clientEmail == null)
            {
                clientEmail = await AddEmail(bodyClientEmail);
                if (clientEmail == null)
                {
                    string errMsg = "Fail to create email.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceEmail(bodyClientEmail))
                {
                    string errMsg = "Fail to replace email.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            // Return a success response with the newly created address
            //return CreatedAtAction(nameof(SaveEmail), new { accno = clientEmail.Client_id }, clientEmail);
            return Ok(clientEmail);
        }

        private async Task<bool> ReplaceEmail(body_cms_client_email bodyClientEmail)
        {
            if (bodyClientEmail == null)
                return false;

            var clientEmail = await _context.Cms_client_emails
                    .FirstOrDefaultAsync(q => q.Id == bodyClientEmail.Id);

            if (clientEmail == null)
                return false;

            if (bodyClientEmail.applyToAllAcc)
            {
                // Set the Lastupdatedatetime to the current time
                clientEmail.Type = bodyClientEmail.Type;
                clientEmail.Email = bodyClientEmail.Email;
                clientEmail.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                clientEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                //clientEmail.Version += 1;
                clientEmail.Dbopr = "U";

                _context.Cms_client_emails.Update(clientEmail);

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            else
            {
                cms_account account = await _context.Cms_accounts
                .Include(c => c.Cms_client_account)
                .FirstOrDefaultAsync(q => q.AccNo == bodyClientEmail.Accno);


                if (account == null)
                    return false;

                // new to create a separate link with a new client_email if the client_email is used by other accounts
                var accountEmail = await _context.Cms_account_emails
                    .FirstOrDefaultAsync(q => q.Client_email_id == bodyClientEmail.Id && q.Acc_id != account.Id);

                if (accountEmail == null)
                {
                    // Set the Lastupdatedatetime to the current time
                    clientEmail.Type = bodyClientEmail.Type;
                    clientEmail.Email = bodyClientEmail.Email;
                    clientEmail.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                    clientEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                    //clientEmail.Version += 1;
                    clientEmail.Dbopr = "U";

                    _context.Cms_client_emails.Update(clientEmail);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cms_client_email new_clientEmail = new cms_client_email();
                    new_clientEmail.Client_id = account.Cms_client_account.Client_id;
                    new_clientEmail.Type = bodyClientEmail.Type;
                    new_clientEmail.Email = bodyClientEmail.Email;
                    new_clientEmail.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                    new_clientEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                    new_clientEmail.Version = 1;
                    new_clientEmail.Dbopr = "I";
                    _context.Cms_client_emails.Add(new_clientEmail);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // remove the original link
                    var old_account_email_list = await _context.Cms_account_emails
                                                    .Include(c => c.Cms_client_email)
                                                    .Where(q => q.Acc_id == account.Id && q.Cms_client_email.Type == bodyClientEmail.Type).ToListAsync();

                    foreach (cms_account_email old_account_email in old_account_email_list)
                    {
                        old_account_email.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                        old_account_email.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                        old_account_email.Version += 1;
                        old_account_email.Dbopr = "D";
                    }


                    // create the link to the new client_email
                    cms_account_email new_accountEmail = new cms_account_email();
                    new_accountEmail.Acc_id = account.Id;
                    new_accountEmail.Client_email_id = new_clientEmail.Id;
                    new_accountEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
                    new_accountEmail.Lastupdatedatetime = bodyClientEmail.Lastupdatedatetime;
                    new_accountEmail.Version = 1;
                    new_accountEmail.Dbopr = "I";

                    _context.Cms_account_emails.Add(new_accountEmail);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
            }

            return true;
        }

        private async Task<cms_client_email> AddEmail(body_cms_client_email bodyClientEmail)
        {
            if (bodyClientEmail == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyClientEmail.Accno);

            if (account == null)
                return null;

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
                return null;

            // Add the new email to the context (client level)
            cms_client_email clientEmail = new cms_client_email();

            // Set the Lastupdatedatetime to the current time
            clientEmail.Type = bodyClientEmail.Type;
            clientEmail.Client_id = client_account.Client_id;
            clientEmail.Email = bodyClientEmail.Email;
            clientEmail.Lastupdatedatetime = DateTime.UtcNow;
            clientEmail.Lastupdateuserid = bodyClientEmail.Lastupdateuserid;
            clientEmail.Version = 1;
            clientEmail.Dbopr = "I";

            _context.Cms_client_emails.Add(clientEmail);

            // Save changes to the database
            await _context.SaveChangesAsync();


            // Add to account level
            cms_account_email accountEmail = new cms_account_email();
            accountEmail.Acc_id = account.Id;
            accountEmail.Client_email_id = clientEmail.Id;
            accountEmail.Lastupdateuserid = clientEmail.Lastupdateuserid;
            accountEmail.Lastupdatedatetime = clientEmail.Lastupdatedatetime;
            accountEmail.Version = 1;
            accountEmail.Dbopr = "I";

            _context.Cms_account_emails.Add(accountEmail);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created email
            return clientEmail;
        }


        [HttpPost("SaveBulkBank")]
        public async Task<ActionResult> SaveBulkBank([FromBody] body_cms_client_bank bodyClientBank)
        {
            string className = GetType().Name;
            string methodName = "SaveBulkBank";
            Logger.LogErrorMessage(className, methodName, "", bodyClientBank.ToString(), Logger.INFO);

            // Validate the input
            int clientBankId = -1;

            if (bodyClientBank == null)
            {
                return BadRequest("Invalid bank data.");
            }

            var clientBank = await _context.Cms_client_banks
                    .FirstOrDefaultAsync(q => q.Id == bodyClientBank.Id && q.Status != "V");

            if (clientBank == null)
            {
                clientBank = await AddBank(bodyClientBank);
                if (clientBank == null)
                {
                    string errMsg = $"Fail to create bank account for account {bodyClientBank.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientBankId = clientBank.Id;
            }
            else
            {
                if (!await ReplaceBank(bodyClientBank))
                {
                    string errMsg = $"Fail to replace bank account for account {bodyClientBank.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientBankId = bodyClientBank.Id;
            }

            // if this bank is default local hkd bank, other banks cannot be the default
            if (bodyClientBank.DefaultLocalHKDBank == "Y")
            {
                var other_clientBanks = await _context.Cms_client_banks
                    .Where(q => q.Id != clientBankId && q.Status != "V" && q.Client_id == clientBank.Client_id)
                    .ToListAsync();

                foreach (cms_client_bank other_client_bank in other_clientBanks)
                {
                    var acc_banks = await _context.Cms_account_banks
                        .Where(q => q.Client_bank_id == other_client_bank.Id)
                        .ToListAsync();

                    foreach (cms_account_bank acc_bank in acc_banks)
                    {
                        if (acc_bank.DefaultLocalHKDBank == "Y")
                        {
                            acc_bank.DefaultLocalHKDBank = "N";
                            _context.Cms_account_banks.Update(acc_bank);
                        }
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }

            /*  
             *  applying changes to all accounts with the same 9-digit account number. Margin, Stock Options, and Futures account information under the same account will be updated accordingly (if applicable).
             *  e.g. changes apply to 116333-001, 116333-001M, 116333-001S, and 116333-001F.
             */
            string original_accno = bodyClientBank.Accno;
            string accno = original_accno;

            int pos = accno.IndexOf("-");
            if (pos > 0)
            {
                string accTypeStr = accno.Substring(pos + 1, UOBCMS.Common.Constant.LEN_ACCTYPENO);
                if (accTypeStr == UOBCMS.Common.Constant.BULK_ACCTYPENO)
                {
                    var account_list = await _context.Cms_accounts
                        .Where(c => c.AccNo.Contains(accno.Substring(0, 10))).ToListAsync();

                    if (account_list != null)
                    {
                        foreach (cms_account account in account_list)
                        {
                            if (account.AccNo != original_accno)
                            {
                                bodyClientBank.Accno = account.AccNo;

                                var client_account = await _context.Cms_client_accounts
                                                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);


                                if (client_account != null)
                                {
                                    //var client_bank_list = await _context.Cms_client_banks
                                    //    .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientBank.Bank_type && c.Status != "V" && c.Id == bodyClientBank.Id).ToListAsync();

                                    List<cms_client_bank> client_bank_list = null;
                                    if (bodyClientBank.Bank_type == UOBCMS.Common.Constant.LOCAL_BANK)
                                    {
                                        client_bank_list = await _context.Cms_client_banks
                                            .Include(cb => cb.Cms_bank) // Include the associated bank details
                                            .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientBank.Bank_type &&
                                            c.Status != "V" && c.Bank_accno == bodyClientBank.Bank_accno && c.Cms_bank.Bank_code == bodyClientBank.Bank_code).ToListAsync();
                                    }
                                    else if (bodyClientBank.Bank_type == UOBCMS.Common.Constant.OVERSEA_BANK)
                                    {
                                        client_bank_list = await _context.Cms_client_banks
                                            .Include(cb => cb.Cms_bank) // Include the associated bank details
                                            .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientBank.Bank_type &&
                                            c.Status != "V" && c.Bank_accno == bodyClientBank.Bank_accno && c.Cms_bank.Bank_code == bodyClientBank.Bank_code).ToListAsync();
                                    }

                                    bool replaced = false;

                                    foreach (cms_client_bank client_bank in client_bank_list)
                                    {
                                        var account_bank = await _context.Cms_account_banks
                                                .FirstOrDefaultAsync(c => c.Acc_id == account.Id && c.Client_bank_id == client_bank.Id);

                                        if (account_bank != null)
                                        {
                                            // if the account has have the bank already, just simply done nothing
                                            replaced = true;
                                        }
                                        /*if (account_bank != null)
                                        {
                                            bodyClientBank.Id = client_bank.Id;
                                            if (!await ReplaceBank(bodyClientBank))
                                                return BadRequest($"Fail to replace bank account for account {bodyClientBank.Accno}.");

                                            replaced = true;
                                        }*/
                                    }

                                    if (!replaced)
                                    {
                                        /*bodyClientBank.Id = -1;
                                        cms_client_bank client_bank = await AddBank(bodyClientBank);
                                        if (client_bank == null)
                                            return BadRequest($"Fail to create bank account for account {bodyClientBank.Accno}.");
                                        */

                                        // if the account does not have the bank, just simply add the bank to that account
                                        cms_account_bank accountBank = new cms_account_bank();
                                        accountBank.Acc_id = account.Id;
                                        accountBank.Client_bank_id = clientBankId;
                                        accountBank.DefaultLocalHKDBank = bodyClientBank.DefaultLocalHKDBank;
                                        accountBank.Lastupdateuserid = accountBank.Lastupdateuserid;
                                        accountBank.Lastupdatedatetime = clientBank.Lastupdatedatetime;
                                        accountBank.Version = 1;
                                        accountBank.Dbopr = "I";

                                        _context.Cms_account_banks.Add(accountBank);

                                        // Save changes to the database
                                        await _context.SaveChangesAsync();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Ok(clientBank);
        }


        [HttpPost("SaveBank")]
        public async Task<ActionResult> SaveBank([FromBody] body_cms_client_bank bodyClientBank)
        {
            string className = GetType().Name;
            string methodName = "SaveBank";
            Logger.LogErrorMessage(className, methodName, "", bodyClientBank.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientBank == null)
            {
                string errMsg = "Invalid bank data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientBank = await _context.Cms_client_banks
                    .FirstOrDefaultAsync(q => q.Id == bodyClientBank.Id && q.Status != "V");

            if (clientBank == null)
            {
                clientBank = await AddBank(bodyClientBank);
                if (clientBank == null)
                {
                    string errMsg = "Fail to create bank.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceBank(bodyClientBank))
                {
                    string errMsg = "Fail to replace bank.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            return Ok(clientBank);
        }

        [HttpPost("VoidOverseaBank")]
        public async Task<ActionResult> VoidOverseaBank([FromBody] body_cms_client_bank bodyClientBank)
        {
            string className = GetType().Name;
            string methodName = "VoidOverseaBank";
            Logger.LogErrorMessage(className, methodName, "", bodyClientBank.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientBank == null)
            {
                string errMsg = "Invalid bank data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientBank = await _context.Cms_client_banks
                    .FirstOrDefaultAsync(q => q.Id == bodyClientBank.Id && q.Status == "A");

            if (clientBank == null)
            {
                string errMsg = "Fail to void bank.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            /*var bank = await _context.Cms_banks
                    .FirstOrDefaultAsync(q => q.Id == clientBank.Bank_id);

            if (bank == null)
                return BadRequest("Fail to void bank.");

            if (bank.Type != UOBCMS.Common.Constant.OVERSEA_BANK)
                return BadRequest("The bank is not a oversea bank, fail to void this bank.");*/

            clientBank.Status = "V";
            clientBank.Lastupdatedatetime = DateTime.UtcNow;
            clientBank.Lastupdateuserid = clientBank.Lastupdateuserid;
            //clientBank.Version += 1;
            clientBank.Dbopr = "U";

            _context.Cms_client_banks.Update(clientBank);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(clientBank);
        }

        [HttpPost("VoidLocalBank")]
        public async Task<ActionResult> VoidLocalBank([FromBody] body_cms_client_bank bodyClientBank)
        {
            string className = GetType().Name;
            string methodName = "VoidLocalBank";
            Logger.LogErrorMessage(className, methodName, "", bodyClientBank.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientBank == null)
            {
                string errMsg = "Invalid bank data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientBank = await _context.Cms_client_banks
                    .FirstOrDefaultAsync(q => q.Id == bodyClientBank.Id && q.Status == "A");

            if (clientBank == null)
            {
                string errMsg = "Fail to void bank.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var bank = await _context.Cms_banks
                    .FirstOrDefaultAsync(q => q.Id == clientBank.Bank_id);

            if (bank == null)
            {
                string errMsg = "Fail to void bank.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            if (bank.Type != UOBCMS.Common.Constant.LOCAL_BANK)
            {
                string errMsg = "The bank is not a local bank, fail to void this bank.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            clientBank.Status = "V";
            clientBank.Lastupdatedatetime = DateTime.UtcNow;
            clientBank.Lastupdateuserid = clientBank.Lastupdateuserid;
            //clientBank.Version += 1;
            clientBank.Dbopr = "U";

            _context.Cms_client_banks.Update(clientBank);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(clientBank);
        }

        private async Task<bool> ReplaceBank(body_cms_client_bank bodyClientBank)
        {
            if (bodyClientBank == null)
                return false;

            var clientBank = await _context.Cms_client_banks
                    .FirstOrDefaultAsync(q => q.Id == bodyClientBank.Id);

            if (clientBank == null)
                return false;

            if (bodyClientBank.Bank_code.TrimEnd() != "" || bodyClientBank.Bene_bank_name.TrimEnd() != "")
            {
                if (bodyClientBank.Bank_code.TrimEnd() != "")
                {
                    var bank = await _context.Cms_banks
                            .FirstOrDefaultAsync(q => q.Bank_code == bodyClientBank.Bank_code);

                    if (bank == null)
                        return false;

                    // Set the Lastupdatedatetime to the current time
                    clientBank.Id = bodyClientBank.Id;
                    clientBank.Bank_id = bank.Id;
                    clientBank.Ccy = bodyClientBank.Ccy;
                    clientBank.Bank_accno = bodyClientBank.Bank_accno;
                    clientBank.Bank_accname = bodyClientBank.Bank_accname;
                    clientBank.Other_bank_details = bodyClientBank.Other_bank_details;
                    clientBank.Type = bodyClientBank.Bank_type;
                    clientBank.Default_payment_type = bodyClientBank.Default_payment_type;
                    clientBank.Bene_bank_country = bodyClientBank.Bene_bank_country;
                    clientBank.Corr_bank_country = bodyClientBank.Corr_bank_country;
                    clientBank.Bene_bank_code = bodyClientBank.Bene_bank_code;
                    clientBank.Corr_bank_code = bodyClientBank.Corr_bank_code;
                    clientBank.Msg_to_bank = bodyClientBank.Msg_to_bank;

                    if (bodyClientBank.Default_payment_type == UOBCMS.Common.Constant.FPS)
                    {
                        if (bodyClientBank.Fps_payment_type == "" && (clientBank.Fps_payment_type == null || clientBank.Fps_payment_type == " "))
                            clientBank.Fps_payment_type = UOBCMS.Common.Constant.FPS_PTA; // default PTA
                        else if (bodyClientBank.Fps_payment_type != "")
                            clientBank.Fps_payment_type = bodyClientBank.Fps_payment_type; // replace the fps_payment_type
                    }
                    else
                    {
                        clientBank.Fps_payment_type = " ";
                    }

                    // don't replace original Fps_payment_type if bodyClientBank.Fps_payment_type == null and clientBank.Fps_payment_type != null
                }
                else
                {
                    clientBank.Id = bodyClientBank.Id;
                    clientBank.Bene_bank_swift = bodyClientBank.Bene_bank_swift;
                    clientBank.Bene_bank_address = bodyClientBank.Bene_bank_address;
                    clientBank.Bene_bank_accname = bodyClientBank.Bene_bank_accname;
                    clientBank.Bene_bank_name = bodyClientBank.Bene_bank_name;
                    clientBank.Corr_bank_name = bodyClientBank.Corr_bank_name;
                    clientBank.Corr_bank_swift = bodyClientBank.Corr_bank_swift;
                    clientBank.Bank_accno = bodyClientBank.Bene_bank_accno;
                    clientBank.Ccy = bodyClientBank.Bene_ccy;
                    clientBank.Type = bodyClientBank.Bank_type;
                    clientBank.Default_payment_type = bodyClientBank.Default_payment_type;
                    clientBank.Fps_payment_type = " ";
                    clientBank.Bene_bank_country = bodyClientBank.Bene_bank_country;
                    clientBank.Corr_bank_country = bodyClientBank.Corr_bank_country;
                    clientBank.Bene_bank_code = bodyClientBank.Bene_bank_code;
                    clientBank.Corr_bank_code = bodyClientBank.Corr_bank_code;
                    clientBank.Msg_to_bank = bodyClientBank.Msg_to_bank;
                }

                clientBank.Lastupdatedatetime = DateTime.UtcNow;
                clientBank.Lastupdateuserid = clientBank.Lastupdateuserid;
                //clientBank.Version += 1;
                clientBank.Dbopr = "U";

                _context.Cms_client_banks.Update(clientBank);

                var account = await _context.Cms_accounts
                    .FirstOrDefaultAsync(c => c.AccNo == bodyClientBank.Accno);

                if (account == null)
                    return false;

                var accountBank_list = await _context.Cms_account_banks
                       .Where(c => c.Client_bank_id == clientBank.Id).ToListAsync();

                if (accountBank_list == null)
                    return false;

                foreach (cms_account_bank accountBank in accountBank_list)
                {
                    if (accountBank.DefaultLocalHKDBank != bodyClientBank.DefaultLocalHKDBank)
                    {
                        accountBank.DefaultLocalHKDBank = bodyClientBank.DefaultLocalHKDBank;
                        _context.Cms_account_banks.Update(accountBank);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return true;
            }
            else
                return false;
        }

        private async Task<cms_client_bank> AddBank(body_cms_client_bank bodyClientBank)
        {
            if (bodyClientBank == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyClientBank.Accno);

            if (account == null)
                return null;

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
                return null;

            if (bodyClientBank.Bank_code.TrimEnd() != "" || bodyClientBank.Bene_bank_name.TrimEnd() != "")
            {
                // Add the new bank to the context (client level)
                cms_client_bank clientBank = new cms_client_bank();

                //if (bodyClientBank.Bank_code.TrimEnd() != "")
                if (bodyClientBank.Bank_type == UOBCMS.Common.Constant.LOCAL_BANK)
                {
                    var bank = await _context.Cms_banks
                        .FirstOrDefaultAsync(q => q.Bank_code == bodyClientBank.Bank_code);

                    if (bank == null)
                        return null;

                    // Set the Lastupdatedatetime to the current time
                    clientBank.Client_id = client_account.Client_id;
                    clientBank.Bank_id = bank.Id;
                    clientBank.Ccy = bodyClientBank.Ccy;
                    clientBank.Bank_accno = bodyClientBank.Bank_accno;
                    clientBank.Bank_accname = bodyClientBank.Bank_accname;
                    clientBank.Other_bank_details = bodyClientBank.Other_bank_details;
                    clientBank.Type = bodyClientBank.Bank_type;
                    clientBank.Default_payment_type = bank.Default_payment_type;
                    clientBank.Bene_bank_country = bodyClientBank.Bene_bank_country;
                    clientBank.Corr_bank_country = bodyClientBank.Corr_bank_country;
                    clientBank.Bene_bank_code = bodyClientBank.Bene_bank_code;
                    clientBank.Corr_bank_code = bodyClientBank.Corr_bank_code;
                    clientBank.Msg_to_bank = bodyClientBank.Msg_to_bank;

                    if (bodyClientBank.Default_payment_type == UOBCMS.Common.Constant.FPS)
                    {
                        if (bodyClientBank.Fps_payment_type == "" && (clientBank.Fps_payment_type == null || clientBank.Fps_payment_type == " "))
                            clientBank.Fps_payment_type = UOBCMS.Common.Constant.FPS_PTA; // default PTA
                        else if (bodyClientBank.Fps_payment_type != "")
                            clientBank.Fps_payment_type = bodyClientBank.Fps_payment_type; // replace the fps_payment_type
                    }
                    else
                    {
                        clientBank.Fps_payment_type = " ";
                    }
                }
                else
                {
                    clientBank.Client_id = client_account.Client_id;
                    clientBank.Bene_bank_swift = bodyClientBank.Bene_bank_swift;
                    clientBank.Bene_bank_address = bodyClientBank.Bene_bank_address;
                    clientBank.Bene_bank_accname = bodyClientBank.Bene_bank_accname;
                    clientBank.Bene_bank_name = bodyClientBank.Bene_bank_name;
                    clientBank.Corr_bank_name = bodyClientBank.Corr_bank_name;
                    clientBank.Corr_bank_swift = bodyClientBank.Corr_bank_swift;
                    clientBank.Bank_accno = bodyClientBank.Bene_bank_accno;
                    clientBank.Ccy = bodyClientBank.Bene_ccy;
                    clientBank.Type = bodyClientBank.Bank_type;

                    if ((bodyClientBank.Bene_ccy == "HKD" ||
                        bodyClientBank.Bene_ccy == "USD" ||
                        bodyClientBank.Bene_ccy == "CNY" ||
                        bodyClientBank.Bene_ccy == "EUR") && 
                        bodyClientBank.Bene_bank_country == "HK")
                    {
                        // according to Finance, Chats: fund transfer within HK = HKD, USD, CNY and EUR(they can be settled and cleared in HK)
                        clientBank.Default_payment_type = UOBCMS.Common.Constant.CHATS;
                    }
                    else
                    {
                        // according to Finance, TT: fund remits by TT outside HK(except for the above 4 currencies)
                        clientBank.Default_payment_type = UOBCMS.Common.Constant.TT;
                    }

                    clientBank.Fps_payment_type = " ";
                    clientBank.Bene_bank_country = bodyClientBank.Bene_bank_country;
                    clientBank.Corr_bank_country = bodyClientBank.Corr_bank_country;
                    clientBank.Bene_bank_code = bodyClientBank.Bene_bank_code;
                    clientBank.Corr_bank_code = bodyClientBank.Corr_bank_code;
                    clientBank.Msg_to_bank = bodyClientBank.Msg_to_bank;
                }

                clientBank.Status = "A";
                clientBank.Lastupdatedatetime = DateTime.UtcNow;
                clientBank.Lastupdateuserid = clientBank.Lastupdateuserid;
                clientBank.Version = 1;
                clientBank.Dbopr = "I";

                _context.Cms_client_banks.Add(clientBank);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Add to account level
                cms_account_bank accountBank = new cms_account_bank();
                accountBank.Acc_id = account.Id;
                accountBank.Client_bank_id = clientBank.Id;
                accountBank.DefaultLocalHKDBank = bodyClientBank.DefaultLocalHKDBank;
                accountBank.Lastupdateuserid = accountBank.Lastupdateuserid;
                accountBank.Lastupdatedatetime = clientBank.Lastupdatedatetime;
                accountBank.Version = 1;
                accountBank.Dbopr = "I";

                _context.Cms_account_banks.Add(accountBank);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return a success response with the newly created bank
                return clientBank;
            }
            else
                return null;
        }


        [HttpPost("SaveBulkAddress")]
        public async Task<ActionResult> SaveBulkAddress([FromBody] body_cms_client_address bodyClientAddress)
        {
            string className = GetType().Name;
            string methodName = "SaveBulkAddress";
            Logger.LogErrorMessage(className, methodName, "", bodyClientAddress.ToString(), Logger.INFO);

            int clientAddressId = -1;

            // Validate the input
            if (bodyClientAddress == null)
            {
                string errMsg = "Invalid address data.";
                Logger.LogErrorMessage(className, methodName, "", bodyClientAddress.ToString(), Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientAddress = await _context.Cms_client_addresses
                    .FirstOrDefaultAsync(q => q.Id == bodyClientAddress.Id);

            if (clientAddress == null)
            {
                clientAddress = await AddAddress(bodyClientAddress);
                if (clientAddress == null)
                {
                    string errMsg = $"Fail to create address for account {bodyClientAddress.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", bodyClientAddress.ToString(), Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientAddressId = clientAddress.Id;
            }
            else
            {
                if (!await ReplaceAddress(bodyClientAddress))
                {
                    string errMsg = $"Fail to replace phone for account {bodyClientAddress.Accno}.";
                    Logger.LogErrorMessage(className, methodName, "", bodyClientAddress.ToString(), Logger.ERROR);
                    return BadRequest(errMsg);
                }

                clientAddressId = bodyClientAddress.Id;
            }

            if (bodyClientAddress.applyToAllAcc)
            {
                /*  
                 *  applying changes to all accounts with the same 9-digit account number. Margin, Stock Options, and Futures account information under the same account will be updated accordingly (if applicable).
                 *  e.g. changes apply to 116333-001, 116333-001M, 116333-001S, and 116333-001F.
                 */
                string original_accno = bodyClientAddress.Accno;
                string accno = original_accno;

                int pos = accno.IndexOf("-");
                if (pos > 0)
                {
                    string accTypeStr = accno.Substring(pos + 1, UOBCMS.Common.Constant.LEN_ACCTYPENO);
                    if (accTypeStr == UOBCMS.Common.Constant.BULK_ACCTYPENO)
                    {
                        var account_list = await _context.Cms_accounts
                            .Where(c => c.AccNo.Contains(accno.Substring(0, 10))).ToListAsync();

                        if (account_list != null)
                        {
                            foreach (cms_account account in account_list)
                            {
                                if (account.AccNo != original_accno)
                                {
                                    bodyClientAddress.Accno = account.AccNo;

                                    var client_account = await _context.Cms_client_accounts
                                                    .FirstOrDefaultAsync(c => c.Acc_id == account.Id);


                                    if (client_account != null)
                                    {
                                        List<cms_client_address> client_address_list = null;
                                        client_address_list = await _context.Cms_client_addresses
                                                .Where(c => c.Client_id == client_account.Client_id && c.Type == bodyClientAddress.Type &&
                                                c.Addr1 == bodyClientAddress.Addr1 &&
                                                c.Addr2 == bodyClientAddress.Addr2 &&
                                                c.Addr3 == bodyClientAddress.Addr3 &&
                                                c.Addr4 == bodyClientAddress.Addr4 &&
                                                c.Country == bodyClientAddress.Country &&
                                                c.District == bodyClientAddress.District &&
                                                c.City == bodyClientAddress.City &&
                                                c.Province == bodyClientAddress.Province &&
                                                c.Postal_code == bodyClientAddress.Postal_code &&
                                                c.Zip == bodyClientAddress.Zip
                                                ).ToListAsync();

                                        bool replaced = false;

                                        foreach (cms_client_address client_address in client_address_list)
                                        {
                                            var account_address = await _context.Cms_account_addresses
                                                    .FirstOrDefaultAsync(c => c.Acc_id == account.Id && c.Client_address_id == client_address.Id);

                                            if (account_address != null)
                                            {
                                                // already linked to the client address with details have been updated by the original account
                                                replaced = true;

                                            }
                                        }

                                        if (!replaced)
                                        {
                                            // if account is not link to the same client address, remove the original link which linked to a different client address
                                            var account_address_list = await _context.Cms_account_addresses
                                                    .Include(c => c.Cms_client_address)
                                                    .Where(q => q.Acc_id == account.Id && q.Cms_client_address.Type == bodyClientAddress.Type).ToListAsync();

                                            foreach (cms_account_address account_address in account_address_list)
                                            {
                                                List<cms_client_address> account_client_address_list = null;
                                                account_client_address_list = await _context.Cms_client_addresses
                                                        .Where(c => c.Id == account_address.Client_address_id && c.Type == bodyClientAddress.Type).ToListAsync();

                                                foreach (cms_client_address account_client_address in account_client_address_list)
                                                {
                                                    account_address.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                                                    account_address.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                                                    account_address.Version += 1;
                                                    account_address.Dbopr = "D";
                                                }
                                            }

                                            // then create the link to link to the same client address with the original account
                                            cms_account_address accountAddress = new cms_account_address();
                                            accountAddress.Acc_id = account.Id;
                                            accountAddress.Client_address_id = clientAddressId;
                                            accountAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                                            accountAddress.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                                            accountAddress.Version = 1;
                                            accountAddress.Dbopr = "I";

                                            _context.Cms_account_addresses.Add(accountAddress);

                                            
                                            // Save changes to the database
                                            await _context.SaveChangesAsync();
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                }
            }

            bool bool_removed_client_address = false;

            // clean up for any client_address not link to any client account
            var all_client_address = await _context.Cms_client_addresses
                                                    .Where(c => c.Client_id == clientAddress.Client_id).ToListAsync();

            foreach (cms_client_address client_address in all_client_address)
            {
                var account_address_list = await _context.Cms_account_addresses
                                                    .Where(c => c.Client_address_id == client_address.Id).ToListAsync();

                if (account_address_list.Count == 0)
                {
                    client_address.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                    client_address.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                    client_address.Version += 1;
                    client_address.Dbopr = "D";
                    bool_removed_client_address = true;
                }
            }

            if (bool_removed_client_address)
                // Save changes to the database
                await _context.SaveChangesAsync();

            return Ok(clientAddress);
        }

        [HttpPost("SaveAddress")]
        public async Task<ActionResult> SaveAddress([FromBody] body_cms_client_address bodyClientAddress)
        {
            string className = GetType().Name;
            string methodName = "SaveAddress";
            Logger.LogErrorMessage(className, methodName, "", bodyClientAddress.ToString(), Logger.INFO);

            // Validate the input
            if (bodyClientAddress == null)
            {
                string errMsg = "Invalid address data.";
                Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                return BadRequest(errMsg);
            }

            var clientAddress = await _context.Cms_client_addresses
                    .FirstOrDefaultAsync(q => q.Id == bodyClientAddress.Id);

            if (clientAddress == null)
            {
                clientAddress = await AddAddress(bodyClientAddress);
                if (clientAddress == null)
                {
                    string errMsg = "Fail to create address.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }
            else
            {
                if (!await ReplaceAddress(bodyClientAddress))
                {
                    string errMsg = "Fail to replace address.";
                    Logger.LogErrorMessage(className, methodName, "", errMsg, Logger.ERROR);
                    return BadRequest(errMsg);
                }
            }

            // Return a success response with the newly created address
            //return CreatedAtAction(nameof(SaveAddress), new { accno = clientAddress.Client_id }, clientAddress);
            return Ok(clientAddress);
        }

        private async Task<bool> ReplaceAddress(body_cms_client_address bodyClientAddress)
        {
            if (bodyClientAddress == null)
                return false;

            var clientAddress = await _context.Cms_client_addresses
                    .FirstOrDefaultAsync(q => q.Id == bodyClientAddress.Id);

            if (clientAddress == null)
                return false;

            if (bodyClientAddress.applyToAllAcc)
            {
                // Set the Lastupdatedatetime to the current time
                clientAddress.Type = bodyClientAddress.Type;
                clientAddress.Addr1 = bodyClientAddress.Addr1;
                clientAddress.Addr2 = bodyClientAddress.Addr2;
                clientAddress.Addr3 = bodyClientAddress.Addr3;
                clientAddress.Addr4 = bodyClientAddress.Addr4;
                clientAddress.Country = bodyClientAddress.Country;
                clientAddress.City = bodyClientAddress.City;
                clientAddress.District = bodyClientAddress.District;
                clientAddress.Province = bodyClientAddress.Province;
                clientAddress.Postal_code = bodyClientAddress.Postal_code;
                clientAddress.Zip = bodyClientAddress.Zip;

                clientAddress.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                clientAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                //clientAddress.Version += 1;
                clientAddress.Dbopr = "U";

                _context.Cms_client_addresses.Update(clientAddress);
            }
            else
            {
                cms_account account = await _context.Cms_accounts
                        .Include(c => c.Cms_client_account)
                        .FirstOrDefaultAsync(q => q.AccNo == bodyClientAddress.Accno);


                if (account == null)
                    return false;

                // new to create a separate link with a new client_address if the client_address is used by other accounts
                var accountAddress = await _context.Cms_account_addresses
                    .FirstOrDefaultAsync(q => q.Client_address_id == bodyClientAddress.Id && q.Acc_id != account.Id);

                if (accountAddress == null)
                {
                    // Set the Lastupdatedatetime to the current time
                    clientAddress.Type = bodyClientAddress.Type;
                    clientAddress.Addr1 = bodyClientAddress.Addr1;
                    clientAddress.Addr2 = bodyClientAddress.Addr2;
                    clientAddress.Addr3 = bodyClientAddress.Addr3;
                    clientAddress.Addr4 = bodyClientAddress.Addr4;
                    clientAddress.Country = bodyClientAddress.Country;
                    clientAddress.City = bodyClientAddress.City;
                    clientAddress.District = bodyClientAddress.District;
                    clientAddress.Province = bodyClientAddress.Province;
                    clientAddress.Postal_code = bodyClientAddress.Postal_code;
                    clientAddress.Zip = bodyClientAddress.Zip;

                    clientAddress.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime; ;
                    clientAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                    //clientAddress.Version += 1;
                    clientAddress.Dbopr = "U";

                    _context.Cms_client_addresses.Update(clientAddress);

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                else
                {
                    cms_client_address new_clientAddress = new cms_client_address();
                    new_clientAddress.Type = bodyClientAddress.Type;
                    new_clientAddress.Addr1 = bodyClientAddress.Addr1;
                    new_clientAddress.Addr2 = bodyClientAddress.Addr2;
                    new_clientAddress.Addr3 = bodyClientAddress.Addr3;
                    new_clientAddress.Addr4 = bodyClientAddress.Addr4;
                    new_clientAddress.Country = bodyClientAddress.Country;
                    new_clientAddress.City = bodyClientAddress.City;
                    new_clientAddress.District = bodyClientAddress.District;
                    new_clientAddress.Province = bodyClientAddress.Province;
                    new_clientAddress.Postal_code = bodyClientAddress.Postal_code;
                    new_clientAddress.Zip = bodyClientAddress.Zip;
                    new_clientAddress.Client_id = account.Cms_client_account.Client_id;
                    new_clientAddress.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                    new_clientAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                    new_clientAddress.Version = 1;
                    new_clientAddress.Dbopr = "I";

                    _context.Cms_client_addresses.Add(new_clientAddress);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                    // remove the original link
                    var old_account_address_list = await _context.Cms_account_addresses
                                                    .Include(c => c.Cms_client_address)
                                                    .Where(q => q.Acc_id == account.Id && q.Cms_client_address.Type == bodyClientAddress.Type).ToListAsync();

                    foreach (cms_account_address old_account_address in old_account_address_list)
                    {
                        old_account_address.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                        old_account_address.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                        old_account_address.Version += 1;
                        old_account_address.Dbopr = "D";
                    }

                    
                    // create the link to the new client_address
                    cms_account_address new_accountAddress = new cms_account_address();
                    new_accountAddress.Acc_id = account.Id;
                    new_accountAddress.Client_address_id = new_clientAddress.Id;
                    new_accountAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
                    new_accountAddress.Lastupdatedatetime = bodyClientAddress.Lastupdatedatetime;
                    new_accountAddress.Version = 1;
                    new_accountAddress.Dbopr = "I";

                    _context.Cms_account_addresses.Add(new_accountAddress);

                    // Save changes to the database
                    await _context.SaveChangesAsync();

                }
            }            

            /*
            // Add to account level
            cms_account_address accountAddress = new cms_account_address();
            accountAddress.Acc_id = bodyClientAddress.Acc_id;
            accountAddress.Client_address_id = clientAddress.Id;
            accountAddress.Lastupdateuserid = "louis";
            accountAddress.Lastupdatedatetime = clientAddress.Lastupdatedatetime;
            accountAddress.Version = 1;
            accountAddress.Dbopr = "I";

            _context.Cms_account_addresses.Add(accountAddress);

            // Save changes to the database
            await _context.SaveChangesAsync();*/

            return true;
        }

        private async Task<cms_client_address> AddAddress(body_cms_client_address bodyClientAddress)
        {
            if (bodyClientAddress == null)
                return null;

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == bodyClientAddress.Accno);

            if (account == null)
                return null;

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
                return null;

            // Add the new address to the context (client level)
            cms_client_address clientAddress = new cms_client_address();

            // Set the Lastupdatedatetime to the current time
            clientAddress.Client_id = client_account.Client_id;
            clientAddress.Type = bodyClientAddress.Type;
            clientAddress.Addr1 = bodyClientAddress.Addr1;
            clientAddress.Addr2 = bodyClientAddress.Addr2;
            clientAddress.Addr3 = bodyClientAddress.Addr3;
            clientAddress.Addr4 = bodyClientAddress.Addr4;
            clientAddress.Country = bodyClientAddress.Country;
            clientAddress.City = bodyClientAddress.City;
            clientAddress.District = bodyClientAddress.District;
            clientAddress.Province = bodyClientAddress.Province;
            clientAddress.Postal_code = bodyClientAddress.Postal_code;
            clientAddress.Zip = bodyClientAddress.Zip;

            clientAddress.Lastupdatedatetime = DateTime.UtcNow;
            clientAddress.Lastupdateuserid = bodyClientAddress.Lastupdateuserid;
            clientAddress.Version = 1;
            clientAddress.Dbopr = "I";

            _context.Cms_client_addresses.Add(clientAddress);

            // Save changes to the database
            await _context.SaveChangesAsync();


            // Add to account level
            cms_account_address accountAddress = new cms_account_address();
            accountAddress.Acc_id = account.Id;
            accountAddress.Client_address_id = clientAddress.Id;
            accountAddress.Lastupdateuserid = clientAddress.Lastupdateuserid;
            accountAddress.Lastupdatedatetime = clientAddress.Lastupdatedatetime;
            accountAddress.Version = 1;
            accountAddress.Dbopr = "I";

            _context.Cms_account_addresses.Add(accountAddress);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success response with the newly created address
            return clientAddress;
        }

        [HttpGet("GetAccountDKQ/{accno}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountDKQ(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountDKQ";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            var accountDKQ = await _context.Cms_account_dkqs
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (accountDKQ == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"DKQ of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"DKQ of Account: {accno} is found", Logger.INFO);

            return Ok(accountDKQ);
        }

        [HttpGet("GetAccountW8/{accno}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountW8(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountW8";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            var accountW8 = await _context.Cms_account_w8s
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (accountW8 == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"W8 of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"W8 of Account: {accno} is found", Logger.INFO);

            return Ok(accountW8);
        }

        [HttpGet("GetAccountAddress/{accno}/{type}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountAddress(string accno, string type)
        {
            string className = GetType().Name;
            string methodName = "GetAccountAddress";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client of Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            var client_addr_list = await _context.Cms_client_addresses
                .Where(c => c.Client_id == client_account.Client_id && c.Type == type).ToListAsync();

            if (client_addr_list == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client Address of Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            foreach (var client_addr in client_addr_list)
            {
                var account_addr = await _context.Cms_account_addresses
                    .FirstOrDefaultAsync(c => c.Client_address_id == client_addr.Id && c.Acc_id == account.Id);

                if (account_addr != null)
                {
                    Logger.LogErrorMessage(className, methodName, "", $"Account Address of Account: {accno} is found", Logger.INFO);
                    return Ok(client_addr); // Return the list of addresses as a successful response*/                    
                }
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Address of Account: {accno} is not found", Logger.ERROR);
            //return NotFound();
            return null;

            /*var account = await _context.Cms_accounts
                .Include(c => c.Cms_account_addresses)
                .ThenInclude(ca => ca.Cms_client_address)
                .FirstOrDefaultAsync(c => c.AccNo == accno);*/

            // Extract addresses from the account
            /*var addresses = account.Cms_account_addresses
                .Select(ca => new
                {
                    ca.Cms_client_address.Addr1,
                    ca.Cms_client_address.Addr2,
                    ca.Cms_client_address.City,
                    ca.Cms_client_address.Province,
                    ca.Cms_client_address.Country,
                    ca.Cms_client_address.Postal_code
                })
                .ToList();

            return Ok(addresses); // Return the list of addresses as a successful response*/
        }

        /* Get all accounts under the same client */
        [HttpGet("GetAccRelatedAccs/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_account>>> GetAccRelatedAccs(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccRelatedAccs";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            var client_accounts_list = await _context.Cms_client_accounts
                .Include(c => c.Cms_account)
                .Where(c => c.Client_id == client_account.Client_id).ToListAsync();

            if (client_accounts_list == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"Related Account of Account: {accno} is found", Logger.ERROR);

            return Ok(client_accounts_list);
        }

        [HttpGet("GetAccountAddresses/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_address>>> GetAccountAddresses(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountAddresses";

            // Fetch account with related addresses
            var accountAddr = await _context.Cms_accounts
                .Include(c => c.Cms_account_addresses)
                .ThenInclude(ca => ca.Cms_client_address)
                .Where(c => c.AccNo == accno)
                .SelectMany(c => c.Cms_account_addresses.Select(ca => ca.Cms_client_address)) // Flatten the addresses
                .ToListAsync();

            // Check if no addresses were found
            if (accountAddr == null || !accountAddr.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Addresss of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Addresss of Account: {accno} is found", Logger.INFO);

            return Ok(accountAddr); // Return the list of addresses
        }

        [HttpGet("GetInstrumentName/{mktcode}/{seccode}")]
        public async Task<ActionResult<string>> GetInstrumentName(string mktcode, string seccode)
        {
            string className = GetType().Name;
            string methodName = "GetInstrumentName";

            var ins = await _iboContext.Instruments
                    .FirstOrDefaultAsync(c => (c.Market == mktcode && c.Instr == seccode));

            if (ins == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Instrument: {mktcode} {seccode} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"Instrument: {mktcode} {seccode} is found", Logger.INFO);

            return Ok(ins.Name);
        }

        [HttpGet("GetAccountMthSavingPlan/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_account_mthsaving_plan>>> GetAccountMthSavingPlans(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountMthSavingPlan";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            var accountMthSavingPlans = await _context.Cms_account_mthsaving_plans
                        .Where(c => (c.Acc_id == account.Id && c.Eff_end_dt >= DateTime.Now.Date))
                        .ToListAsync();

            if (accountMthSavingPlans == null || !accountMthSavingPlans.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Monthly Saving Plan of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            List<body_cms_account_mthsaving_plan> bodyAccountMthSavingPlanList = new List<body_cms_account_mthsaving_plan>();

            foreach (cms_account_mthsaving_plan accountMthSavingPlan in accountMthSavingPlans)
            {
                body_cms_account_mthsaving_plan bodyAccountMthSavingPlan = new body_cms_account_mthsaving_plan();

                bodyAccountMthSavingPlan.Id = accountMthSavingPlan.Id;
                bodyAccountMthSavingPlan.Acc_id = accountMthSavingPlan.Acc_id;
                bodyAccountMthSavingPlan.Accno = accno;
                bodyAccountMthSavingPlan.Eff_start_dt = accountMthSavingPlan.Eff_start_dt;
                bodyAccountMthSavingPlan.Eff_end_dt = accountMthSavingPlan.Eff_end_dt;
                bodyAccountMthSavingPlan.Mkt_code = accountMthSavingPlan.Mkt_code;
                bodyAccountMthSavingPlan.Sec_code = accountMthSavingPlan.Sec_code;

                var ins = await _iboContext.Instruments
                    .FirstOrDefaultAsync(c => c.Instr == accountMthSavingPlan.Sec_code);

                if (ins != null)
                    bodyAccountMthSavingPlan.Sec_name = ins.Name;
                else
                    bodyAccountMthSavingPlan.Sec_name = "";

                bodyAccountMthSavingPlan.Ccy = accountMthSavingPlan.Ccy;
                bodyAccountMthSavingPlan.Invest_amt = accountMthSavingPlan.Invest_amt;
                bodyAccountMthSavingPlan.Lastupdateuserid = accountMthSavingPlan.Lastupdateuserid;
                bodyAccountMthSavingPlan.Lastupdatedatetime = accountMthSavingPlan.Lastupdatedatetime;
                bodyAccountMthSavingPlan.Version = accountMthSavingPlan.Version;
                bodyAccountMthSavingPlan.Dbopr = accountMthSavingPlan.Dbopr;

                bodyAccountMthSavingPlanList.Add(bodyAccountMthSavingPlan);
            }

            Logger.LogErrorMessage(className, methodName, "", $"Monthly Saving Plan of Account: {accno} is found", Logger.INFO);

            return Ok(bodyAccountMthSavingPlanList);
        }

        [HttpGet("GetAccountLocalBanks/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_bank>>> GetAccountLocalBanks(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountLocalBanks";

            // Fetch account with related banks
            var accountBanks = await _context.Cms_accounts
            .Include(c => c.Cms_account_banks)
                .ThenInclude(ca => ca.Cms_client_bank)
                    .ThenInclude(cb => cb.Cms_bank) // Include the associated bank details
            .Where(c => c.AccNo == accno) // Filter accounts by AccNo
            .SelectMany(c => c.Cms_account_banks
                .Select(ca => new
                {
                    ClientBank = ca.Cms_client_bank, // Select the client bank
                    Bank = ca.Cms_client_bank.Cms_bank, // Select the associated bank
                    AccountBank = ca // Include the account bank details
                })
                .Where(result => result.ClientBank.Type == UOBCMS.Common.Constant.LOCAL_BANK && result.ClientBank.Status == "A") // Filter where bank.Type is 0
            )
            .ToListAsync();

            // Check if no bank were found
            if (accountBanks == null || !accountBanks.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Local Bank of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            List<body_cms_client_bank> banks = new List<body_cms_client_bank>();

            foreach (var accountBank in accountBanks)
            {
                body_cms_client_bank bank = new body_cms_client_bank();
                bank.Id = accountBank.ClientBank.Id;

                if (accountBank.Bank != null)
                {
                    bank.Bank_name = accountBank.Bank.Bank_name;
                    bank.Bank_code = accountBank.Bank.Bank_code;
                }
                else
                {
                    bank.Bank_name = "";
                    bank.Bank_code = "";
                }

                bank.Bank_accno = accountBank.ClientBank.Bank_accno;
                bank.Bank_accname = accountBank.ClientBank.Bank_accname;

                bank.DefaultLocalHKDBank = accountBank.AccountBank.DefaultLocalHKDBank;
                bank.Bene_bank_swift = accountBank.ClientBank.Bene_bank_swift;
                bank.Bene_bank_address = accountBank.ClientBank.Bene_bank_address;
                bank.Bene_bank_accname = accountBank.ClientBank.Bene_bank_accname;
                bank.Bene_bank_name = accountBank.ClientBank.Bene_bank_name;
                bank.Corr_bank_name = accountBank.ClientBank.Corr_bank_name;
                bank.Corr_bank_swift = accountBank.ClientBank.Corr_bank_swift;
                bank.Ccy = accountBank.ClientBank.Ccy;
                bank.Other_bank_details = accountBank.ClientBank.Other_bank_details;
                bank.Status = accountBank.ClientBank.Status;
                bank.Default_payment_type = accountBank.ClientBank.Default_payment_type;
                bank.Bene_bank_country = accountBank.ClientBank.Bene_bank_country;
                bank.Corr_bank_country = accountBank.ClientBank.Corr_bank_country;
                bank.Bene_bank_code = accountBank.ClientBank.Bene_bank_code;
                bank.Corr_bank_code = accountBank.ClientBank.Corr_bank_code;
                bank.Msg_to_bank = accountBank.ClientBank.Msg_to_bank;

                banks.Add(bank);
            }

            Logger.LogErrorMessage(className, methodName, "", $"Local Bank of Account: {accno} is found", Logger.INFO);

            return Ok(banks); // Return the list of banks
        }

        [HttpGet("GetAccountLocalBank/{accno}/{clientbankId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountLocalBank(string accno, int clientbankId)
        {
            string className = GetType().Name;
            string methodName = "GetAccountLocalBank";

            var accountBanks = await _context.Cms_accounts
            .Include(c => c.Cms_account_banks)
                .ThenInclude(ca => ca.Cms_client_bank)
                    .ThenInclude(cb => cb.Cms_bank) // Include the associated bank details
            .Where(c => c.AccNo == accno)           // Filter accounts by AccNo
            .SelectMany(c => c.Cms_account_banks
                .Select(ca => new
                {
                    ClientBank = ca.Cms_client_bank, // Select the client bank
                    Bank = ca.Cms_client_bank.Cms_bank, // Select the associated bank
                    AccountBank = ca // Include the account bank details
                })
                .Where(result => result.ClientBank.Id == clientbankId && result.ClientBank.Status == "A")
            )
            .ToListAsync();

            // Check if no bank were found
            if (accountBanks == null || !accountBanks.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Local Bank of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            List<body_cms_client_bank> banks = new List<body_cms_client_bank>();

            foreach (var accountBank in accountBanks)
            {
                body_cms_client_bank bank = new body_cms_client_bank();
                bank.Id = accountBank.ClientBank.Id;

                if (accountBank.Bank != null)
                {
                    bank.Bank_type = accountBank.Bank.Type;
                    bank.Bank_name = accountBank.Bank.Bank_name;
                    bank.Bank_code = accountBank.Bank.Bank_code;
                }
                else
                {
                    bank.Bank_type = UOBCMS.Common.Constant.LOCAL_BANK;
                    bank.Bank_name = "";
                    bank.Bank_code = "";
                }

                bank.Bank_accno = accountBank.ClientBank.Bank_accno;
                bank.Bank_accname = accountBank.ClientBank.Bank_accname;
                bank.DefaultLocalHKDBank = accountBank.AccountBank.DefaultLocalHKDBank;
                bank.Bene_bank_swift = accountBank.ClientBank.Bene_bank_swift;
                bank.Bene_bank_address = accountBank.ClientBank.Bene_bank_address;
                bank.Bene_bank_accname = accountBank.ClientBank.Bene_bank_accname;
                bank.Bene_bank_name = accountBank.ClientBank.Bene_bank_name;
                bank.Corr_bank_name = accountBank.ClientBank.Corr_bank_name;
                bank.Corr_bank_swift = accountBank.ClientBank.Corr_bank_swift;
                bank.Ccy = accountBank.ClientBank.Ccy;
                bank.Other_bank_details = accountBank.ClientBank.Other_bank_details;
                bank.Status = accountBank.ClientBank.Status;
                bank.Default_payment_type = accountBank.ClientBank.Default_payment_type;
                bank.Bene_bank_country = accountBank.ClientBank.Bene_bank_country;
                bank.Corr_bank_country = accountBank.ClientBank.Corr_bank_country;
                bank.Bene_bank_code = accountBank.ClientBank.Bene_bank_code;
                bank.Corr_bank_code = accountBank.ClientBank.Corr_bank_code;
                bank.Msg_to_bank = accountBank.ClientBank.Msg_to_bank;
                banks.Add(bank);
                break;
            }

            Logger.LogErrorMessage(className, methodName, "", $"Local Bank of Account: {accno} is found", Logger.INFO);
            return Ok(banks);
        }



        [HttpGet("GetAccountOverseaBanks/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_bank>>> GetAccountOverseaBanks(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountOverseaBanks";

            // Fetch account with related banks
            var accountBanks = await _context.Cms_accounts
            .Include(c => c.Cms_account_banks)
                .ThenInclude(ca => ca.Cms_client_bank)
            .Where(c => c.AccNo == accno) // Filter accounts by AccNo
            .SelectMany(c => c.Cms_account_banks
                .Select(ca => new
                {
                    ClientBank = ca.Cms_client_bank, // Select the client bank
                    AccountBank = ca // Include the account bank details
                })
                .Where(result => result.ClientBank.Status == "A" && result.ClientBank.Type == UOBCMS.Common.Constant.OVERSEA_BANK) // Filter where bank id is null which are oversea banks
            )
            .ToListAsync();

            // Check if no bank were found
            if (accountBanks == null || !accountBanks.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Oversea Bank of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            List<body_cms_client_bank> banks = new List<body_cms_client_bank>();

            foreach (var accountBank in accountBanks)
            {
                body_cms_client_bank bank = new body_cms_client_bank();
                bank.Id = accountBank.ClientBank.Id;
                bank.Bank_accno = accountBank.ClientBank.Bank_accno;
                bank.Bank_accname = accountBank.ClientBank.Bank_accname;
                bank.DefaultLocalHKDBank = accountBank.AccountBank.DefaultLocalHKDBank;
                bank.Bene_bank_swift = accountBank.ClientBank.Bene_bank_swift;
                bank.Bene_bank_address = accountBank.ClientBank.Bene_bank_address;
                bank.Bene_bank_accname = accountBank.ClientBank.Bene_bank_accname;
                bank.Bene_bank_name = accountBank.ClientBank.Bene_bank_name;
                bank.Corr_bank_name = accountBank.ClientBank.Corr_bank_name;
                bank.Corr_bank_swift = accountBank.ClientBank.Corr_bank_swift;
                bank.Ccy = accountBank.ClientBank.Ccy;
                bank.Other_bank_details = accountBank.ClientBank.Other_bank_details;
                bank.Status = accountBank.ClientBank.Status;
                bank.Default_payment_type = accountBank.ClientBank.Default_payment_type;
                bank.Bene_bank_country = accountBank.ClientBank.Bene_bank_country;
                bank.Corr_bank_country = accountBank.ClientBank.Corr_bank_country;
                bank.Bene_bank_code = accountBank.ClientBank.Bene_bank_code;
                bank.Corr_bank_code = accountBank.ClientBank.Corr_bank_code;
                bank.Msg_to_bank = accountBank.ClientBank.Msg_to_bank;
                banks.Add(bank);
            }

            Logger.LogErrorMessage(className, methodName, "", $"Oversea Bank of Account: {accno} is found", Logger.INFO);

            return Ok(banks); // Return the list of banks
        }

        [HttpGet("GetAccountOverseaBank/{accno}/{clientbankId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountOverseaBank(string accno, int clientbankId)
        {
            string className = GetType().Name;
            string methodName = "GetAccountOverseaBank";

            var accountBanks = await _context.Cms_accounts
            .Include(c => c.Cms_account_banks)
                .ThenInclude(ca => ca.Cms_client_bank)
            .Where(c => c.AccNo == accno)           // Filter accounts by AccNo
            .SelectMany(c => c.Cms_account_banks
                .Select(ca => new
                {
                    ClientBank = ca.Cms_client_bank, // Select the client bank
                    AccountBank = ca // Include the account bank details
                })
                .Where(result => result.ClientBank.Status == "A" && result.ClientBank.Bank_id == null && result.ClientBank.Id == clientbankId)
            )
            .ToListAsync();

            // Check if no bank were found
            if (accountBanks == null || !accountBanks.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Oversea Bank of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            List<body_cms_client_bank> banks = new List<body_cms_client_bank>();

            foreach (var accountBank in accountBanks)
            {
                body_cms_client_bank bank = new body_cms_client_bank();
                bank.Id = accountBank.ClientBank.Id;
                bank.Bank_type = UOBCMS.Common.Constant.OVERSEA_BANK;
                bank.Bank_accno = accountBank.ClientBank.Bank_accno;
                bank.Bank_accname = accountBank.ClientBank.Bank_accname;
                bank.DefaultLocalHKDBank = accountBank.AccountBank.DefaultLocalHKDBank;
                bank.Bene_bank_swift = accountBank.ClientBank.Bene_bank_swift;
                bank.Bene_bank_address = accountBank.ClientBank.Bene_bank_address;
                bank.Bene_bank_accname = accountBank.ClientBank.Bene_bank_accname;
                bank.Bene_bank_name = accountBank.ClientBank.Bene_bank_name;
                bank.Corr_bank_name = accountBank.ClientBank.Corr_bank_name;
                bank.Corr_bank_swift = accountBank.ClientBank.Corr_bank_swift;
                bank.Ccy = accountBank.ClientBank.Ccy;
                bank.Other_bank_details = accountBank.ClientBank.Other_bank_details;
                bank.Status = accountBank.ClientBank.Status;
                bank.Default_payment_type = accountBank.ClientBank.Default_payment_type;
                bank.Bene_bank_country = accountBank.ClientBank.Bene_bank_country;
                bank.Corr_bank_country = accountBank.ClientBank.Corr_bank_country;
                bank.Bene_bank_code = accountBank.ClientBank.Bene_bank_code;
                bank.Corr_bank_code = accountBank.ClientBank.Corr_bank_code;
                bank.Msg_to_bank = accountBank.ClientBank.Msg_to_bank;
                banks.Add(bank);
                break;
            }

            Logger.LogErrorMessage(className, methodName, "", $"Oversea Bank of Account: {accno} is found", Logger.INFO);

            return Ok(banks);
        }

        [HttpGet("GetSimilarEmail/{accno}/{email}")]
        public async Task<ActionResult<IEnumerable<string>>> GetSimilarEmail(string accno, string email)
        {
            string className = GetType().Name;
            string methodName = "GetSimilarEmail";

            var accountEmails = await _context.Cms_accounts
            .Include(c => c.Cms_account_emails)
                .ThenInclude(ca => ca.Cms_client_email)
            .Where(c => c.AccNo != accno && c.Cms_account_emails
                .Any(ca => ca.Cms_client_email.Email == email && (ca.Cms_client_email.Type == "0" || ca.Cms_client_email.Type == "1" || ca.Cms_client_email.Type == "2")) // Check if any associated email matches
            ).ToListAsync();

            return Ok(accountEmails);
        }

        [HttpGet("GetAccountEmail/{accno}/{type}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountEmail(string accno, string type)
        {
            string className = GetType().Name;
            string methodName = "GetAccountEmail";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client Account of Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            var client_email_list = await _context.Cms_client_emails
                .Where(c => c.Client_id == client_account.Client_id && c.Type == type).ToListAsync();

            if (client_email_list == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client Email of Account: {accno} is not found", Logger.ERROR);
                //return NotFound();
                return null;
            }

            foreach (var client_email in client_email_list)
            {
                var account_email = await _context.Cms_account_emails
                    .FirstOrDefaultAsync(c => c.Client_email_id == client_email.Id && c.Acc_id == account.Id);

                if (account_email != null)
                {
                    Logger.LogErrorMessage(className, methodName, "", $"Client Email of Account: {accno} is found", Logger.INFO);
                    return Ok(client_email); // Return the list of emails as a successful response*/                    
                }
            }

            Logger.LogErrorMessage(className, methodName, "", $"Client Email of Account: {accno} is not found", Logger.ERROR);

            //return NotFound();
            return null;
        }


        [HttpGet("GetAccountEmails/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_email>>> GetAccountEmails(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountEmails";

            // Fetch account with related emails
            var accountEmail = await _context.Cms_accounts
                .Include(c => c.Cms_account_emails)
                .ThenInclude(ca => ca.Cms_client_email)
                .Where(c => c.AccNo == accno)
                .SelectMany(c => c.Cms_account_emails.Select(ca => ca.Cms_client_email)) // Flatten the emails
                .ToListAsync();

            // Check if no emails were found
            if (accountEmail == null || !accountEmail.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Email of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Email of Account: {accno} is found", Logger.INFO);

            return Ok(accountEmail); // Return the list of emails
        }

        [HttpGet("GetAccountPhone/{accno}/{type}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccountPhone(string accno, string type)
        {
            string className = GetType().Name;
            string methodName = "GetAccountPhone";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return null;
                //return NotFound();
            }

            var client_account = await _context.Cms_client_accounts
                .FirstOrDefaultAsync(c => c.Acc_id == account.Id);

            if (client_account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client Account of Account: {accno} is not found", Logger.ERROR);
                return null;
                //return NotFound();
            }

            var client_phone_list = await _context.Cms_client_phones
                .Where(c => c.Client_id == client_account.Client_id && c.Type == type).ToListAsync();

            if (client_phone_list == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Client Phone of Account: {accno} is not found", Logger.ERROR);
                return null;
                //return NotFound();
            }

            foreach (var client_phone in client_phone_list)
            {
                var account_phone = await _context.Cms_account_phones
                    .FirstOrDefaultAsync(c => c.Client_phone_id == client_phone.Id && c.Acc_id == account.Id);

                if (account_phone != null)
                {
                    Logger.LogErrorMessage(className, methodName, "", $"Account Phone of Account: {accno} is found", Logger.INFO);
                    return Ok(client_phone); // Return the list of phones as a successful response*/                    
                }
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Phone of Account: {accno} is not found", Logger.ERROR);
            //return NotFound();
            return null;
        }

        [HttpGet("GetAccountPhones/{accno}")]
        public async Task<ActionResult<IEnumerable<cms_client_phone>>> GetAccountPhones(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountPhones";

            // Fetch account with related phones
            var accountPhone = await _context.Cms_accounts
                .Include(c => c.Cms_account_phones)
                .ThenInclude(ca => ca.Cms_client_phone)
                .Where(c => c.AccNo == accno)
                .SelectMany(c => c.Cms_account_phones.Select(ca => ca.Cms_client_phone)) // Flatten the phones
                .ToListAsync();

            // Check if no phones were found
            if (accountPhone == null || !accountPhone.Any())
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Phone of Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Phone of Account: {accno} is found", Logger.INFO);

            return Ok(accountPhone); // Return the list of phones
        }

        // GET: Clients
        [HttpGet("api/accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.Cms_accounts.ToListAsync();
            return Ok(accounts);  // Return accounts with OK status
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> Index()
        {
            /*List<cms_account> accounts = await _context.Cms_accounts
                                        .Include(a => a.Cms_client_account)
                                        .ToListAsync();*/

            var accounts = await _context.Cms_accounts
                .Include(a => a.Cms_client_account) // Include the Cms_client_account
                .ThenInclude(c => c.Cms_client) // Then include the Cms_client
                .Select(a => new AccountDto
                {
                    Id = a.Id,
                    AccNo = a.AccNo,
                    Ename = a.Ename,
                    Cname = a.Cname,
                    TypeString = a.TypeString,
                    StatusString = a.StatusString,
                    // Map other properties from cms_account
                    Cif = a.Cms_client_account.Cms_client.Cif // Access the Cif field correctly
                })
                .ToListAsync();

            /*List<cms_account> accounts = await _context.Cms_accounts
                                        .ToListAsync();*/

            return View(accounts); // Return the list to the view
        }


        // GET: Account Details
        [HttpGet("api/accountByAccno/{accno}")]
        public async Task<IActionResult> GetAccountbyAccno(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccountbyAccno";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the account is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is found", Logger.INFO);

            return Ok(account);
        }

        // GET: Account Details
        [HttpGet("api/accounts/{id}")]
        public async Task<IActionResult> GetAccountDetails(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountDetails";

            var account = await _context.Cms_accounts
                .Include(c => c.Cms_account_addresses)
                    .ThenInclude(ca => ca.Cms_client_address)
                .Include(cp => cp.Cms_account_phones)
                    .ThenInclude(cpa => cpa.Cms_client_phone)
                .Include(cp => cp.Cms_account_emails)
                    .ThenInclude(cpa => cpa.Cms_client_email)
                .Include(cp => cp.Cms_account_banks)
                    .ThenInclude(cpa => cpa.Cms_client_bank)
                    .ThenInclude(cpa => cpa.Cms_bank)
                .Include(cp => cp.Cms_account_mthsaving_plans)
                .Include(cp => cp.Cms_account_w8)
                .Include(cp => cp.Cms_account_dkq)
                .Include(cp => cp.Cms_account_hkidr)
                .Include(cp => cp.Cms_account_additionalinfos)
                    .ThenInclude(cpa => cpa.Cms_additionalinfo)
                .Include(cp => cp.Cms_account_limits)
                .Include(cp => cp.Cms_ae)
                .Include(cp => cp.Cms_account_markets)
                    .ThenInclude(cpa => cpa.Cms_account_bank)
                        .ThenInclude(cpa => cpa.Cms_client_bank)
                            .ThenInclude(cpa => cpa.Cms_bank)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Details of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            account.Cms_account_phones = account.Cms_account_phones
               .OrderBy(cp => cp.Cms_client_phone.Type) // Adjust the property as needed
               .ToList();

            Logger.LogErrorMessage(className, methodName, "", $"Details of Account: {id} is found", Logger.INFO);

            return Ok(account);
        }

        [HttpGet("accounts/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            string className = GetType().Name;
            string methodName = "Details";

            /*var account = await _context.Cms_accounts
                .Include(cab => cab.Cms_account_banks)
                    .ThenInclude(ccb => ccb.Cms_client_bank)
                    .ThenInclude(b => b.Cms_bank)
                .Include(cab => cab.Cms_account_phones)
                    .ThenInclude(ccb => ccb.Cms_client_phone)
                .Include(cab => cab.Cms_account_emails)
                    .ThenInclude(ccb => ccb.Cms_client_email)
                .Include(cab => cab.Cms_account_addresses)
                    .ThenInclude(ccb => ccb.Cms_client_address)
                .FirstOrDefaultAsync(c => c.Id == id);*/

            var account = await _context.Cms_accounts
                .AsNoTracking()
                .Include(cl => cl.Cms_client_account)
                    .ThenInclude(ca => ca.Cms_client)
                //.Include(c => c.Cms_account_addresses)
                //    .ThenInclude(ca => ca.Cms_client_address)
                //.Include(cp => cp.Cms_account_phones)
                //    .ThenInclude(cpa => cpa.Cms_client_phone)
                //.Include(cp => cp.Cms_account_emails)
                //    .ThenInclude(cpa => cpa.Cms_client_email)
                //.Include(cp => cp.Cms_account_banks)
                //    .ThenInclude(cpa => cpa.Cms_client_bank)
                //    .ThenInclude(cpa => cpa.Cms_bank)
                .Include(cp => cp.Cms_account_mthsaving_plans)
                .Include(cp => cp.Cms_account_w8)
                .Include(cp => cp.Cms_account_dkq)
                .Include(cp => cp.Cms_ae)
                .Include(cp => cp.Cms_account_north_bound)
                //.Include(cp => cp.Cms_account_hkidr)
                //.Include(cp => cp.Cms_account_additionalinfos)
                //.Include(cp => cp.Cms_account_limits)
                //.Include(cp => cp.Cms_account_markets)
                //    .ThenInclude(cpa => cpa.Cms_account_bank)
                //        .ThenInclude(cpa => cpa.Cms_client_bank)
                //            .ThenInclude(cpa => cpa.Cms_bank)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Details of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            account.Cms_account_phones = account.Cms_account_phones
               .OrderBy(cp => cp.Cms_client_phone.Type) // Adjust the property as needed
               .ToList();

            Logger.LogErrorMessage(className, methodName, "", $"Details of Account: {id} is found", Logger.INFO);
            return View(account); // Return the detailed view of the account
        }        

        [HttpGet("api/w8s/{id}")]
        public async Task<IActionResult> GetAccountW8s(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountW8s";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_w8)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"W8 of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"W8 of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/hkidrs/{id}")]
        public async Task<IActionResult> GetAccountHKIDRs(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountHKIDRs";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_hkidr)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"HKIDR of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"HKIDR of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/limits/{id}")]
        public async Task<IActionResult> GetAccountLimits(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountLimits";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_limits)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Limit of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Limit of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/markets/{id}")]
        public async Task<IActionResult> GetAccountMarkets(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountMarkets";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_markets)
                    .ThenInclude(cpa => cpa.Cms_account_bank)
                        .ThenInclude(cpa => cpa.Cms_client_bank)
                            .ThenInclude(cpa => cpa.Cms_bank)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Market of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Market of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/socialmedias/{id}")]
        public async Task<IActionResult> GetAccountSocialMedias(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountSocialMedias";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_social_medias)                    
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Social Media of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Socal Media of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/controllingpersons/{id}")]
        public async Task<IActionResult> GetAccountControllingPersons(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountControllingPersons";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_controlling_persons)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Controlling Persons of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Controlling Persons of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/authparties/{id}")]
        public async Task<IActionResult> GetAccountAuthParties(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountAuthParties";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_auth_parties)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Authroized Parties of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Authroized Parties of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/companystructure/{id}")]
        public async Task<IActionResult> GetAccountCompanyStructure(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountCompanyStructure";           

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_company_structure)
                    .ThenInclude(cps => cps.Cms_account_company_structure_directors)
                .Include(cp => cp.Cms_account_company_structure)
                    .ThenInclude(cps => cps.Cms_account_company_structure_shareholders)
                .Include(cp => cp.Cms_account_company_structure)
                    .ThenInclude(cps => cps.Cms_account_company_structure_intermediaries)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Company Structure of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Company Structure of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/dkqs/{id}")]
        public async Task<IActionResult> GetAccountDKQs(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountDKQs";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_dkq)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"DKQ of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"DKQ of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/phones/{id}")]
        public async Task<IActionResult> GetAccountPhones(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountPhones";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_phones)
                    .ThenInclude(cpa => cpa.Cms_client_phone)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Phone of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            account.Cms_account_phones = account.Cms_account_phones
               .OrderBy(cp => cp.Cms_client_phone.Type) // Adjust the property as needed
               .ToList();

            Logger.LogErrorMessage(className, methodName, "", $"Phone of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/emails/{id}")]
        public async Task<IActionResult> GetAccountEmails(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountEmails";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_emails)
                    .ThenInclude(cpa => cpa.Cms_client_email)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Email of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Email of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }


        [HttpGet("api/docs/{id}")]
        public async Task<IActionResult> GetAccountDocs(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountDocs";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_docs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Document of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Document of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/addresses/{id}")]
        public async Task<IActionResult> GetAccountAddresses(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountAddresses";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_addresses)
                    .ThenInclude(cpa => cpa.Cms_client_address)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Address of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found                
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Address of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/addinfos/{id}")]
        public async Task<IActionResult> GetAccountAddInfos(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountAddInfos";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_additionalinfos)
                    .ThenInclude(cpa => cpa.Cms_additionalinfo)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Additional Info of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Additional Info of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("api/banks/{id}")]
        public async Task<IActionResult> GetAccountBanks(int id)
        {
            string className = GetType().Name;
            string methodName = "GetAccountBanks";

            var account = await _context.Cms_accounts
                .Include(cp => cp.Cms_account_banks)
                    .ThenInclude(cpa => cpa.Cms_client_bank)
                    .ThenInclude(cpa => cpa.Cms_bank)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account Banks Info of Account: {id} is not found", Logger.ERROR);
                return NotFound(); // Return 404 if the product is not found
            }

            Logger.LogErrorMessage(className, methodName, "", $"Account Banks Info of Account: {id} is found", Logger.INFO);
            return Ok(account);
        }

        [HttpGet("GetAccount/{accno}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAccount(string accno)
        {
            string className = GetType().Name;
            string methodName = "GetAccount";

            var account = await _context.Cms_accounts
                .FirstOrDefaultAsync(c => c.AccNo == accno);

           if (account == null)
            {
                Logger.LogErrorMessage(className, methodName, "", $"Account: {accno} is not found", Logger.ERROR);
                return NotFound();
            }            

            return Ok(account);
        }
    }
}

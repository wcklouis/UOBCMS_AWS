using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.api
{
    public class body_cms_account_dkq
    {
        [Key]
        public int Id { get; set; }

        public int Acc_id { get; set; }

        public string Accno { get; set; }

        public string TrdExpOnexch_RelevantFinInst { get; set; }
        public string TrdExpOnexch_ProdType { get; set; }

        public int TrdExpOnexch_TradingPeriod { get; set; }
        public string TrdExpOnexch_Yes { get; set; }
        public string TrdExpOnexch_No { get; set; }
        public string WkExpOnDerProd_Employer { get; set; }
        public string WkExpOnDerProd_Dept { get; set; }
        public string WkExpOnDerProd_Pos { get; set; }
        public int WkExpOnDerProd_WorkingYear { get; set; }
        public string WkExpOnDerProd_Yes { get; set; }
        public string WkExpOnDerProd_No { get; set; }
        public string Training_CoursesName { get; set; }
        public string Training_Yes { get; set; }
        public string Training_No { get; set; }
        public string Training_OrgInstName { get; set; }
        public DateTime Training_AttendanceDate { get; set; }
        public string Whatisderivative_BankDeposit { get; set; }
        public string Whatisderivative_StkOrWithOwnership { get; set; }
        public string Whatisderivative_Loan { get; set; }
        public string Whatisderivative_FinInstFmUndAsset { get; set; }
        public string DerRiskfactors_CptyRisk { get; set; }
        public string DerRiskfactors_LiquidityRisk { get; set; }
        public string DerRiskfactors_MarketRisk { get; set; }
        public string DerRiskfactors_All { get; set; }
        public string TypesOfFutures_IndFut { get; set; }
        public string TypesOfFutures_ComFut { get; set; }
        public string TypesOfFutures_CcyFut { get; set; }
        public string TypesOfFutures_All { get; set; }
        public string IslvgeDer_Yes { get; set; }
        public string IslvgeDev_No { get; set; }
        public string LossInExcessInitMarginFunds_Yes { get; set; }
        public string LossInExcessInitMarginFunds_No { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}

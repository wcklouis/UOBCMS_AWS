using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_market
    {
        [Key]
        public int Id { get; set; }

        public int Acc_id { get; set; }

        public string Mkt_code { get; set; }

        public string Chqnetoff { get; set; }

        public string ChqnetoffString
        {
            get
            {
                switch (Chqnetoff)
                {
                    case "Y":
                        return "Yes";
                    case "N":
                        return "No";
                    default:
                        return "";
                }
            }
        }

        public string Aecode { get; set; }

        public int Account_bank_id { get; set; }

        public string? Bank_for_holdfund { get; set; }

        public string Bank_for_holdfundString
        {
            get
            {
                switch (Bank_for_holdfund)
                {
                    case "Y":
                        return "Yes";
                    case "N":
                        return "No";
                    default:
                        return "";
                }
            }
        }

        public string? Sett_mtd { get; set; }

        public string Sett_mtdString
        {
            get
            {
                switch (Sett_mtd)
                {
                    case "0":
                        return "BANKAC";
                    default:
                        return "";
                }
            }
        }
        public string? Clearing_cparty_pid { get; set; }
        public string? Clearing_clnt_acc { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
        public virtual cms_account_bank Cms_account_bank { get; set; }

        public virtual ICollection<cms_account_market_limit> Cms_account_market_limits { get; set; } = new List<cms_account_market_limit>();
        public virtual ICollection<cms_account_market_interest> Cms_account_market_interests { get; set; } = new List<cms_account_market_interest>();

        public virtual ICollection<cms_account_market_brokerage> Cms_account_market_brokerages { get; set; } = new List<cms_account_market_brokerage>();

        public virtual ICollection<cms_account_market_price_cap> Cms_account_market_price_caps { get; set; } = new List<cms_account_market_price_cap>();

        public virtual ICollection<cms_account_market_inscat> Cms_account_market_inscats { get; set; } = new List<cms_account_market_inscat>();

        public virtual ICollection<cms_account_market_cash_si> Cms_account_market_cash_sis { get; set; } = new List<cms_account_market_cash_si>();
        public virtual ICollection<cms_account_market_inst_si> Cms_account_market_inst_sis { get; set; } = new List<cms_account_market_inst_si>();

        public virtual ICollection<cms_account_market_confo> Cms_account_market_confos { get; set; } = new List<cms_account_market_confo>();

        public virtual ICollection<cms_account_market_rebate> Cms_account_market_rebates { get; set; } = new List<cms_account_market_rebate>();
    }
}

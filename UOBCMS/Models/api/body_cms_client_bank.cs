using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.api
{
    public class body_cms_client_bank
    {
        [Key]
        public int Id { get; set; }
        public string Bank_type { get; set; }
        public string Bank_name { get; set; }
        public string Bank_code { get; set; }
        public string Accno { get; set; }
        public string Ccy { get; set; }
        public string Bank_accno { get; set; }
        public string DefaultLocalHKDBank { get; set; }

        public string Bene_bank_swift { get; set; }
        public string Bene_bank_address { get; set; }
        public string Bene_bank_accname { get; set; }

        public string Bene_bank_accno { get; set; }
        public string Bene_bank_name { get; set; }

        public string Bene_ccy { get; set; }
        public string Corr_bank_name { get; set; }
        public string Corr_bank_swift { get; set; }

        public string Status { get; set; }

        public string Default_payment_type { get; set; }

        public string Fps_payment_type { get; set; }

        public string Bank_accname { get; set; }
        public string? Other_bank_details { get; set; }
        public string? Bene_bank_country { get; set; }
        public string? Corr_bank_country { get; set; }
        public string? Bene_bank_code { get; set; }
        public string? Corr_bank_code { get; set; }
        public string? Msg_to_bank { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }
    }
}

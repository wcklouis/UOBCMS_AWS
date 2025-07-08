using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models
{
    public class cms_account_w8
    {
        [Key]
        public int Id { get; set; }

        public int Acc_id { get; set; }
        public string Name_of_bene_owner { get; set; }

        public string Country_of_citizenship { get; set; }

        public string Residence_addr1 { get; set; }

        public string Residence_addr2 { get; set; }

        public string Residence_country { get; set; }

        public string Mailing_addr1 { get; set; }

        public string Mailing_addr2 { get; set; }

        public string Mailing_country { get; set; }

        public string Us_taxpayer_idnos { get; set; }

        public string Foreign_tax_idnos { get; set; }

        public string Chk_FTIN_not_legally_req { get; set; }

        public string Ref_nos { get; set; }

        public DateTime Dob { get; set; }

        public string Treaty_claim_tax_country { get; set; }

        public string Treaty_spec_rate_and_condition { get; set; }

        public Decimal Treaty_percent { get; set; }

        public string Treaty_withholding_on { get; set; }

        public string Treaty_explain { get; set; }

        public string Certify { get; set; }

        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public virtual cms_account Cms_account { get; set; }
    }
}

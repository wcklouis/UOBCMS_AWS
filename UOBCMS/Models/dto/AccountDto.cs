using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.dto
{
    public class AccountDto
    {
        [Key]
        public int Id { get; set; }

        public string Cif { get; set; }
        public string AccNo { get; set; }
        public string Type { get; set; }
        public string TypeString { get; set; }
        public string Ename { get; set; }
        public string Cname { get; set; }
        public string Status { get; set; }
        public string StatusString { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }
        public int Version { get; set; }
        public string Dbopr { get; set; }

        public List<AccountPhoneDto> Phones { get; set; }

        public List<AccountEmailDto> Emails { get; set; }

        public List<AccountAddressDto> Addresses { get; set; }
    }
}

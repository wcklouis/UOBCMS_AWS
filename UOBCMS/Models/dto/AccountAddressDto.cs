namespace UOBCMS.Models.dto
{
    public class AccountAddressDto
    {
        public int Id { get; set; }
        public int Client_address_id { get; set; }
        public int Acc_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }

        public ClientAddressDto ClientAddress { get; set; }
    }
}

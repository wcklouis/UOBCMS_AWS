namespace UOBCMS.Models.dto
{
    public class AccountPhoneDto
    {
        public int Id { get; set; }
        public int Client_phone_id { get; set; }
        public int Acc_id { get; set; }

        public ClientPhoneDto ClientPhone { get; set; }
    }
}

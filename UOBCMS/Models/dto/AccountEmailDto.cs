namespace UOBCMS.Models.dto
{
    public class AccountEmailDto
    {
        public int Id { get; set; }
        public int Client_email_id { get; set; }
        public int Acc_id { get; set; }
        public string Lastupdateuserid { get; set; }
        public DateTime Lastupdatedatetime { get; set; }

        public ClientEmailDto ClientEmail { get; set; }
    }
}

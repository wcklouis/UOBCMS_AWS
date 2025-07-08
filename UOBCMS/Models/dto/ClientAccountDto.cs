namespace UOBCMS.Models.dto
{
    public class ClientAccountDto
    {
        public int Client_id { get; set; }
        // Include any other necessary properties

        public int Acc_id { get; set; }

        public AccountDto Account { get; set; }
    }
}

namespace UOBCMS.Models.IBO
{
    public class UserIDContact
    {
        public string UserID { get; set; }
        public int SeqNo { get; set; }
        public string ContactType { get; set; }
        public string Details { get; set; }
        public string Active { get; set; }
        public Byte[] Timestamp { get; set; }
    }
}

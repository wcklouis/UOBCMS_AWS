using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.IBO
{
    public class ClntContact
    {
        public string ClntCode { get; set; }
        public int SeqNo { get; set; }
        public string ContactType { get; set; }
        public string Details { get; set; }
        public string BatchConfirmation { get; set; }
        public string RealTimeConfirmation { get; set; }
        public int ConfCode { get; set; }
        public string Attention { get; set; }
        public string Active { get; set; }
        public string Company { get; set; }
        public Byte[] Timestamp { get; set; }
    }
}

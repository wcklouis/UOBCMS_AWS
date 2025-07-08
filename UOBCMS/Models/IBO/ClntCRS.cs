using System.ComponentModel.DataAnnotations;

namespace UOBCMS.Models.IBO
{
    public class ClntCRS
    {
        public string ClntCode { get; set; }
        public int SeqNo { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Estimation { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string AccType { get; set; }
        public byte[] Timestamp { get; set; }
    }
}


namespace SMS_Tool
{
    public class SmsTable
    {
        public string Name {get;set;}
        public string MemberCode { get; set; }

        public string LoanCode { get; set; }
        public string MobileNo { get; set; }
        public string DueDate { get; set; }
        public bool SendSMS { get; set; }

    }
}

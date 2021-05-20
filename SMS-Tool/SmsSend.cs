using System;
using System.Net.Http;


namespace SMS_Tool
{
    class SmsSend
    {
        static HttpClient client = new HttpClient();
        public string template_id {get;set;}= "1207162098847501879";
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public string senderid { get; set; } = "";


        public async void send(string MobileNo,string LoanScheme,string DueDate,string acc_no,string cust_id)
        {
            System.Net.Http.HttpResponseMessage response;
            string responseString = "";
            //"Your {#var#} due on {#var#} come before 2 days early or contact CEO BALELE PACS"
            string message = "Your " + LoanScheme + " due on " + DueDate + " come before 2 days early or contact CEO BALELE PACS";
            string URL = "http://weberleads.in/http-api.php?username=" + username + "&password=" + password + "&senderid=" + senderid + "&route=2&number=" + MobileNo + "&message=" + message + "&templateid=" + template_id;
            System.Diagnostics.Debug.WriteLine(URL);

            try
            {
                response = await client.GetAsync(URL);
                responseString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(response.StatusCode.ToString());

                if (response.StatusCode.ToString().Equals("OK"))
                {
                    DatabaseAccessor databaseAccessor = new DatabaseAccessor();
                    databaseAccessor.InsertSmsTable(MobileNo, message ,cust_id,acc_no);
                }
                System.Diagnostics.Debug.WriteLine(responseString);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }



        }
    }
}

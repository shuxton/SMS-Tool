using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace SMS_Tool
{
    public class DatabaseAccessor
    {
       static OdbcConnection DbConnection = new OdbcConnection("DSN=ibsbalele");

     
        public Dictionary<string,string> GetLoanSchema()
        {
            Dictionary<string, string> loanScheme = new Dictionary<string, string>();
            DbConnection.Open();
            OdbcCommand DbCommand = DbConnection.CreateCommand();
            try
            {
                
                DbCommand.CommandText = "SELECT CATG,SCM_CODE,SCM_DESC FROM schemast";
                OdbcDataReader DbReader = DbCommand.ExecuteReader();
                int fCount = DbReader.FieldCount;
                while (DbReader.Read())
                {
                    for (int i = 0; i < fCount; i++)
                    {
                        loanScheme.Add(DbReader.GetValue(++i).ToString() + " - " + DbReader.GetValue(++i).ToString(), DbReader.GetValue(i - 2).ToString());
                    }

                }
                DbReader.Close();
                
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            DbCommand.Dispose();
            DbConnection.Close();
            return loanScheme;
        }

        public List<SmsTable> GetSmsTable(string loanSchema,DateTime toDate,DateTime fromDate,int category)
        {
            List<SmsTable> table = new List<SmsTable>();
            DbConnection.Open();
            OdbcCommand DbCommand = DbConnection.CreateCommand();
            try
            {
               
                if (category == 1 || category == 5)
                    DbCommand.CommandText = "SELECT LON_CODE,END_DT,MNAME,RCONTACT,MBRMAST.MCODE FROM CCLNMAS INNER JOIN MBRMAST ON CCLNMAS.MCODE=MBRMAST.MCODE" +
                                           " WHERE END_DT > #" + fromDate.Date + "# AND END_DT < +#" + toDate.Date + "#";
                else
                    DbCommand.CommandText = "SELECT LON_CODE,END_DT,MNAME,RCONTACT,MBRMAST.MCODE FROM LOAN_MAS INNER JOIN MBRMAST ON LOAN_MAS.MCODE=MBRMAST.MCODE" +
                                       " WHERE END_DT > #" + fromDate.Date + "# AND END_DT < +#" + toDate.Date + "#";
                OdbcDataReader DbReader = DbCommand.ExecuteReader();
                int fCount = DbReader.FieldCount;
                while (DbReader.Read())
                {
                    for (int i = 0; i < fCount; i++)
                    {
                        if (DbReader.GetValue(i + 3).ToString().Length == 10)
                        {
                            table.Add(new SmsTable()
                            {
                                LoanCode = DbReader.GetValue(i).ToString(),
                                DueDate = DbReader.GetDate(++i).ToString("MM-dd-yyyy"),
                                Name = DbReader.GetValue(++i).ToString(),
                                MobileNo = DbReader.GetValue(++i).ToString(),
                                MemberCode = DbReader.GetValue(++i).ToString(),
                                SendSMS = true
                            });
                        }
                        else
                        {
                            i = i + 4;
                        }

                    }

                }
                DbReader.Close();
                
            }catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            DbCommand.Dispose();
            DbConnection.Close();
            return table;

        }

        public Dictionary<string, string> GetSmsCredentials()
        {
            Dictionary<string, string> credentials = new Dictionary<string, string>();
            DbConnection.Open();
            OdbcCommand DbCommand = DbConnection.CreateCommand();
            try
            {

                DbCommand.CommandText = "SELECT Sender_ID,User_ID,Password FROM sms_settings";
                OdbcDataReader DbReader = DbCommand.ExecuteReader();
                
                int fCount = DbReader.FieldCount;
                while (DbReader.Read())
                {
                    
                    credentials.Add("senderid", DbReader.GetValue(0).ToString());
                    credentials.Add("userid", DbReader.GetValue(1).ToString());
                    credentials.Add("password", DbReader.GetValue(2).ToString());


                }
                DbReader.Close();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            DbCommand.Dispose();
            DbConnection.Close();
            return credentials;
        }


        public int InsertSmsTable(string mobile_no, string message,string custid,string acc_no)
        {
            int affected = 0;
            DbConnection.Open();
            OdbcCommand DbCommand = DbConnection.CreateCommand();
            try
            {

                DbCommand.CommandText = "INSERT INTO sms_send (mobile_no,custid,sms,gen_dt,gen_dttime,module,ac_no)" +
                    " VALUES ('"+mobile_no + "','" + custid+ "','" + message + "',#" + DateTime.Now.Date + "#,'" + DateTime.Now.ToString() + "','SMS_Tool','" + acc_no + "')";
                affected = DbCommand.ExecuteNonQuery();
                System.Diagnostics.Debug.WriteLine(affected);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            DbCommand.Dispose();
            DbConnection.Close();
            return affected;

        }
    }
}

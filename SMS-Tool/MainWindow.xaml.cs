using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace SMS_Tool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public static List<SmsTable> table = new List<SmsTable>();
        public static Dictionary<string, bool> flag = new Dictionary<string, bool>();
        public static string loanScheme = "";


        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (loanSchemeInput.SelectedValue != null && toDateInput.SelectedDate != null && fromDateInput.SelectedDate != null)
              {
                loanScheme = loanSchemeInput.SelectedValue.ToString();
                //.Split(' ')[0].Trim();
         
                var toDate = toDateInput.SelectedDate ?? DateTime.Now;
                var fromDate = fromDateInput.SelectedDate ?? DateTime.Now;

                DatabaseAccessor databaseAccessor = new DatabaseAccessor();

                var data = databaseAccessor.GetLoanSchema();
                string category;
                System.Diagnostics.Debug.WriteLine(data.TryGetValue(loanScheme, out category));
                table = databaseAccessor.GetSmsTable(loanScheme.Split(' ')[0].Trim(),toDate.Date,fromDate.Date,int.Parse(category));
                if (table.Count == 0)
                {
                    smsTableOutput.ItemsSource = null;
                    smsTableOutput.Visibility = Visibility.Hidden;
                    smsButton.IsEnabled = false;
                    smsButton.Visibility = Visibility.Hidden;

                    MessageBox.Show("No users found in this list");
                }
                else
                {
                    smsTableOutput.Visibility = Visibility.Visible;
                    smsTableOutput.ItemsSource = table;
                    smsButton.IsEnabled = true;
                    smsButton.Visibility = Visibility.Visible;
                }


            }

            else
            {
                MessageBox.Show("Error: One or more fields is empty");
            }
           

        }

        private void loanSchemeLoad(object sender, EventArgs e)
        {
            DatabaseAccessor databaseAccessor = new DatabaseAccessor();
            
            var data = databaseAccessor.GetLoanSchema();
            foreach(var scheme in data)
            {
                loanSchemeInput.Items.Add(scheme.Key);
            }
            
        }

       

        private void SmsButton_Click(object sender, RoutedEventArgs e)
        {
            if (table.Count < 1) return;
            DatabaseAccessor databaseAccessor = new DatabaseAccessor();
            Dictionary<string, string> credentials = databaseAccessor.GetSmsCredentials();
            SmsSend smsSend = new SmsSend();
            smsSend.senderid = credentials["senderid"];
            smsSend.username = credentials["userid"];
            smsSend.password = credentials["password"];

            foreach (var item in table)
            {
                if (flag.ContainsKey(item.LoanCode) && !flag[item.LoanCode])
                {
                    System.Diagnostics.Debug.WriteLine("SMS was not sent to " + item.LoanCode);
                    continue;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SMS Sent to " + item.LoanCode+" "+ loanScheme.Substring(6));
                    
                   smsSend.send(item.MobileNo, loanScheme.Substring(6), item.DueDate, item.LoanCode, item.MemberCode);
                }
            }

            MessageBox.Show("Operation Complete: SMS Sent!");




        }

        private void editable(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid x = sender as DataGrid;

            if(e.Column.Header.ToString().Equals("SendSMS"))
            {
                var checkBox = x.Columns[5].GetCellContent(x.Items[x.SelectedIndex]) as CheckBox;
                TextBlock txt = x.Columns[2].GetCellContent(x.Items[x.SelectedIndex]) as TextBlock;
                bool checkBoxStatus = checkBox.IsChecked ?? true;
    
                string loanCode = txt.Text ?? "";
                if (!checkBoxStatus)
                {
                    
                    if (!flag.ContainsKey(loanCode))
                    {
                        flag.Add(loanCode, false);
                    }

                }
                else
                {
                    if (flag.ContainsKey(loanCode))
                    {
                        flag[loanCode] = true;
                    }
                }
                

            }


        }
    }
}

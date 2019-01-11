using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Alexa.NET;
using Alexa.NET.Request.Type;
using System.Collections.Generic;
using System.Xml;
using Microsoft.SyndicationFeed.Rss;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace AlexaBanking
{
    static class Global
    {
        public static String user = null;

        public static String GlobalVar
        {
            get { return user; }
            set { user = value; }
        }
        //Session["user"]="he";
    }
    public static class Function1
    {
        [FunctionName("Alexa")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            string json = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(json);

            var requestType = skillRequest.GetRequestType();

            SkillResponse response = null;

            if (requestType == typeof(LaunchRequest))
            {
                response = ResponseBuilder.Tell("Welcome to your alexa banking interface!"+" To log in say your pin.");
                response.Response.ShouldEndSession = false;               
            }

            else if (requestType == typeof(IntentRequest))
            { 
                
                var intentRequest = skillRequest.Request as IntentRequest;
                if (intentRequest.Intent.Name == "CheckBalance")
                {
                    if (Global.GlobalVar != null)
                    {
                        //string output = $"Your balance is {Global.GlobalVar}";
                        string[] lines;
                        var list = new List<string>();
                        var fileStream = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", FileMode.Open, FileAccess.Read);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list.Add(line);
                            }
                        }
                        lines = list.ToArray();


                        foreach (string s in lines)
                        {
                            if (s.StartsWith("balance"))
                            {
                                string balance_str = new String(s.Where(Char.IsDigit).ToArray());
                                int balance = Int32.Parse(balance_str);
                                response = ResponseBuilder.Tell("Your current balance is Rs " + balance_str + ". Do you want to make another transaction?");
                            }
                        }
                        response.Response.ShouldEndSession = false;
                        //response = ResponseBuilder.Tell(output);
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if (intentRequest.Intent.Name == "WithdrawMoney")
                {
                    //string amt = intentRequest.Intent.Slots["amount"].Value;
                    //response = ResponseBuilder.Tell(amt);
                    if (Global.user != null)
                    {
                        string[] lines;
                        var list = new List<string>();
                        var fileStream = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", FileMode.Open, FileAccess.Read);
                        var fileStream_temp = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        var streamWriter_temp = new StreamWriter(fileStream_temp, Encoding.UTF8);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list.Add(line);
                            }
                        }
                        lines = list.ToArray();


                        foreach (string s in lines)
                        {
                            if (s.StartsWith("balance"))
                            {
                                string balance_str = new String(s.Where(Char.IsDigit).ToArray());
                                int balance = Int32.Parse(balance_str);
                                string amt_str = intentRequest.Intent.Slots["amount"].Value;
                                int amt = Int32.Parse(amt_str);
                                int new_balance = balance - amt;
                                if (new_balance < 0)
                                {
                                    goto error;
                                }
                                string new_balance_str = new_balance.ToString();
                                streamWriter_temp.Write("balance = " + new_balance_str);
                                response = ResponseBuilder.Tell("Transaction successful. Rs " + amt_str + " have been withdrawn from your account and your current account balance is Rs " + new_balance_str + ". Do you want to execute another transaction?");
                            }
                            else
                            {
                                streamWriter_temp.WriteLine(s);
                            }
                        }
                        streamWriter_temp.Close();
                        //File.Replace("account.txt", "temp.txt", null);
                        File.Replace(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", @"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", null);

                        goto noerror;

                        error: response = ResponseBuilder.Tell("You have insufficient balance to execute this withdrawal. Do you want to continue?");
                        noerror: response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }

                else if(intentRequest.Intent.Name=="DepositMoney")
                {
                    if (Global.user != null)
                    {
                        string[] lines;
                        var list = new List<string>();
                        var fileStream = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", FileMode.Open, FileAccess.Read);
                        var fileStream_temp = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        var streamWriter_temp = new StreamWriter(fileStream_temp, Encoding.UTF8);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list.Add(line);
                            }
                        }
                        lines = list.ToArray();


                        foreach (string s in lines)
                        {
                            if (s.StartsWith("balance"))
                            {
                                string balance_str = new String(s.Where(Char.IsDigit).ToArray());
                                int balance = Int32.Parse(balance_str);
                                string amt_str = intentRequest.Intent.Slots["amount"].Value;
                                int amt = Int32.Parse(amt_str);
                                int new_balance = balance + amt;
                                string new_balance_str = new_balance.ToString();
                                streamWriter_temp.Write("balance = " + new_balance_str);
                                response = ResponseBuilder.Tell("Transaction successful. Rs " + amt_str + " have been added to your account and your current account balance is Rs " + new_balance_str + ". Do you want to execute another transaction?");
                            }
                            else
                            {
                                streamWriter_temp.WriteLine(s);
                            }
                        }
                        streamWriter_temp.Close();
                        //File.Replace("account.txt", "temp.txt", null);
                        File.Replace(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", @"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", null);
                        response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if(intentRequest.Intent.Name=="AMAZON.NoIntent")
                {
                    Global.GlobalVar = null;
                    response = ResponseBuilder.Tell("Thank you for using alexa banking. You have been logged out.");
                }
                else if(intentRequest.Intent.Name=="AMAZON.YesIntent")
                {
                    response = ResponseBuilder.Tell("Give me the next command to execute next transaction.");
                    response.Response.ShouldEndSession = false;
                }
                else if(intentRequest.Intent.Name=="BuyIntent")
                {
                    if (Global.user != null)
                    {
                        string[] lines;
                        var list = new List<string>();
                        var fileStream = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", FileMode.Open, FileAccess.Read);
                        var fileStream_temp = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        var streamWriter_temp = new StreamWriter(fileStream_temp, Encoding.UTF8);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list.Add(line);
                            }
                        }
                        lines = list.ToArray();


                        foreach (string s in lines)
                        {
                            if (s.StartsWith("balance"))
                            {
                                string balance_str = new String(s.Where(Char.IsDigit).ToArray());
                                int balance = Int32.Parse(balance_str);
                                string buyitem = intentRequest.Intent.Slots["buyitem"].Value;
                                int amt;
                                if (buyitem == "car")
                                {
                                    amt = 200000;
                                }
                                else if (buyitem == "mobile")
                                {
                                    amt = 10000;
                                }
                                else
                                {
                                    amt = 1000;
                                }
                                int new_balance = balance - amt;
                                if (new_balance < 0)
                                {
                                    goto error;
                                }

                                String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                                String Query = "INSERT INTO buyintent(item,amount) values (@buyitem,@amt)";
                                MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                                MySqlCommand Command = new MySqlCommand(Query);
                                Command.Parameters.AddWithValue("@buyitem", buyitem);
                                Command.Parameters.AddWithValue("@amt", amt);

                                Command.Connection = DBConnect;
                                MySqlDataReader Reader;
                                DBConnect.Open();
                                Reader = Command.ExecuteReader();

                                string new_balance_str = new_balance.ToString();
                                streamWriter_temp.Write("balance = " + new_balance_str);
                                response = ResponseBuilder.Tell("Transaction successful. Rs " + amt + " have been withdrawn from your account and your current account balance is Rs " + new_balance_str + ". Do you want to execute another transaction?");
                            }
                            else
                            {
                                streamWriter_temp.WriteLine(s);
                            }
                        }
                        streamWriter_temp.Close();
                        //File.Replace("account.txt", "temp.txt", null);
                        File.Replace(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", @"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", null);

                        goto noerror;

                        error: response = ResponseBuilder.Tell("You have insufficient balance to buy the requested product. Kindly add more balance to your account to be able to buy it. Do you want to continue?");
                        noerror: response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }

                }
                else if(intentRequest.Intent.Name=="TransactionIntent")
                {
                    if (Global.user != null)
                    {
                        String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                        String Query = "SELECT item,amount,time from buyintent order by srno desc limit 3";
                        MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                        MySqlCommand Command = new MySqlCommand(Query);

                        Command.Connection = DBConnect;
                        MySqlDataReader Reader;
                        DBConnect.Open();
                        Reader = Command.ExecuteReader();
                        String text = "Your last three transactions are as follows: \n You bought";
                        while (Reader.Read())
                        {
                            text = text + ", " + Reader.GetString(0) + " for Rupees " + Reader.GetString(1) + " on " + Reader.GetString(2);
                        }
                        text = text + ". Do you want to execute any other transaction or command?";
                        response = ResponseBuilder.Tell(text);
                        response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if(intentRequest.Intent.Name=="DetailsIntent")
                {
                    if (Global.user != null)
                    {
                        String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                        String Query = "SELECT Customerid,Name,Age,Address,Branch from accountdetails";
                        MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                        MySqlCommand Command = new MySqlCommand(Query);

                        Command.Connection = DBConnect;
                        MySqlDataReader Reader;
                        DBConnect.Open();
                        Reader = Command.ExecuteReader();
                        String text = "Here are your details. ";
                        while (Reader.Read())
                        {
                            text = text + "Customer ID: " + Reader.GetString(0) + " Name: " + Reader.GetString(1) + ", Age: " + Reader.GetString(2) + ", Address: " + Reader.GetString(3) + ", Branch: " + Reader.GetString(4);
                        }
                        text = text + ". Do you want to execute any other transaction or command?";
                        response = ResponseBuilder.Tell(text);
                        response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if(intentRequest.Intent.Name=="LastDaysIntent")
                {
                    if (Global.user != null)
                    {
                        DateTime myDateTime = DateTime.Now;
                        DateTime MyNewDateValue;
                        string prev = intentRequest.Intent.Slots["days"].Value;
                        int prev_int = Int32.Parse(prev);
                        MyNewDateValue = myDateTime.AddDays(-prev_int);
                        MyNewDateValue = DateTime.ParseExact(MyNewDateValue.ToString(), "dd-MM-yyyy HH:mm:ss", null);
                        //response = ResponseBuilder.Tell(MyNewDateValue.ToString());

                        String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                        String Query = "SELECT amount,time from buyintent";
                        MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                        MySqlCommand Command = new MySqlCommand(Query);

                        Command.Connection = DBConnect;
                        MySqlDataReader Reader;
                        DBConnect.Open();
                        Reader = Command.ExecuteReader();
                        int spend = 0;
                        DateTime time;
                        while (Reader.Read())
                        {
                            time = DateTime.ParseExact(Reader.GetString(1), "dd-MM-yyyy HH:mm:ss", null);
                            if (DateTime.Compare(time, MyNewDateValue) >= 0)
                            {
                                spend = spend + Int32.Parse(Reader.GetString(0));
                            }

                        }
                        String text = "You spent " + spend.ToString() + " rupees on amazon store in last " + prev + " days. Do you want to execute any other transaction or command?";
                        response = ResponseBuilder.Tell(text);
                        response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if (intentRequest.Intent.Name == "TransferIntent")
                {
                    if (Global.user != null)
                    {
                        string[] lines;
                        var list = new List<string>();
                        var fileStream = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", FileMode.Open, FileAccess.Read);
                        var fileStream_temp = new FileStream(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        var streamWriter_temp = new StreamWriter(fileStream_temp, Encoding.UTF8);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            string line;
                            while ((line = streamReader.ReadLine()) != null)
                            {
                                list.Add(line);
                            }
                        }
                        lines = list.ToArray();


                        foreach (string s in lines)
                        {
                            if (s.StartsWith("balance"))
                            {
                                string balance_str = new String(s.Where(Char.IsDigit).ToArray());
                                int balance = Int32.Parse(balance_str);
                                string amt = intentRequest.Intent.Slots["amount"].Value;
                                String account = intentRequest.Intent.Slots["accountnumber"].Value;
                                int amt_int = Int32.Parse(amt);
                                int account_int = Int32.Parse(account);

                                int new_balance = balance - amt_int;
                                if (new_balance < 0)
                                {
                                    goto error;
                                }

                                String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                                String Query = "INSERT INTO transferintent(account,amount) values (@account,@amt)";
                                MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                                MySqlCommand Command = new MySqlCommand(Query);
                                Command.Parameters.AddWithValue("@account", account_int);
                                Command.Parameters.AddWithValue("@amt", amt_int);

                                Command.Connection = DBConnect;
                                MySqlDataReader Reader;
                                DBConnect.Open();
                                Reader = Command.ExecuteReader();

                                string new_balance_str = new_balance.ToString();
                                streamWriter_temp.Write("balance = " + new_balance_str);

                                response = ResponseBuilder.Tell("Transaction successful. Rs " + amt + " have been transfered from your account to account number " + account + " and your current account balance is Rs " + new_balance_str + ". Do you want to execute another transaction?");
                            }
                            else
                            {
                                streamWriter_temp.WriteLine(s);
                            }
                        }
                        streamWriter_temp.Close();
                        //File.Replace("account.txt", "temp.txt", null);
                        File.Replace(@"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\temp.txt", @"C:\Users\CHINMAY\source\repos\AlexaBanking\AlexaBanking\account.txt", null);

                        goto noerror;

                        error: response = ResponseBuilder.Tell("You have insufficient balance to transfer this amount. Kindly add more balance to your account or try transferring a lower amount. Do you want to continue?");
                        noerror: response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if (intentRequest.Intent.Name == "AMAZON.HelpIntent")
                {
                    if (Global.user != null)
                    {
                        response = ResponseBuilder.Tell("Here is a list of functions I can do: You can ask me to check your balance, get your account details," +
                            " or transfer money to some other account. I can also buy stuff for you from the amazon store and provide your" +
                            " recent transactions and spendings.");
                        response.Response.ShouldEndSession = false;
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("Kindly say your pin to continue.");
                        response.Response.ShouldEndSession = false;
                    }
                }
                else if(intentRequest.Intent.Name == "PinIntent")
                {
                    
                    
                    if (Global.user == null)
                    {
                        String ConnectString = "datasource=localhost;port=3306;username=root;password=;database=alexa;";
                        String Query = "SELECT Password from accountdetails";
                        MySqlConnection DBConnect = new MySqlConnection(ConnectString);
                        MySqlCommand Command = new MySqlCommand(Query);

                        Command.Connection = DBConnect;
                        MySqlDataReader Reader;
                        DBConnect.Open();
                        Reader = Command.ExecuteReader();
                        Reader.Read();
                        int check = Int32.Parse(Reader.GetString(0));

                        int pin = Int32.Parse(intentRequest.Intent.Slots["pin"].Value);
                        if (pin == check)
                        {
                            Global.user = "Chinmay";
                            response = ResponseBuilder.Tell("Login Successful. Say a command to execute transaction. If you want a brief idea of what" +
                                " functions I can execute, just say help.");
                            response.Response.ShouldEndSession = false;
                        }
                        else
                        {
                            response = ResponseBuilder.Tell("Entered pin is incorrect. Kindly enter the correct pin to login.");
                            response.Response.ShouldEndSession = false;
                        }
                    }
                    else
                    {
                        response = ResponseBuilder.Tell("You are already logged in. You can execute your transactions.");
                        response.Response.ShouldEndSession = false;
                    }
                }
            }
            return new OkObjectResult(response);
        }

        /*private static async Task<List<string>> ParseFeed(string url)
        {
            List<string> news = new List<string>();
            using (var xmlReader = XmlReader.Create(url, new XmlReaderSettings { Async = true }))
            {
                RssFeedReader feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (feedReader.ElementType == Microsoft.SyndicationFeed.SyndicationElementType.Item)
                    {
                        var item = await feedReader.ReadItem();
                        news.Add(item.Title);
                    }
                }
            }
            return news;

        }*/
    }
}

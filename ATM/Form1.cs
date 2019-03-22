using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ATM
{
    public partial class Form1 : Form
    {
        //Create clients file if it doesn't exist
        public Form1()
        {
            InitializeComponent();
            System.IO.File.AppendAllText(@"Clients.txt", "");
            string[] check = System.IO.File.ReadAllLines(@"Clients.txt");

            try
            {
                bool when = check[0] == string.Empty;
            }catch(Exception exc)
            {
                System.IO.File.WriteAllText(@"Clients.txt", "Default~1111111~0000~0~0000000" + Environment.NewLine);
            }
        }
        //Variables
        string accountNo = "";
        string Pin = "";
        bool Access = false;
        Double convert = 1;
        int counter = 0;
        private string[] details;
        //Account number or user name text box
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            accountNo = t.Text;
        }
        //Log in button
        private void button1_Click(object sender, EventArgs e)
        {
            Pin = getNum(Pin);
            if (accountNo.Length < 1)
            {
                MessageBox.Show("Invalid Account Number");
            } else if (Pin.Length < 4)
            {
                MessageBox.Show("Invalid Pin Number");
            }
            else
            {
                bool found = false;
                string[] An = accountNo.Split(' ');
                string AccNo = String.Join("-", An);
                string[] clients = System.IO.File.ReadAllLines(@"Clients.txt");
                foreach (var client in clients)
                {
                    details = client.Split('~');
                    if (details[0].Equals(AccNo, StringComparison.CurrentCultureIgnoreCase) || details[1].Equals(accountNo))
                    {
                        found = true;
                        if (details[2].Equals(Pin)){
                            newWindow("LogedIn");
                            Access = true;
                            MessageBox.Show("Welcome " + details[0].Replace("-"," "), "Welcome");
                            details[3] = getNum(details[3]);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Pin", "Error");
                        }
                        
                    }
                    counter++;
                }
                if (!found)
                {
                    MessageBox.Show("Can not find such a user", "Error");
                }
            }
        }
        //Pin text box
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox t = (TextBox)sender;
            Pin = t.Text;
        }

        private void newWindow(string win)
        {
            if(win == "home") {
                button1.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                button3.Visible = true;
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
                button9.Visible = false;
                button10.Visible = false;
                button11.Visible = false;
                textBox3.Visible = false;
                label1.Visible = true;
                label2.Visible = true;
            }
            else if(win == "LogedIn")
            {
                button1.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                button3.Visible = false;
                button4.Visible = true;
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = true;
                button8.Visible = true;
                button9.Visible = true;
                button10.Visible = true;
                button11.Visible = false;
                textBox3.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
            }
            else if(win == "Statement"){
                button1.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                button5.Visible = false;
                button6.Visible = false;
                button7.Visible = false;
                button8.Visible = false;
                button9.Visible = false;
                button10.Visible = false;
                button11.Visible = true;
                textBox3.Visible = true;
                label1.Visible = false;
                label2.Visible = false;
            }
           
        }
        //Fast withdrawal
        private void button4_Click(object sender, EventArgs e)
        {
            if(Access == true)
            {
                if(double.Parse(details[3]) < 100)
                {
                    MessageBox.Show("Insufficient funds.\nYour Balance is " + Math.Round(double.Parse(details[3]) * convert, 2)+" "+ comboBox1.Text.ToString());
                }
                else
                {
                    try
                    {
                        DialogResult ans = MessageBox.Show("Are You sure you want to withdraw " + (100 * convert) + " "+comboBox1.Text.ToString(), "Confirmation", MessageBoxButtons.YesNo);
                        if (ans == DialogResult.Yes)
                        {
                            change(100, "Withdraw");
                            recordLog(100, "Withdrawal");
                            MessageBox.Show("You have withdrawn " + (100 * convert) + "\nNew Account balance is "+ Math.Round(double.Parse(details[3]) * convert, 2)+" "+ comboBox1.Text.ToString());
                        }
                    }
                    catch(Exception exc) { }
                }
            }
        }
        //Deposit button
        private void button5_Click(object sender, EventArgs e)
        {
            if (Access)
            {
                string amt = "0";
                try
                {
                    amt = Interaction.InputBox("Enter amount with no commas or spaces", "Enter Amount");
                    amt = getNum(amt);
                    change(double.Parse(amt) / convert, "Deposit");
                    recordLog(double.Parse(amt) / convert, "Deposit");
                    MessageBox.Show("You have deposited " + amt + comboBox1.Text + ".\nNew account balance is " + Math.Round(double.Parse(details[3]) * convert, 2), "Confirmation");
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Failed to deposit", "Error");
                }
            }
        }
        //Withdrawal Button
        private void button6_Click(object sender, EventArgs e)
        {
            if (Access)
            {
                string amt;
                try
                {
                    amt = Interaction.InputBox("Enter amount with no commas or spaces", "Enter Amount");
                    amt = getNum(amt);
                    if (double.Parse(details[3])/convert < (double.Parse(amt)))
                    {
                        MessageBox.Show("Insufficient funds.\nYour Balance is " + Math.Round(double.Parse(details[3]) * convert, 2) + " "+comboBox1.Text.ToString());
                    }
                    else
                    {
                        DialogResult ans = DialogResult.No;
                        ans = MessageBox.Show("Are You sure you want to withdraw " + amt, "Confirmation", MessageBoxButtons.YesNo);

                        if (ans == DialogResult.Yes)
                        {
                            recordLog(int.Parse(amt)/convert, "Withdrawal");
                            change(int.Parse(amt)/convert, "Withdraw");
                            MessageBox.Show("You have withdrawn " + amt + "\nYour new Account balance is "+ Math.Round(double.Parse(details[3]) * convert, 2) + " " + comboBox1.Text.ToString());
                        }

                    }
                }
                catch(Exception exc)
                {
                    amt = "";
                }
            }
        }
        //Transfer button
        private void button7_Click(object sender, EventArgs e)
        {
            if (Access)
            {
                string amt;
                try
                {
                    amt = Interaction.InputBox("Enter amount with no commas or spaces", "Enter Amount");
                    amt = getNum(amt);
                    if (double.Parse(amt) > double.Parse(details[3])/convert)
                    {
                        MessageBox.Show("You have insufficient balance.\nYour account balance is " + Math.Round(double.Parse(details[3]) * convert, 2) + " " + comboBox1.Text.ToString(), "Insufficient Balance");
                    }
                    else
                    {
                        int InCounter = 0;
                        string accNo;
                        bool found = false;
                        accNo = Interaction.InputBox("Enter account number", "Enter Amount");
                        string[] clients = OpenFile();
                        foreach (var client in clients)
                        {
                            if (details[1] == accNo.Trim())
                            {
                                found = true;
                                MessageBox.Show("You have entered your account number", "Invalid Account Number");
                                break;
                            }
                            string[] Tdetails = client.Split('~');
                            if (Tdetails[1] == accNo.Trim())
                            {
                                DialogResult ans = DialogResult.No;
                                ans = MessageBox.Show("Are You sure you want to transfer " + amt, "Confirmation", MessageBoxButtons.YesNo);

                                if (ans == DialogResult.Yes)
                                {
                                    found = true;
                                    change(double.Parse(amt)/convert, "Deposit", InCounter);
                                    change(double.Parse(amt)/ convert, "Withdraw");
                                    recordLog(double.Parse(amt)/ convert, "TransferOut");
                                    recordLog(double.Parse(amt)/ convert, "TransferIn", InCounter);
                                    RefreshDetails();
                                    MessageBox.Show("You have transferred " + amt + " to " + Tdetails[0].Replace("-"," ") + "\nNew account balance is " + Math.Round(double.Parse(details[3]) * convert, 2) + " " + comboBox1.Text.ToString(), "Confirmation");
                                }
                            }
                            InCounter++;
                        }
                        if (!found)
                        {
                            MessageBox.Show("No such client Exists.", "Account Number not found");
                        }

                    }
                }
                catch (Exception exc)
                {
                    amt = "0";
                }
                
            }
        }
        //View balance
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Your Account balance is " + (Math.Round(double.Parse(details[3])) * convert) + " " + comboBox1.Text, "Account Balance");
            }catch(Exception exc)
            {
                MessageBox.Show("An erro occurred. We can not access your acount.\nTry loging in afresh", "Error");
            }
            
        }
        //Account statement
        private void button9_Click(object sender, EventArgs e)
        {
            if (Access)
            {
                try
                {
                    textBox3.Text += "Account   Date/Time                     Transaction   Amount         Balance";
                    string[] logs = System.IO.File.ReadAllLines(@"Log.txt");
                    foreach (var log in logs)
                    {
                        string[] line = log.Split('~');
                        if (details[1] == line[0])
                        {
                            string formated = string.Join("         ", line);
                            textBox3.Text += Environment.NewLine +formated + Environment.NewLine;
                        }
                    }
                    newWindow("Statement");
                }
                catch(Exception exc)
                {
                    textBox3.Clear();
                    MessageBox.Show("No transactions yet","No records");
                }
            }
        }
        //DOne button
        private void button10_Click(object sender, EventArgs e)
        {
            Access = false;
            newWindow("home");
            textBox1.Clear();
            textBox2.Clear();
            convert = 1;
            counter = 0;
        }
        //Carries out changes made to an account.
        private void change(double amt, string type, int InCounter = -1)
        {
            amt = Math.Round(amt,2);
            if(InCounter == -1)
            {
                InCounter = counter;
            }

            string[] clients = System.IO.File.ReadAllLines(@"Clients.txt");
            details = clients[InCounter].Split('~');
            double NewAmt;
            if(type == "Withdraw")
            {
                NewAmt = double.Parse(details[3]) - amt;
            }
            else
            {
                NewAmt = double.Parse(details[3]) + amt;
            }
            
            details[3] = NewAmt.ToString();
            clients[InCounter] = string.Join("~", details);
            System.IO.File.WriteAllLines(@"Clients.txt", clients);
        }
        //Open clients file
        private string[] OpenFile()
        {
            return System.IO.File.ReadAllLines(@"Clients.txt");
        }
        private void recordLog(double amt, string type, int InCounter = -1)
        {
            int space = 11 - type.Length;
            if(type.Trim() == "Deposit")
            {
                space += 6;
            }
            for(;space > 0; space--)
            {
                type = type + " ";
            }
            amt = Math.Round(amt, 2);
            if(InCounter >= 0){
                string[] Clients = OpenFile();
                details = Clients[InCounter].Split('~');
            }
            int space2 = 15 - (amt.ToString()).Length;
            string amts = amt.ToString();
            for (; space2 > 0; space2--)
            {
                amts = amts + " ";
            }
            string date = DateAndTime.Today.ToString();
            string log = "\n"+details[1] +"~"+ date + "~" + type + "~" + amts + "~" + details[3];
            System.IO.File.AppendAllText(@"Log.txt", log + Environment.NewLine);
        }
        //Back button
        private void button11_Click(object sender, EventArgs e)
        {
            newWindow("LogedIn");
            textBox3.Clear();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            
        }
        //Retrieves the clients info
        private void RefreshDetails()
        {
            String[] clients = OpenFile();
            details = clients[counter].Split('~');
        }
        //Register button
        private void button3_Click(object sender, EventArgs e)
        {
            string name;
            try
            {
                name = Interaction.InputBox("Enter your full name as in your ID", "Register new account");
                if (name != string.Empty)
                {
                    string ID;
                    ID = Interaction.InputBox("Enter your ID card number", "Register new account");

                    if (ID == string.Empty)
                    {
                        MessageBox.Show("Invalid ID", "Error");
                    }else if (exists(ID))
                    {
                        MessageBox.Show("User already exists","Error");
                    }else
                    {
                        string pin;
                        pin = Interaction.InputBox("Enter a four digit access code", "Register an account");

                        if (pin.Length != 4)
                        {
                            MessageBox.Show("Invalid Pin", "Error");
                        }
                        else
                        {
                            string[] clients = OpenFile();
                            string[] NewDetails = clients[0].Split('~');
                            int AccNo = int.Parse(NewDetails[1]);
                            bool Match;
                            do
                            {
                                Match = false;
                                foreach (var client in clients)
                                {
                                    NewDetails = client.Split('~');
                                    if (AccNo == int.Parse(NewDetails[1]))
                                    {
                                        AccNo += 1;
                                        Match = true;
                                        break;
                                    }
                                }
                            } while (Match);
                            string[] names = name.Split(' ');
                            name = string.Join("-", names);
                            string NewDetail = name + "~" + AccNo + "~" + pin + "~"+"0"+"~"+ID;
                            System.IO.File.AppendAllText(@"Clients.txt", NewDetail + Environment.NewLine);
                            MessageBox.Show("Your account number is " + AccNo + ".\nPlease take note of it.");
                            DialogResult ans = MessageBox.Show("Do you want to log in now", "Make first deposit", MessageBoxButtons.YesNo);

                            if (ans == DialogResult.Yes)
                            {
                                textBox1.Text = AccNo.ToString();
                                textBox2.Text = pin;
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                name = "";
            }
            
        }
     
        private void button2_Click(object sender, EventArgs e)
        {
            //string change = Interaction.Choose();
        }

        public enum currency {USD, EUR, CNY, JPY, RUB, HKD}
        //Convert the value of the currency according to the users selection
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Text == "USD")
            {
                convert = 1;
            }else if(comboBox1.Text == "EUR")
            {
                convert = 0.83;
            }else if(comboBox1.Text == "RUB")
            {
                convert = 56.98;
            }
            else if(comboBox1.Text == "CNY")
            {
                convert = 6.49;
            }
            else if (comboBox1.Text == "JPY")
            {
                convert = 112.72;
            }
            else if(comboBox1.Text == "HKD")
            {
                convert = 7.82;
            }
            else if (comboBox1.Text == "GBP")
            {
                convert = 1.35;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void Form_1KeyPressed(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("You clicked");
        }

        private void Form_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        //Checks if current ID exists to prevent duplocate records.
        private bool exists(string NewAcc)
        {
            bool existing = false;
            string[] clients = OpenFile();
            foreach (var client in clients)
            {
                string[] NewClients = client.Split('~');
                if(NewClients[4] == NewAcc.Trim())
                {
                    existing = true;
                    break;
                }
            }
            return existing;
        }
        private string getNum(string input)
        {
            return new string(input.Where(char.IsDigit).ToArray());
        }

        private void Key_Pressed(object sender, KeyPressEventArgs e)
        {
            if(Convert.ToInt32(e.KeyChar)== 13 && textBox2.Text.Length == 0)
            {
                textBox2.Focus();
            }else if (Convert.ToInt32(e.KeyChar) == 13 && textBox2.Text.Length > 0)
            {
                button1.Focus();
            }
        }

        private void Key_Pressed2(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 13 && textBox2.Text.Length == 0)
            {
                button1.Focus();
            }
        }
    }

}

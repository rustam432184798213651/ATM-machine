using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;

namespace ATM_machine
{
    internal class ORM
    {
        /*
            Insert new account into table accounts
        */
        private ORM() { this.GetConnection(); }
        static  ORM objectForStoringDatabase = new ORM { };
        public static ORM GetInstance() { return objectForStoringDatabase; }
        public void WithdrawMoneyFromAccountNumber(string currentUser, string currentAccountNumber, string amountOfMoneyToWithdraw)
        {
            database.Open();
            NpgsqlCommand commandToExecute = new NpgsqlCommand
            {
                Connection = database,
                CommandText = "UPDATE " + nameOfTableForAccountNumbers + " SET balance = balance - " + amountOfMoneyToWithdraw + " WHERE account_name = " + "'"+currentUser+"'"+" AND " + "account_number = " + "'" +currentAccountNumber+"'"+";"
            };
            commandToExecute.ExecuteNonQuery();
            database.Close();
        }

        public void InsertNewAccount(string nameOfAccount, string passwordOfAccount)
        {
            database.Open();
            NpgsqlCommand commandToExecute = new NpgsqlCommand
            {
                Connection = database,
                CommandText = "INSERT INTO " + nameOfTable + " (name_of_account, password_of_account) VALUES ('" + nameOfAccount + "' , '" + passwordOfAccount + "')"

            };
            commandToExecute.ExecuteNonQuery();
            database.Close();
        }
        public bool CheckIfTheUserNameInDatabase(string nameOfAccount)
        {
            database.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from accounts where name_of_account = '" + nameOfAccount + "';", database);
            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();
            
            if (dr.HasRows)
            {
                database.Close();
                return true;
            }
            database.Close();
            return false;
        }
        public int GetBalance(string accountName, string accountNumber)
        {
            database.Open();
            NpgsqlCommand commandToExecute = new NpgsqlCommand
            {
                Connection = database,
                CommandText = "SELECT account_balance FROM " + nameOfTableForAccountNumbers + " WHERE account_name =" + "'"+accountName+"'"+" "+"AND"+" "+"account_number ="+" "+"'"+accountNumber+"'"+";"

            };
            NpgsqlDataReader dr = commandToExecute.ExecuteReader();
            dr.Read();
            int balance = (int)dr[0];
            database.Close();
            return balance;

        }
        public bool InsertAccountNumberIntoTableOfAccountNUmbers(string accountName, string accountNumber, string pincode)
        {
            database.Open();
            NpgsqlCommand commandToExecute = new NpgsqlCommand
            {
                Connection = database,
                CommandText = "INSERT INTO " + nameOfTableForAccountNumbers + " (account_name, account_number, account_pincode, account_balance) VALUES ('" + accountName + "' , '" + accountNumber + "', '"+pincode+"', 0)"

            };
            NpgsqlDataReader dr = commandToExecute.ExecuteReader();
            dr.Read();

            if (dr.HasRows)
            {
                database.Close();
                return true;
            }
            database.Close();
            return false;
        }
        public bool CheckIfTheAccountNumberInTableOfAccountNumbers(string username, string accountNumber)
        {
            database.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from "+ nameOfTableForAccountNumbers +" where account_number = '" + accountNumber + "' "+"AND account_name = '"+username+"'"+";", database);
            NpgsqlDataReader dr = command.ExecuteReader();
            dr.Read();

            if (dr.HasRows)
            {
                database.Close();
                return true;
            }
            database.Close();
            return false;
        }
     
        public bool CheckIfThePasswordSuitUsername(string nameOfAccount, string passwordOfAccount) 
        {
            database.Open();
            NpgsqlCommand command = new NpgsqlCommand("select * from accounts where name_of_account = '" + nameOfAccount + "' AND password_of_account = '" + passwordOfAccount + "' ;", database);
            NpgsqlDataReader table = command.ExecuteReader();
            table.Read();

            if (table.HasRows)
            {
                database.Close();
                return true;
            }
            database.Close();
            return false;
        }
        /*
            Get connection with database in pgAdmin 4
        */
        public void GetConnection()
        {
            //   string name_of_database = "DatabaseForAccounts";
            //   string password = "megSAKDjdi2oj3id129jfdap";
            //   string username = "postgres";
            database = new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=megSAKDjdi2oj3id129jfdap;Database=DatabaseForAccounts");
        }
        private const string nameOfDatabase = "DatabaseForAccounts";
        private const string nameOfTable = "accounts";
        private const string nameOfTableForAccountNumbers = "account_numbers";
        private NpgsqlConnection database;
    }
}

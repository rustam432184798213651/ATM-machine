using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;

namespace ATM_machine
{
    internal class ORM
    {
        /*
            Insert new account into table accounts
        */
        private void Insert_new_account(string name_of_account, string password_of_account)
        {
            database.Open();
            NpgsqlCommand commandToExecute = new NpgsqlCommand
            {
                Connection = database,
                CommandText = "INSERT INTO " + name_of_database + " (name_of_account, password_of_account) VALUES (" + name_of_account + "," + "password_of_account" + ")"

            };
            commandToExecute.ExecuteNonQueryAsync();
        }
        /*
            Get connection with database in pgAdmin 4
        */
        private void GetConnection()
        {
            //   string name_of_database = "DatabaseForAccounts";
            //   string password = "megSAKDjdi2oj3id129jfdap";
            //   string username = "postgres";
            database = new NpgsqlConnection(@"Server=localhost;Port=5432;User Id=postgres;Password=megSAKDjdi2oj3id129jfdap;Database=DatabaseForAccounts");
        }
        private const string name_of_database = "DatabaseForAccounts";
        private NpgsqlConnection database;
    }
}

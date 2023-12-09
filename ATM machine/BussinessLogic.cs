using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ATM_machine
{
    internal class CurrentLogic
    {
        public CurrentLogic() { bussinessLogic = new LogicForChoosingUserStage { }; }
        public string Generate_question() { return bussinessLogic.Generate_question();}
        public string Generate_question_after_receiving_incorrect_answer() { return bussinessLogic.Generate_question_after_receiving_incorrect_answer(); }
        public void Execution_for_the_correct_answer(string answer) { bussinessLogic.Execution_for_the_correct_answer(answer, this); }
        internal ORM database = ORM.GetInstance();
        internal IBussinessLogic bussinessLogic;
        internal string currentUser = "";
        internal string currentAccountNumber = "";
    }

    internal interface IBussinessLogic
    {
        string Generate_question ();
        string Generate_question_after_receiving_incorrect_answer();
        void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic);
       
    }

    internal class LogicForChoosingUserStage : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Would you like to enter as a:\n1. User\n2. System admin";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Your input is incorrect. Please try again:\n1. User\n2. System admin";
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if(answer == "1")
            { 
                currentLogic.bussinessLogic = new LogicForAuthentificationStage {};
                
            }
            if (answer == "2")
            {
                currentLogic.bussinessLogic = new LogicForSystemAdminStage {};
            }
        }
    }

    internal class LogicForSystemAdminStage : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter password:\n";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            Environment.Exit(0);
            return "";
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit") currentLogic.bussinessLogic = new LogicForAuthentificationStage { };
        }
    }

    internal class LogicForAuthentificationStage : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Would you like to: \n1. sign up\n2. log in\n3. exit";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Please try again: \n1. sign up\n2. log in\n3. exit";   
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "1") 
            {
                currentLogic.bussinessLogic = new LogicForSignUpStage { };
            }
            if (answer == "2") 
            {
                currentLogic.bussinessLogic = new LogicForLogInStage { };
            }
            if (answer == "3") currentLogic.bussinessLogic = new LogicForChoosingUserStage { };
        }
    }

    internal class LogicForSignUpStage : IBussinessLogic 
    { 
        public string Generate_question()
        {
            return "Enter your Username then your password separated by comma. . If you want to return to previous list of choices then write \"exit\":";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Written Username is already used. Please try again.\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForAuthentificationStage { };
            }
            else 
            { 
                int positionOfComma = answer.IndexOf(','); 
                
                string username = answer.Substring(0, positionOfComma);
                string password = answer.Substring(positionOfComma + 1, answer.Length - positionOfComma - 1);
                if (username.Length * password.Length > 0)
                {
                    currentLogic.database.InsertNewAccount(username, password);
                }
            }
        }
    }

    internal class LogicForLogInStage : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter your username then password. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Username or password is incorrect. Please try again\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit") currentLogic.bussinessLogic = new LogicForAuthentificationStage() { };
            else { currentLogic.currentUser = answer.Substring(0, answer.IndexOf(',')); currentLogic.bussinessLogic = new LogicForWorkingWithAccountNumber { }; }
        }
    }

    internal class LogicForLoggedAccount : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter your account number then pincode separated by comma. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Username or pincode is incorrect. Please try again\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            string username = answer.Substring(0,answer.IndexOf(","));
            string password = answer.Substring(answer.IndexOf(",") + 1, answer.Length - 1 - answer.IndexOf(","));
            if (currentLogic.database.CheckIfThePasswordSuitUsername(username, password)) currentLogic.bussinessLogic = new LogicForLoggedAccount { };
        }
    }
    public enum listOfChoicesForWorkingWithAccountNumber
    {
        CreateAccountNumber = 1,
        CheckBalance,
        PayMoneyIn,
        MakeAWithrawal,
        CheckBalanceHistory
    }

    internal class LogicForWorkingWithAccountNumber : IBussinessLogic
    {
        public string Generate_question()
        {
            return "If you want to return to previous list of choices then write \"exit\"\nWould you like to:\n1. Create account number\n2. Check balance\n3.Pay money in\n4. Make a withdrawal\n5. Check balance history";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Your input is incorrect. Please try again\n" + Generate_question();
        }
        
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if(answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForLoggedAccount();
            }
            else
            {
                switch (Convert.ToInt16(answer))
                {
                    case (int)listOfChoicesForWorkingWithAccountNumber.CreateAccountNumber:
                        currentLogic.bussinessLogic = new LogicForCreatingAccountNumber { };
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.CheckBalance:
                        currentLogic.bussinessLogic = new LogicForCheckBalance { };
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.PayMoneyIn:
                        currentLogic.bussinessLogic = new LogicForPayMoneyIn { };
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.CheckBalanceHistory:
                        currentLogic.bussinessLogic = new LogicForCheckBalanceHistory { };
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.MakeAWithrawal:
                        currentLogic.bussinessLogic = new LogicForMakeAWithrawal { };
                        break;
                }
            }
        }


    }

    internal class LogicForMakeAWithdrawal : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter how much money you want to withdraw. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "You does not have such amount of money. Please try again.\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            string amountOfMoneyToWithdraw = answer;
            currentLogic.database.WithdrawMoneyFromAccountNumber(currentLogic.currentUser, currentLogic.currentAccountNumber, amountOfMoneyToWithdraw);
        }
    }

    internal class LogicForCheckBalance : IBussinessLogic
    {
        public string Generate_question()
        {
            return "If you want to return to previous list of choices then write \"exit\"\nYour balance:";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "You does not have such amount of money. Please try again.\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            string amountOfMoneyToWithdraw = answer;
            currentLogic.database.WithdrawMoneyFromAccountNumber(currentLogic.currentUser, currentLogic.currentAccountNumber, amountOfMoneyToWithdraw)
        }
    }

    internal class LogicForCreatingAccountNumber : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter your account number then pincode separated by comma. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Username or pincode have to contain at least one character each. Please try again\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            string accountNumber = answer.Substring(0, answer.IndexOf(","));
            string pincode = answer.Substring(answer.IndexOf(",") + 1, answer.Length - 1 - answer.IndexOf(","));
            currentLogic.database.InsertAccountNumberIntoTableOfAccountNUmbers(currentLogic.currentUser, accountNumber, pincode);
            currentLogic.bussinessLogic = new LogicForLoggedAccount { };
        }
    }
}

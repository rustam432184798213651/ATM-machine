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
        public string Generate_question() { return bussinessLogic.Generate_question(); }
        public string Generate_question_after_receiving_incorrect_answer() { return bussinessLogic.Generate_question_after_receiving_incorrect_answer(); }
        public void Execution_for_the_correct_answer(string answer) { bussinessLogic.Execution_for_the_correct_answer(answer, this); }
        internal ORM database = ORM.GetInstance();
        internal IBussinessLogic bussinessLogic;
        internal string currentUser = "";
        internal string currentAccountNumber = "";

    }

    internal interface IBussinessLogic
    {
        string Generate_question();
        string Generate_question_after_receiving_incorrect_answer();
        void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic);

    }


    enum enumForLogicForChossingUserStage
    {
        WantsToEnterAsUser = 1,
        WantsToEnterAsSystemAdmin 
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
            if(answer == enumForLogicForChossingUserStage.WantsToEnterAsUser.ToString())
            { 
                currentLogic.bussinessLogic = new LogicForAuthentificationStage {};
                
            }
            if (answer == enumForLogicForChossingUserStage.WantsToEnterAsSystemAdmin.ToString())
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
    enum enumForLogicForAuthentificationStage
    {
        WantsToSignUp = 1,
        WantsToLogIn,
        WantsToExit
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
            
            if (answer == enumForLogicForAuthentificationStage.WantsToSignUp.ToString()) 
                currentLogic.bussinessLogic = new LogicForSignUpStage { };
            if (answer == enumForLogicForAuthentificationStage.WantsToLogIn.ToString()) 
                currentLogic.bussinessLogic = new LogicForLogInStage { };
            if (answer == enumForLogicForAuthentificationStage.WantsToExit.ToString()) 
                currentLogic.bussinessLogic = new LogicForChoosingUserStage { };
        }
    }

    internal class LogicForSignUpStage : IBussinessLogic 
    { 
        public string Generate_question()
        {
            return "Enter your Username then your password separated by comma. Username and password cannot contain commas. If you want to return to previous list of choices then write \"exit\":";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Format of Username or password is incorrect or someone else is already using such username. Please try again with diffrent input\n" + Generate_question();
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
            return "Enter your Username then your password separated by comma. Username and password cannot contain commas. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Format of Username or password is incorrect or someone else is already using such username. Please try again with diffrent input\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit") currentLogic.bussinessLogic = new LogicForAuthentificationStage() { };
            else 
            { 
                currentLogic.currentUser = answer.Substring(0, answer.IndexOf(',')); currentLogic.bussinessLogic = new LogicForCreatingOrEnteringAccountNumber { }; 
            }
        }
    }

    internal class LogicForCreatingOrEnteringAccountNumber : IBussinessLogic
    {
        public string Generate_question()
        {
            return "If you want to return to previous list of choices then write \"exit\"\nChoose what would you like to do:\n1. Create account number\n2. Enter existing account number";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "You should choose one or two. Please try again with diffrent input\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit") currentLogic.bussinessLogic = new LogicForAuthentificationStage() { };
            else
            {
                if(answer == "1")
                { 
                  currentLogic.bussinessLogic = new LogicForCreatingAccountNumber { };
                }
                else if (answer == "2")
                {
                    currentLogic.bussinessLogic = new LogicForChoosingAccountNumber { };
                }
            }
        }
    }
    public enum listOfChoicesForWorkingWithAccountNumber
    {

        CheckBalance = 1,
        PayMoneyIn,
        MakeAWithrawal,
        CheckBalanceHistory
    }
    internal class LogicForLoggedAccount : IBussinessLogic
    {
        public string Generate_question()
        {
            return "If you want to return to previous list of choices then write \"exit\"\nWould you like to:\n1. Check balance\n2. Pay money in\n3. Make a withdrawal\n4. Check balance history";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Your input is incorrect. Please try again\n" + Generate_question();
        }

        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForChoosingAccountNumber();
            }
            else
            {
                switch (Convert.ToInt16(answer))
                {
                    
                    case (int)listOfChoicesForWorkingWithAccountNumber.CheckBalance:
                        CLI.GetInstance().getOutputFromBussinessLogic("Your balance is " + currentLogic.database.GetBalance(currentLogic.currentUser, currentLogic.currentAccountNumber).ToString());
                        CLI.GetInstance().showOutputFromBussinessLogic();
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.MakeAWithrawal:
                        currentLogic.bussinessLogic = new LogicForMakeAWithdrawal {};
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.PayMoneyIn:
                        currentLogic.bussinessLogic = new LogicForPutMoneyIn();
                        break;
                    case (int)listOfChoicesForWorkingWithAccountNumber.CheckBalanceHistory:
                        string history =  currentLogic.database.GetOperationsWithAccountNumber(currentLogic.currentUser, currentLogic.currentAccountNumber);
                        CLI.GetInstance().getOutputFromBussinessLogic(history);
                        CLI.GetInstance().showOutputFromBussinessLogic();
                        break;
                        /*              case (int)listOfChoicesForWorkingWithAccountNumber.PayMoneyIn:
                                          currentLogic.bussinessLogic = new LogicForPayMoneyIn { };
                                          break;
                                      case (int)listOfChoicesForWorkingWithAccountNumber.CheckBalanceHistory:
                                          currentLogic.bussinessLogic = new LogicForCheckBalanceHistory { };
                                          break;
                                      case (int)listOfChoicesForWorkingWithAccountNumber.MakeAWithrawal:
                                          currentLogic.bussinessLogic = new LogicForMakeAWithrawal { };
                                          break;*/
                }
            }
        }
    }
  

    internal class LogicForChoosingAccountNumber : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter your account number then pincode separated by comma. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "Account number or pincode is incorrect. Please try again\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if(answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForCreatingOrEnteringAccountNumber { };
            }
            else 
            { 
                string accountNumber = answer.Substring(0, answer.IndexOf(","));
                string pincode = answer.Substring(answer.IndexOf(",") + 1, answer.Length - 1 - answer.IndexOf(","));
                currentLogic.currentAccountNumber = accountNumber;
                currentLogic.bussinessLogic = new LogicForLoggedAccount { };
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
            if (answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForLoggedAccount { };
            }
            else 
            { 
            string amountOfMoneyToWithdraw = answer;
            currentLogic.database.WithdrawMoneyFromAccountNumber(currentLogic.currentUser, currentLogic.currentAccountNumber, amountOfMoneyToWithdraw);
            }
        }
    }
    internal class LogicForPutMoneyIn : IBussinessLogic
    {
        public string Generate_question()
        {
            return "Enter how much money you want to put into bank account. If you want to return to previous list of choices then write \"exit\"";
        }
        public string Generate_question_after_receiving_incorrect_answer()
        {
            return "You should put number and only number. Please try again.\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForLoggedAccount { };
            }
            else
            {
                string amountOfMoneyToPutIn = answer;
                currentLogic.database.PutMoneyIntoAccountNumber(currentLogic.currentUser, currentLogic.currentAccountNumber, amountOfMoneyToPutIn);
            }
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
            return "Account number and pincode have to be represented as numbers. Please try again\n" + Generate_question();
        }
        public void Execution_for_the_correct_answer(string answer, CurrentLogic currentLogic)
        {
            if (answer == "exit")
            {
                currentLogic.bussinessLogic = new LogicForCreatingOrEnteringAccountNumber { };
            }
            else 
            { 
                string accountNumber = answer.Substring(0, answer.IndexOf(","));
                string pincode = answer.Substring(answer.IndexOf(",") + 1, answer.Length - 1 - answer.IndexOf(","));
                currentLogic.currentAccountNumber = accountNumber;
                currentLogic.database.InsertAccountNumberIntoTableOfAccountNumbers(currentLogic.currentUser, accountNumber, pincode);
                currentLogic.bussinessLogic = new LogicForLoggedAccount { };
            }
        }
    }
}

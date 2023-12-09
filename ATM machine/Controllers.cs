using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;


namespace ATM_machine
{
    
    class ClassForValidationOfData
    {
        internal bool StringContainsOnlyDigits(string answer)
        {
            int out_number = 0;
            return int.TryParse(answer, out out_number);
        }
        internal bool CheckStringOnCorrectFormat(string answer)
        {
            int number_of_commas = answer.Length - answer.Replace(",", "").Length;
            if (number_of_commas > 1)
            {
                return false;
            }
            if (number_of_commas == 0) return false;
            int number_of_characters_before_comma = 0;
            int number_of_characters_after_comma = 0;
            for (int i = 0; i < answer.IndexOf(','); i++)
            {
                if (answer[i] != ' ')
                {
                    number_of_characters_before_comma++;
                }
            }
            for (int i = answer.IndexOf(',') + 1; i < answer.Length; i++)
            {
                if (answer[i] != ' ')
                {
                    number_of_characters_after_comma++;
                }
            }
            return number_of_characters_before_comma * number_of_characters_after_comma > 0;
        }
        internal ClassForValidationOfData() { controller = new ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect {}; }
        public bool Check(string answer) { return controller.Check(answer, this); }
        internal string currentUser = "";
        internal string currentAccountNumber = "";
        internal IController controller;
    }


    interface IController
    {
        bool Check(string answer, ClassForValidationOfData classForValidationOfData);
    }

    internal class ControllerForCheckingIfTheAnswerForAuthentificationStageIsCorrect : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (!classForValidationOfData.StringContainsOnlyDigits(answer)) return false;
            bool AnswerIsCorrect = (Convert.ToInt16(answer) >= 1 && Convert.ToInt16(answer) <= 3);
            if (AnswerIsCorrect)
            {
                switch (Convert.ToInt16(answer))
                {
                    case 1:
                        classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForSignUpStageIsCorrect { };
                        break;
                    case 2:
                        classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForLogInStageIsCorrect { };
                        break;
                    case 3:
                        classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect { };
                        break;
                }
            }
            return AnswerIsCorrect;
        }
    }

    //Problem with this one
    internal class ControllerForCheckingIfTheAnswerForLogInStageIsCorrect : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit" )
            {
                classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForAuthentificationStageIsCorrect { };
                return true;
            }
            
            if(classForValidationOfData.CheckStringOnCorrectFormat(answer))
            {
                string username = answer.Substring(0, answer.IndexOf(","));
                string password = answer.Substring(answer.IndexOf(",") + 1);
                if (ORM.GetInstance().CheckIfThePasswordSuitUsername(username, password))
                {
                
                    classForValidationOfData.currentUser = username;
                    classForValidationOfData.controller = new ControllerForCreatingOrEnteringAccountNumber { };
                    return true;
                }
            }


            return false;
        }

       
    }
    internal class ControllerForCreatingOrEnteringAccountNumber : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForAuthentificationStageIsCorrect { };
                return true;
            }
            
            if (classForValidationOfData.StringContainsOnlyDigits(answer))
            {
                int chosenNumber = Convert.ToInt16(answer);
                if (!(Convert.ToInt16(answer) < 3 && Convert.ToInt16(answer) > 0))
                {
                    return false;
                }
                switch (chosenNumber)
                {
                    case 1:
                        classForValidationOfData.controller = new ControllerForWorkingWithCreatingAccount { };
                        break;
                    case 2:
                        classForValidationOfData.controller = new ControllerForChoosingAccountNumber { };
                        break;
                }
                return true;
            }


            return false;
        }
    }
    internal class ControllerForChoosingAccountNumber : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForCreatingOrEnteringAccountNumber { };
                return true;
            }
            if (!classForValidationOfData.CheckStringOnCorrectFormat(answer)) return false; 
            string accountNumber = answer.Substring(0, answer.IndexOf(","));
          
            if (!ORM.GetInstance().CheckIfTheAccountNumberInTableOfAccountNumbers(classForValidationOfData.currentUser, accountNumber))
            {
                return false;
            }
            classForValidationOfData.currentAccountNumber = accountNumber;
            classForValidationOfData.controller = new ControllerForWorkingWithAccountNumber { };
            return true;

        }
    }

    internal class ControllerForWorkingWithAccountNumber : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForChoosingAccountNumber { };
                return true;
            }
             
            if (!classForValidationOfData.StringContainsOnlyDigits(answer)) return false;

            int chosenNumber = Convert.ToInt16(answer);
            if (chosenNumber > 0 && chosenNumber < 5)
            {
                switch (chosenNumber)
                {
                    case 2:
                        classForValidationOfData.controller = new ControllerForWorkingPayMoneyIn  { };
                        break;
                    case 3:
                        classForValidationOfData.controller = new ControllerForWorkingWithWithdrawalOfMoney { };
                        break;

                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
   
 

    internal class ControllerForWorkingWithBalance : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            { classForValidationOfData.controller = new ControllerForCreatingOrEnteringAccountNumber { }; return true; };
            if (answer == "Yes") return true;
            return false;
        }
    }
    internal class ControllerForWorkingPayMoneyIn : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            { classForValidationOfData.controller = new ControllerForWorkingWithAccountNumber { }; return true; };
            return classForValidationOfData.StringContainsOnlyDigits(answer);
        }
    }

    

    internal class ControllerForWorkingWithPayInMoney : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForWorkingWithAccountNumber { };
                return true;
            }
            if (classForValidationOfData.StringContainsOnlyDigits(answer)) return true;
            return false;
        }
    }

    internal class ControllerForWorkingWithCreatingAccount : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForCreatingOrEnteringAccountNumber { };
                return true;
            }
            if (classForValidationOfData.StringContainsOnlyDigits(answer.Substring(0, answer.IndexOf(","))) && classForValidationOfData.StringContainsOnlyDigits(answer.Substring(answer.IndexOf(",") + 1)))
            {
                classForValidationOfData.controller = new ControllerForWorkingWithAccountNumber { };
                return true;
            }
            return false;
        }
    }
   
    internal class ControllerForWorkingWithWithdrawalOfMoney : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForWorkingWithAccountNumber { };
                return true;
            }
            if (ORM.GetInstance().GetBalance(classForValidationOfData.currentUser, classForValidationOfData.currentAccountNumber) < Convert.ToInt16(answer)) return  false;
            return true;
        }
    }

    internal class ControllerForCheckingIfTheAnswerForSignUpStageIsCorrect : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            if (answer == "exit")
            {
                classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForAuthentificationStageIsCorrect { };
                return true;
            }
            int number_of_commas = answer.Length - answer.Replace(",", "").Length;
            if (number_of_commas != 1)
            {
                return false;
            }
            if (answer.Length > 0)
            {
                return true;
            }
            return false;
        }
    }

    internal class ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect : IController
    {
        internal ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect() { }
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
   

            if (!classForValidationOfData.StringContainsOnlyDigits(answer)) return false;

            bool AnswerDoesNotBreakAnyRules = (Convert.ToInt16(answer) < 3 && Convert.ToInt16(answer) > 0);
            switch (Convert.ToInt16(answer))
            {
                case 1:
                    classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForAuthentificationStageIsCorrect { };
                    break;
                case 2:
                    classForValidationOfData.controller = new ControllerForSystemAdminStage { };
                    break;

            }
            return AnswerDoesNotBreakAnyRules;
        }
    }
    internal class ControllerForSystemAdminStage : IController
    {
        public bool Check(string answer, ClassForValidationOfData classForValidationOfData)
        {
            
            return answer.Length > 0;
        }
    }




}

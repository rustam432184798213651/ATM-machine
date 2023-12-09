using System;
using System.Collections.Generic;
using System.Linq;
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
        internal ClassForValidationOfData() { controller = new ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect {}; }
        public bool Check(string answer) { return controller.Check(answer, this); }
        internal string currentUser = "";
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
            int number_of_commas = answer.Length - answer.Replace(",", "").Length;
            if (number_of_commas > 1)
            {
                return false;
            }
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



            string username = answer.Substring(0, answer.IndexOf(","));
            string password = answer.Substring(answer.IndexOf(",") + 1);
            if (number_of_characters_after_comma >= 1 && number_of_characters_before_comma >= 1 && !ORM.GetInstance().CheckIfThePasswordSuitUsername(username, password))
            {
                classForValidationOfData.currentUser = username;
                classForValidationOfData.controller = new ControllerForChoosingAccountNumber { };
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
                classForValidationOfData.controller = new ControllerForCheckingIfTheAnswerForLogInStageIsCorrect { };
                return true;
            }
            string accountNumber = answer.Substring(0, answer.IndexOf(","));
          
            if (ORM.GetInstance().CheckIfTheAccountNumberInTableOfAccountNumbers(classForValidationOfData.currentUser, accountNumber))
            {
                return false;
            }

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
            if (number_of_commas > 1)
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

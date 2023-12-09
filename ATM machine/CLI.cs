using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_machine
{
    /*
        CLI = Command-line interface
    */
   

    /*
        State Pattern 
        and 
        Template method pattern
    */
    public class CLI
    {
        public CLI() 
        {
            classForValidationOfData = new ClassForValidationOfData
            {
                controller = new ControllerForCheckingIfTheAnswerForChoosingUserStageIsCorrect()
            };
            currentBussinessLogic = new CurrentLogic
            {
                bussinessLogic = new LogicForChoosingUserStage { }
            };
        }

        private enum AnswerToDoYouWantTo
        {
            sign_in = 1,
            log_in,
            exit
        }
        public void Run()
        {
            string question = currentBussinessLogic.Generate_question();
            string answer;
            while (true)
            {
                Console.WriteLine(question);
                answer = Console.ReadLine();
                if (classForValidationOfData.Check(answer))
                {
                    currentBussinessLogic.Execution_for_the_correct_answer(answer);
                    question = currentBussinessLogic.Generate_question();
                }
                else
                {
                    question = currentBussinessLogic.Generate_question_after_receiving_incorrect_answer();
                }
            }

        }
        CurrentLogic currentBussinessLogic;
        ClassForValidationOfData classForValidationOfData = new ClassForValidationOfData { };


    }
}

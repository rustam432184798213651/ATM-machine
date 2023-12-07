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
    interface ICLI
    {
        
    }

    /*
        State Pattern 
        and 
        Template method pattern
    */
    internal class CLI
    {

        private enum AnswerToDoYouWantTo
        {
            sign_in = 1,
            log_in,
            exit
        }
        public void Authentification()
        {
          
            Console.WriteLine("Do you want to: ");
            Console.WriteLine("1. sign up");
            Console.WriteLine("2. log in");
            Console.WriteLine("3. exit");
            int answer = Convert.ToInt16(Console.ReadKey().KeyChar);
            

        }
    }
}

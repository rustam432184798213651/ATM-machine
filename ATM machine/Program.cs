using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ATM_machine
{
    
    internal class Program
    {
        
        static void Main(string[] args)
        {
            CLI.GetInstance().Run();
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestTask
{
    class OpenWallet
    {
        public static void Main(string[] args)
        {
            string input = "0";
            
            while (input != "q") 
            {
                Console.WriteLine("Введите одну из команд:" +
                    "\n 1. Создать кошелек. " +
                    "\n 2. Посмотреть все транзакции кошелька за месяц. " +
                    "\n 3. Самые большие траты за месяц. " +
                    "\n 4. Совершить новую транзакцию. " +
                    "\n q. Выйти из приложения.");
                input = Console.ReadLine();
                Menu.ReadCommands(input);
            }
        } 
    }
}

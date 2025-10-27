using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TestTask.Tests
{
    [TestClass()]
    public class WalletTests
    {
        private static Wallet wallet1;
        private static Wallet wallet2;
        
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext context) 
        {
            wallet1 = new Wallet("A","$",100);
            wallet2 = new Wallet("B","$",100);
        }

        [TestMethod()]
        public void AddTransactionsToWalletTest()
        {
            bool ExeptionCatched = false;
            Transaction newTransaction = new Transaction(wallet1, 10, "Expense");
            
            try
            {
                newTransaction = new Transaction(wallet1, 1000, "Expense");
                
            }
            catch (Exception ex)
            {
                ExeptionCatched = true;
            }
            newTransaction = new Transaction(wallet1, 10, "Income");
            
            newTransaction = new Transaction(wallet1, 100, "Expense");
            
            newTransaction = new Transaction(wallet1, 100, "Income");
            
            Assert.IsTrue( ExeptionCatched, "Должно вызываться иключение из-за недостатка средств" );
        }
        [TestMethod()]
        public void GetCurrentMonthExpensesTest()
        {
            List<Transaction> Expenses = wallet1.GetMonthExpenses(DateTime.Now.Month,DateTime.Now.Year);
            HashSet<decimal> expected = [10, 100];
            foreach (Transaction transaction in Expenses) 
            {
                if (!expected.Contains(transaction.value))
                {
                    Assert.Fail($"В список добавлена ненужная трата.");
                }
                else
                {

                    expected.Remove(transaction.value);
                }
            }
            if (expected.Count > 0)
            {
                Assert.Fail("Не все траты вошли в список");
            }
        }
        [TestMethod()]
        public void GetCurrentMonthIncomesTest()
        {
            HashSet<decimal> expected = [10, 100];

            List<Transaction> Incomes = wallet1.GetMonthIncomes(DateTime.Now.Month,DateTime.Now.Year);

            foreach (Transaction transaction in Incomes) 
            {
                if (!expected.Contains(transaction.value))
                {
                    Assert.Fail($"В список добавлена ненужное пополнение.");
                }
                else
                {
                    expected.Remove(transaction.value);
                }
            }
            if (expected.Count > 0)
            {
                Assert.Fail("Не все пополнения вошли в список");
            }
        }

        [TestMethod()]
        public void ShowMonthThreeBiggestExpensesTest()
        {
            var consoleOut = new StringWriter();
            string expectedOut = "";
            Console.SetOut(consoleOut);
            
            Transaction transaction = new Transaction(wallet1, 1000, "Income", DateTime.Now.AddDays(-1));
            transaction = new Transaction(wallet1, 200, "Expense", DateTime.Now.AddDays(-1));

            expectedOut += $"Три самые больщие траты в кошельке A за {DateTime.Now.Month}.{DateTime.Now.Year}\n" +
               "200\n" +
               "100\n" +
               "10\n"+
               $"Три самые больщие траты в кошельке B за {DateTime.Now.Month}.{DateTime.Now.Year}\n" +
               "Трата номер 1 не найдена \n" +
               "Трата номер 2 не найдена \n" +
               "Трата номер 3 не найдена \n";
            Menu.ShowMonthThreeBiggestExpenses(wallet1, DateTime.Now.Month, DateTime.Now.Year);
            Menu.ShowMonthThreeBiggestExpenses(wallet2, DateTime.Now.Month, DateTime.Now.Year);
            if (consoleOut.ToString() != expectedOut)
            {
                Assert.Fail($"Пришло это \n {consoleOut.ToString()}\n должно было это \n {expectedOut}");
            }

        }

        [TestMethod()]
        public void ShowMonthTransactionsTest()
        {
            var consoleOut = new StringWriter();
            string expectedOut = "";
            Console.SetOut(consoleOut);
            Transaction transaction = new Transaction(wallet1, 1000, "Income", DateTime.Now.AddMonths(-1));
            transaction = new Transaction(wallet1, 200, "Expense", DateTime.Now.AddMonths(-1));

            expectedOut += " Сумма пополнений в этом месяце +1110 | Сумма трат в этом месяце -310\n"+
                "Поступления | Tраты\n"+
                "1000 |200 \n"+
                "10 |10 \n"+
                "100 |100 \n";

            Menu.ShowMonthTransactions(wallet1, DateTime.Now.Month, DateTime.Now.Year);

            if (consoleOut.ToString() != expectedOut) {
                Assert.Fail($"Пришло это \n {consoleOut.ToString()}\n должно было это \n {expectedOut}");
            }

        }
       
    }
}
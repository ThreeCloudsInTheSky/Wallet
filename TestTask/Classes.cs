using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Net.Quic;
using System.Reflection;
using System.Xml.Linq;
namespace TestTask
{
    public class Wallet
    {
        private static int walletIdCounter = 0;
        private decimal _openingBalance;
        public List<Transaction> transactions {  get; private set; }
        public int id {  get; private set; }
        public decimal currentBalance { get; private set; }
        public string name { get; private set; }
        public string currency { get; private set; }

        public Wallet(string Name, string Currency, decimal Balance)
        {
            id = ++walletIdCounter;
            transactions = new List<Transaction>();
            _openingBalance = Balance;
            currentBalance = Balance;
            name = Name;
            currency = Currency;
        }
        internal bool AddNewTransaction(Transaction transaction)
        {
            if (transaction.type == "Expense")
            {
                if ( currentBalance < transaction.value)
                {
                    return false;
                }
                else
                {
                    transactions.Add(transaction);
                    currentBalance -= transaction.value;
                }
            }
            else
            {
                transactions.Add(transaction);
                currentBalance += transaction.value;
            }
            return true;
        }
        public List<Transaction> GetMonthExpenses(int month, int year)
        {
            List<Transaction> Expenses = new List<Transaction>();
            foreach (Transaction transaction in transactions)
            {
                if ((transaction.creationDate.Year == year) && (transaction.creationDate.Month == month))
                {
                    if (transaction.type == "Expense")
                    {
                        Expenses.Add(transaction);
                    }
                }
            }
            return Expenses;
        }
        public List<Transaction> GetMonthIncomes(int month, int year)
        {
            List<Transaction> Incomes = new List<Transaction>();
            foreach (Transaction transaction in transactions)
            {
                if ((transaction.creationDate.Year == year) && (transaction.creationDate.Month == month))
                {
                    if (transaction.type == "Income")
                    {
                        Incomes.Add(transaction);
                    }
                }
            }
            return Incomes;
        }
    }
    public class Transaction
    {
        private static int transactionIdCounter = 0;
        private int _id;
        private int _walletId;
        public decimal value { get; private set; }
        public string type { get; private set; }
        public DateTime creationDate { get; private set; }
        public string description { get; private set; }

        public Transaction(Wallet Wallet, decimal Value, string Type, string? Descryption = "")
        {
            _id = ++transactionIdCounter;
            _walletId = Wallet.id;
            value = Value;
            type = Type;
            creationDate = DateTime.UtcNow;
            description = Descryption;
            if (!Wallet.AddNewTransaction(this)) throw new WarningException("Транзакция не прошла, недостаточно средств.");
        }
        public Transaction(Wallet Wallet, decimal Value, string Type, DateTime CreationDate, string? Descryption = "")
        {
            _id = ++transactionIdCounter;
            _walletId = Wallet.id;
            value = Value;
            type = Type;
            creationDate = CreationDate;
            description = Descryption;
            if (!Wallet.AddNewTransaction(this)) throw new WarningException("Транзакция не прошла, недостаточно средств.");

        }
    }
    public static class Menu
    {
        private static List<Wallet> Wallets = new List<Wallet>();
        public static void ReadCommands(string InputCommand)
        {        
            switch (InputCommand)
            {
                case "1": //Создать кошелек
                    {
                        Console.Clear();
                        CreateWallet();
                        break;
                    }
                case "2": //Посмотреть все транзакции кошелька за определенный месяц.
                    {
                        Console.Clear();
                        int walletNumber = ChooseWalletNumber();
                        int year;
                        int month;
                        ChooseYearAndMonth( out year, out month);

                        ShowMonthTransactions(Wallets[walletNumber],month, year);
                        break;
                    }
                case "3": //Самые большие траты за месяц.
                    {
                        int year;
                        int month;
                        ChooseYearAndMonth(out year, out month);
                        Console.Clear();
                        foreach (Wallet wallet in Wallets) 
                        {
                            ShowMonthThreeBiggestExpenses(wallet, month, year);
                        }
                        break;
                    }
                case "4": //Совершить новую транзакцию.
                    {
                        int walletNumber = ChooseWalletNumber();
                        CreateTransaction(Wallets[walletNumber]);
                        break;
                    }
                case "5":
                    {
                        int walletNumber = ChooseWalletNumber();
                        Console.WriteLine(Wallets[walletNumber].currentBalance);
                        break;
                    }
                case "q": //Выйти
                    {
                        Console.Clear();
                        Console.WriteLine("Спасибо за использование программы");
                        break;
                    }
                default:
                    {
                        Console.Clear();
                        Console.WriteLine("Введена неверная команда");
                        break;
                    }

            }
        }
        public static void ChooseYearAndMonth(out int year, out int month)
        {
            
        YearCall:
            Console.WriteLine("Введите желаемый год");
            string yearInput = Console.ReadLine();
            if ((!int.TryParse(yearInput, out year)) && ((year > DateTime.Now.Year) || (year < 0)))
            {
                Console.Clear();
                Console.WriteLine("Год необходимо ввести в числовом формате, так же он не должен превышать нынешний.\n Пример -\"2002\"");
                goto YearCall;
            }
        MonthCall:
            Console.WriteLine("Введите желаемый месяц");
            string monthInput = Console.ReadLine();
            if ((!int.TryParse(monthInput, out month)) && ((month > 12) || (month < 1)))
            {
                Console.Clear();
                Console.WriteLine("Месяц необходимо ввести в числовом формате.\n Пример - \"9\" (Сентябрь)");
                goto MonthCall;
            }
        }
        public static int ChooseWalletNumber()
        {
            Console.Clear();

            WalletNumberCall:
            Console.WriteLine("Выберите кошелек:");
            ShowWallets();
            string walletNumberInput = Console.ReadLine();
            if ((int.TryParse(walletNumberInput, out int walletNumber)) && (walletNumber <= Wallets.Count))
            {
                Console.Clear();
                Console.WriteLine($"Выбран кошелек {walletNumber}. {Wallets[walletNumber - 1].name}");
                return walletNumber - 1;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Введен неправильный номер кошелька");

                goto WalletNumberCall;
            }
        }
        public static void ShowWallets()
        {
            int walletCounter = 0;
            foreach (Wallet Wallet in Wallets) 
            {
                walletCounter++;
                Console.WriteLine($"{walletCounter}. Кошелек {Wallet.name}");
            }
        }
        private static void CreateWallet()
        {
            NameCall:
            Console.WriteLine("Выберите имя кошелька:");
            string name = Console.ReadLine();
            if (name == null) goto NameCall;
            
            CurrencyCall:
            Console.WriteLine("Введите валюту хранимую в кошельке:");
            string currency = Console.ReadLine();
            if (currency == null) goto CurrencyCall;
            
            BalanceCall:
            Console.WriteLine("Введите текущий баланс кошелька:");
            string balanceInput = Console.ReadLine();
            if (Decimal.TryParse(balanceInput, out decimal balance))
            {
                Wallet NewWallet = new Wallet(name, currency, balance);
                Wallets.Add(NewWallet);
                Console.Clear();
                Console.WriteLine($"Создан новый кошелек {name}");

            }
            else
            {
                Console.WriteLine("Введен неправильный баланс.");
                goto BalanceCall;
            }
        }
        private static void CreateTransaction(Wallet wallet)
        {
            ValueCall:
            Console.WriteLine("Введите сумму транзакции:");
            string ValueInput = Console.ReadLine();
            if ((!Decimal.TryParse (ValueInput, out decimal value))||(value<=0))
            {
                Console.Clear();
                Console.WriteLine("Неккоректная сумма транзакции");
                goto ValueCall;
            }

            TypeCall:
            string type = "";
            Console.WriteLine("Выберите тип транзакции:\n 1. Снятие \n 2. Поступление");
            string TypeInput= Console.ReadLine();
            if ((!int.TryParse(TypeInput, out int typeNumber)) || (typeNumber < 1) || (typeNumber > 2))
            {
                Console.Clear();
                Console.WriteLine("Выбран неправильный тип транзакции");
                goto TypeCall;
            }
            if (typeNumber == 1) type = "Expense";
            else type = "Income";

            Console.WriteLine("Введите описание транзакции");
            string descrypion = Console.ReadLine();

            Transaction newTransaction;
            try
            {
                if (descrypion == null)
                {
                    newTransaction = new Transaction(wallet, value, type);
                }
                else
                {
                    newTransaction = new Transaction(wallet, value, type, descrypion);
                }
            }
            catch (WarningException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
        public static void ShowMonthTransactions(Wallet wallet, int month, int year)
        {
            List<Transaction> ExpenseTransactions;
            List<Transaction> IncomeTransactions;
            decimal ExpenseTransactionsSum = 0;
            decimal IncomeTransactionsSum = 0;
            ExpenseTransactions = wallet.GetMonthExpenses(month, year);
            IncomeTransactions = wallet.GetMonthIncomes(month, year);
            foreach (Transaction transaction in ExpenseTransactions)
            {
                ExpenseTransactionsSum += transaction.value;
            }
            foreach (Transaction transaction in IncomeTransactions)
            {
                IncomeTransactionsSum += transaction.value;
            }
            ExpenseTransactions.Sort((a, b)=>DateTime.Compare(a.creationDate,b.creationDate));
            IncomeTransactions.Sort((a, b) => DateTime.Compare(a.creationDate, b.creationDate));

            Console.Write($" Сумма пополнений в этом месяце +{IncomeTransactionsSum} | Сумма трат в этом месяце -{ExpenseTransactionsSum}\n");
            int MaximumTransactionsOfOneType;

            if (IncomeTransactions.Count > ExpenseTransactions.Count) MaximumTransactionsOfOneType = IncomeTransactions.Count;
            else MaximumTransactionsOfOneType = ExpenseTransactions.Count;

            if (ExpenseTransactionsSum <= IncomeTransactionsSum)
            {
                Console.Write("Поступления | Tраты\n");
                for (int i = 0; i < MaximumTransactionsOfOneType * 2; i++)
                {
                    if (i % 2 == 0)
                    {
                        if ((IncomeTransactions.Count - 1) >= (i / 2))
                        {
                            Console.Write($"{IncomeTransactions[i / 2].value} |");
                        }
                        else
                        {
                            Console.Write("__________ |");
                        }
                    }
                    else
                    {
                        if ((ExpenseTransactions.Count - 1) >= ((i - 1) / 2))
                        {
                            Console.Write($"{ExpenseTransactions[(i - 1) / 2].value} \n");

                        }
                        else
                        {
                            Console.Write("__________ \n");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Траты | Поступления");
                for (int i = 0; i < MaximumTransactionsOfOneType * 2; i++)
                {
                    if (i % 2 == 0)
                    {
                        if ((ExpenseTransactions.Count - 1) >= (i / 2))
                        {
                            Console.Write($"{ExpenseTransactions[i / 2].value} |");
                        }
                        else
                        {
                            Console.Write("__________ |");
                        }
                    }
                    else
                    {
                        if ((IncomeTransactions.Count - 1) >= ((i - 1) / 2))
                        {
                            Console.Write($"{IncomeTransactions[(i - 1) / 2].value} \n");
                        }
                        else
                        {
                            Console.Write("__________ \n");
                        }
                    }
                }
            }
        }
        public static void ShowMonthThreeBiggestExpenses(Wallet wallet, int month, int year)
        {
            List<Transaction> ExpenseTransactions;
            ExpenseTransactions = wallet.GetMonthExpenses(month, year);
            ExpenseTransactions.Sort((a,b)=> -decimal.Compare(a.value,b.value));
            Console.Write($"Три самые больщие траты в кошельке {wallet.name} за {month}.{year}\n");
            for (int i = 0; i < 3; i++)
            {
                if (ExpenseTransactions.Count >= i + 1)
                {
                    Console.Write(ExpenseTransactions[i].value + "\n");
                }
                else Console.Write($"Трата номер {i + 1} не найдена \n");
            }
        }
    }
}

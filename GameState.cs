using Mine;
using Miner.Miner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;



namespace Miner
{
    public class GameState
    {
        
      
        public WarehouseItem RawOre { get; set; }
        public List<Workers> Workers { get; set; }
        public List<Equipment> Equipments { get; set; }
       
        public double Balance { get; private set; } = 500000;
        public int MineLevel { get; set; }
        public double TotalIncome { get; private set; }
        public double TotalExpenses { get; private set; }
        public double LastDayExpenses { get; private set; }
        public double TotalProfit => TotalIncome - TotalExpenses;
        public int DayCounter { get; private set; }
        public bool TaxPaid { get; set; }          // Оплачен ли налог за текущий месяц
        public int CurrentMonthProfit { get; set; } // Прибыль за месяц (считается отдельно)




        public List<DayReport> Reports { get; private set; } = new List<DayReport>();


        public GameState()
        {
            Deposits = new List<MineDeposit>
            {
                new MineDeposit(900, 450000_000, 175_000_000), // 1 450 000 млн т (в тоннах)
                new MineDeposit(1250, 670000_000, 250_000_000), // 250 млн грн
                new MineDeposit(1315, 750000_000, 295_000_000),
                // можешь добавить дальше уровни...
            };

            Workers = new List<Workers>
            {
                new Workers("Горняк", 0.48, 800),
                new Workers("Администратор", 0, 700, 3.5)
            };

            Equipments = new List<Equipment>
            {
                new Equipment("Буровое оборудование", 7.5, 500, 350000),
                new Equipment("Транспортное оборудование", 8.5, 900, 450000),
                new Equipment("Скреперные лебедки", 5.3, 450, 175000)
            };

            RawOre = new WarehouseItem("Сырая руда", 0, 3400); // тут должна быть только цена 
        }



        public void AddIncome(double revenue)
        {
            TotalIncome += revenue;
            Balance += revenue;
        }

        public void CalculateDailyExpenses()
        {
            LastDayExpenses = Workers.Sum(w => w.Count * w.SalaryPerDay);
            TotalExpenses += LastDayExpenses;
            Balance -= LastDayExpenses;
        }


        public void CalculateMonthlyEquipmentExpenses()
        {
            LastDayExpenses = Equipments.Sum(eq => eq.MaintenanceCost * eq.Count);
            TotalExpenses += LastDayExpenses;
            Balance -= LastDayExpenses;
        }

        public void CalculateExpenses()
        {
            double salary = Workers.Sum(w => w.Count * w.SalaryPerDay);
            double maintenance = Equipments.Sum(eq => eq.Count * eq.MaintenanceCost);
            TotalExpenses = salary + maintenance;
            Balance -= TotalExpenses;
        }


        public bool CanAfford(double cost) => Balance >= cost;

        public void Spend(double cost)
        {
            if (CanAfford(cost))
                Balance -= cost;
        }

        // ... твои текущие поля

        // 👉 Тарифы
        public double WaterTariff { get; set; } = 25.0;       // грн за куб (1000 литров)
        public double ElectricityTariff { get; set; } = 3.5;  // грн за кВт·ч

        // 👉 Потребление
        public double WaterConsumptionLiters { get; private set; } = 0;     // литры
        public double ElectricityConsumptionKwh { get; private set; } = 0;  // кВт·ч
        // Глубины

        public List<MineDeposit> Deposits { get; private set; }
        public int CurrentDepositIndex { get; private set; } = 0;
        public MineDeposit CurrentDeposit => Deposits[CurrentDepositIndex];


        // 👉 Нормы на 1 тонну руды
        public double WaterPerTon { get; set; } = 100;   // литров
        public double EnergyPerTon { get; set; } = 100;  // кВт·ч
       

        public event Action DayChanged;


        // 👉 Добыча руды
        public void MineOre(double tons)
        {

            var deposit = CurrentDeposit;
            if (deposit.RemainingOre <= 0)
            {
                MessageBox.Show("Залежи на текущей глубине исчерпаны. Улучшите шахту для перехода глубже.");
                return;

            }
            double mined = Math.Min(tons, deposit.RemainingOre);
            deposit.RemainingOre -= mined;
            RawOre.Quantity += mined;
            WaterConsumptionLiters += mined * WaterPerTon;
            ElectricityConsumptionKwh += mined * EnergyPerTon;


        }
        public void UpgradeMine()
        {
            if (CurrentDepositIndex + 1 >= Deposits.Count)
            {
                MessageBox.Show("Дальнейшие глубины пока недоступны.");
                return;
            }

            var nextDeposit = Deposits[CurrentDepositIndex + 1];

            if (Balance < nextDeposit.UpgradeCost)
            {
                MessageBox.Show("Недостаточно средств для перехода на следующую глубину.");
                return;
            }

            Balance -= nextDeposit.UpgradeCost;
            CurrentDepositIndex++;
        }


        // 👉 Оплата тарифов
        public void PayUtilities()
        {
            // переводим литры в кубы (1000 литров = 1 куб)
            double waterCost = (WaterConsumptionLiters / 1000.0) * WaterTariff;
            double electricityCost = ElectricityConsumptionKwh * ElectricityTariff;
            double total = waterCost + electricityCost;

            if (CanAfford(total))
            {
                Spend(total);
                WaterConsumptionLiters = 0;
                ElectricityConsumptionKwh = 0;
            }
        }
        public void AddMonthlyReport(DateTime date)
        {
            // считаем налог (25% от прибыли месяца)
            double tax = Math.Round(CurrentMonthProfit * 0.25);

            Reports.Add(new DayReport
            {
                Date = date,
                Production = RawOre.Quantity,       // сколько добыто за месяц
                Sold = 0,                           // можно добавить продажи, если фиксируешь отдельно
                Income = TotalIncome,               // доход за месяц
                Expenses = TotalExpenses,           // расходы за месяц
                Profit = TotalIncome - TotalExpenses, // прибыль до налога
                Balance = Balance,                  // текущий баланс
                MonthProfit = CurrentMonthProfit,   // накопленная прибыль месяца
                TaxAmount = tax,                    // налог 25%
                TaxPaid = TaxPaid                   // статус оплаты
            });

            // 👉 после формирования отчёта обнуляем доходы/расходы месяца
            TotalIncome = 0;
            TotalExpenses = 0;
        }


        // 👉 Переход на следующий день
        public void NextDay()
        {
            DayCounter++;

            if (DayCounter % 30 == 0)
            {
                AddMonthlyReport(DateTime.Now);
                TotalIncome = 0;
                TotalExpenses = 0;
            }

            DayChanged?.Invoke(); // 👉 уведомляем все контролы
        }

        public void AddDayReport(DayReport report)
        {
            Reports.Add(report);
            DayCounter++;

            // Прибыль за день
            int dailyProfit = (int)(report.Income - report.Expenses);
            CurrentMonthProfit += dailyProfit;

            // Каждые 30 дней проверяем налог
            if (DayCounter % 30 == 0)
            {
                TaxPaid = false; // налог нужно оплатить
            }
        }
        // Оплата налога
        public bool PayTax()
        {
            int tax = (int)Math.Round(CurrentMonthProfit * 0.25); // 25% от прибыли          
            if (Balance >= tax)
            {
                Balance -= tax;
                TaxPaid = true;
                CurrentMonthProfit = 0; // начинаем новый месяц
                return true;
            }
            return false;
        }
    

        public void AddReport(DateTime date, double production, double sold, double income)
        {
            Reports.Add(new DayReport
            {
                Date = date,
                Production = production,
                Sold = sold,
                Income = income,
                Expenses = LastDayExpenses,
                Profit = income - LastDayExpenses,
                Balance = Balance
            });
        }
    }
}

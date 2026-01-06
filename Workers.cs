using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Miner
{
    public class Workers
    {
        public string Name { get; set; }              // должность
        public int Count { get; set; }                // количество сотрудников
        public double ProductionPerDay { get; set; }  // производство в день (тонн руды)
        public double SalaryPerDay { get; set; }      // зарплата в день
        public double BonusPercent { get; set; }      // бонус к производству (для администраторов)

        // Конструктор для обычных работников
        public Workers(string name, double productionPerDay, double salaryPerDay)
        {
            Name = name;
            ProductionPerDay = productionPerDay;
            SalaryPerDay = salaryPerDay;
            BonusPercent = 0;
            Count = 0;
        }

        // Конструктор для работников с бонусом (например, администратор)
        public Workers(string name, double productionPerDay, double salaryPerDay, double bonusPercent)
        {
            Name = name;
            ProductionPerDay = productionPerDay;
            SalaryPerDay = salaryPerDay;
            BonusPercent = bonusPercent;
            Count = 0;
        }
    }
}



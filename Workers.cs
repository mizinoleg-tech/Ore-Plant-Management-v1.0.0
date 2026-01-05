using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Miner
{
    namespace Miner
    {
        public class Workers
        {
            public string Name { get; set; }              // Должность
            public double ProductionPerDay { get; set; }  // Производство (тонн/день)
            public double SalaryPerDay { get; set; }      // Зарплата (грн/день)
            public double BonusPercent { get; set; }      // % бонуса к производству (для админа)
            public int Count { get; set; }                // Количество сотрудников

            public Workers(string name, double production, double salary, double bonusPercent = 0)
            {
                Name = name;
                ProductionPerDay = production;
                SalaryPerDay = salary;
                BonusPercent = bonusPercent;
                Count = 0;
            }
        }
    }
}

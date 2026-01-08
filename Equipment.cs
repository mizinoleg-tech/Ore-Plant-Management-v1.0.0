using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

  namespace Miner
{
    public class Equipment
{
    public string Name { get; set; }              // Название оборудования
    public double BonusPercent { get; set; }      // % к производительности рабочих
    public double MaintenanceCost { get; set; }   // Стоимость обслуживания (грн/месяц)
    public double Price { get; set; }             // Цена покупки (если нужно)
    public int Count { get; set; }                // Количество единиц

    public Equipment(string name, double bonusPercent, double maintenanceCost, double price = 0)
    {
        Name = name;
        BonusPercent = bonusPercent;
        MaintenanceCost = maintenanceCost;
        Price = price;
        Count = 0;
    }
        public override string ToString()
        {
            return $"{Name} | Кол-во: {Count} | Цена: {Price.ToString("#,##0")} грн | Обслуживание: {MaintenanceCost:#,##0} грн/мес";
        }

    }
}


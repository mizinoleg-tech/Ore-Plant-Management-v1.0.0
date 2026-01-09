using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miner
{
    public class Warehouse
    {
        public List<WarehouseItem> Items { get; set; } = new List<WarehouseItem>();
        public int Capacity { get; set; } = 5000; // начальная вместимость

        // Текущая загруженность склада
        public double CurrentLoad => Items.Sum(i => i.Quantity);

        // Свободное место
        public double FreeSpace => Capacity - CurrentLoad;

        // Добавление ресурса с проверкой вместимости
        public bool AddItem(WarehouseItem item)
        {
            if (CurrentLoad + item.Quantity <= Capacity)
            {
                Items.Add(item);
                return true;
            }
            return false; // склад переполнен
        }

        // Добавление руды напрямую
        public bool AddOre(double tons, double pricePerTon)
        {
            if (CurrentLoad + tons <= Capacity)
            {
                var ore = Items.FirstOrDefault(i => i.Name == "Сырая руда");
                if (ore == null)
                {
                    ore = new WarehouseItem("Сырая руда", 0, pricePerTon);
                    Items.Add(ore);
                }
                ore.Add(tons);
                ore.PricePerTon = pricePerTon;
                return true;
            }
            return false; // нет места
        }

        public void RemoveItem(string name, double quantity)
        {
            var item = Items.FirstOrDefault(i => i.Name == name);
            if (item != null && item.Quantity >= quantity)
            {
                item.Quantity -= quantity;
                if (item.Quantity == 0)
                    Items.Remove(item);
            }
        }
    }
}



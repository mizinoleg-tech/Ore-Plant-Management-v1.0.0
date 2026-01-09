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
            double free = FreeSpace;
            if (free <= 0) return false;

            // если предмет больше, чем свободное место — кладём частично
            double toAdd = Math.Min(item.Quantity, free);

            var existing = Items.FirstOrDefault(i => i.Name == item.Name);
            if (existing == null)
            {
                existing = new WarehouseItem(item.Name, 0, item.PricePerTon);
                Items.Add(existing);
            }
            existing.Add(toAdd);
            return true;
        }

        public bool AddOre(double tons, double pricePerTon)
        {
            double free = FreeSpace;
            if (free <= 0) return false;

            double toAdd = Math.Min(tons, free);

            var ore = Items.FirstOrDefault(i => i.Name == "Сырая руда");
            if (ore == null)
            {
                ore = new WarehouseItem("Сырая руда", 0, pricePerTon);
                Items.Add(ore);
            }
            ore.Add(toAdd);
            ore.PricePerTon = pricePerTon;
            return true;
        }

        public void RemoveItem(string name, double quantity)
        {
            var item = Items.FirstOrDefault(i => i.Name == name);
            if (item != null)
            {
                // используем SetQuantity вместо прямого присвоения
                item.SetQuantity(item.Quantity - quantity);

                if (item.Quantity == 0)
                    Items.Remove(item);
            }
        }


    }
}



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public class UiUpdater
    {
        private readonly Label _lblBalance;
        private readonly Label _lblEmployees;
        private readonly Label _lblMineLevel;
        private readonly ListBox _lstEquipment;
        private readonly WarehouseControl _warehouseControl;
        private readonly TaxControl _taxControl;

        public UiUpdater(Label lblBalance, Label lblEmployees, Label lblMineLevel,
                         ListBox lstEquipment, WarehouseControl warehouseControl, TaxControl taxControl)
        {
            _lblBalance = lblBalance;
            _lblEmployees = lblEmployees;
            _lblMineLevel = lblMineLevel;
            _lstEquipment = lstEquipment;
            _warehouseControl = warehouseControl;
            _taxControl = taxControl;
        }

        public void Refresh(GameState gameState)
        {
            if (_lblBalance != null)
                _lblBalance.Text = $"Баланс: {gameState.Balance:N0} грн";

            if (_lblEmployees != null)
            {
                if (gameState.Workers != null && gameState.Workers.Any())
                {
                    // ищем работников по имени
                    int miners = gameState.Workers.FirstOrDefault(w => w.Name == "Горняк")?.Count ?? 0;
                    int admins = gameState.Workers.FirstOrDefault(w => w.Name == "Администратор")?.Count ?? 0;

                    _lblEmployees.Text = $"Горняки: {miners}, Администраторы: {admins}";
                }
                else
                {
                    _lblEmployees.Text = "Сотрудники: нет данных";
                }
            }

            if (_lblMineLevel != null)
                _lblMineLevel.Text = $"Уровень шахты: {gameState.MineLevel}";

            if (_lstEquipment != null)
            {
                _lstEquipment.Items.Clear();
                if (gameState.Equipments != null)
                {
                    foreach (var eq in gameState.Equipments)
                        _lstEquipment.Items.Add($"{eq.Name} x{eq.Count}");
                }
            }

            _warehouseControl?.UpdateData(
            _warehouseControl?.GetItem());
            _taxControl?.UpdateData();
        }



    }
}

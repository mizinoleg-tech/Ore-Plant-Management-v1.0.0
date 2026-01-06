using Mine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Miner
{
    public class EquipmentControl : UserControl
    {
        private DataGridView dgv;
        private Label lblSummary;
        private List<Equipment> equipments;

        public EquipmentControl(List<Equipment> sharedEquipments)
        {
            equipments = sharedEquipments ?? new List<Equipment>();
            InitUI();
            LoadData();
        }

        // 🔒 Метод для обновления данных после загрузки сохранения
        public void UpdateData(List<Equipment> newEquipments)
        {
            equipments = newEquipments ?? new List<Equipment>();
            LoadData();
        }

        private void InitUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(40, 40, 45);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            dgv.Columns.Add("Name", "Оборудование");
            dgv.Columns.Add("Bonus", "Бонус %");
            dgv.Columns.Add("Cost", "Затраты/день");
            dgv.Columns.Add("Count", "Количество");

            lblSummary = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            this.Controls.Add(dgv);
            this.Controls.Add(lblSummary);
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var eq in equipments)
            {
                dgv.Rows.Add(eq.Name, $"{eq.BonusPercent}%", $"{eq.CostPerDay} грн", eq.Count);
            }

            lblSummary.Text = $"Всего оборудования: {equipments.Count}";
        }
    }
}

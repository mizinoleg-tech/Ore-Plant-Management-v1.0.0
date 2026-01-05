using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace Miner
{
    public class ReportsControl : UserControl
    {
        private DataGridView dgv;
        private Label lblSummary;
        private GameState gameState;

        public ReportsControl(GameState state)
        {
            gameState = state;
            this.Dock = DockStyle.Fill;
            InitUI();
            LoadData();
        }

        private void InitUI()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            // === Таблица отчётов ===
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.Fixed3D
            };

            dgv.RowTemplate.Height = 35;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);

            dgv.Columns.Add("Date", "📅 Дата");
            dgv.Columns.Add("Production", "⚒️ Добыча (т)");
            dgv.Columns.Add("Sold", "📦 Продано (т)");
            dgv.Columns.Add("Income", "💰 Доходы (грн)");
            dgv.Columns.Add("Expenses", "💸 Расходы (грн)");
            dgv.Columns.Add("Profit", "📊 Прибыль (грн)");
            dgv.Columns.Add("Balance", "🏦 Баланс (грн)");

            // === Сводка ===
            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 55)
            };

            layout.Controls.Add(dgv, 0, 0);
            layout.Controls.Add(lblSummary, 0, 1);

            this.Controls.Add(layout);
        }

        private void LoadData()
        {
            dgv.Rows.Clear();
            foreach (var report in gameState.Reports)
            {
                int rowIndex = dgv.Rows.Add(
                    report.Date.ToString("dd.MM.yyyy"),
                    report.Production.ToString("F2"),
                    report.Sold.ToString("F2"),
                    report.Income.ToString("F2"),
                    report.Expenses.ToString("F2"),
                    report.Profit.ToString("F2"),
                    report.Balance.ToString("F2")
                );

                // 👉 цветовая подсветка прибыли/убытков
                var row = dgv.Rows[rowIndex];
                double profit = report.Profit;
                if (profit > 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(60, 100, 60); // зелёный
                else if (profit < 0)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(120, 60, 60); // красный
            }

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            double totalIncome = gameState.Reports.Sum(r => r.Income);
            double totalExpenses = gameState.Reports.Sum(r => r.Expenses);
            double totalProfit = gameState.Reports.Sum(r => r.Profit);

            lblSummary.Text = $"Итого: 💰 Доход {totalIncome:F2} грн | 💸 Расходы {totalExpenses:F2} грн | 📊 Прибыль {totalProfit:F2} грн";
        }
    }
}

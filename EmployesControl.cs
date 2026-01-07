using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Miner
{
    public class EmployeesControl : UserControl
    {
        private DataGridView dgv;
        private Label lblSummary;
        private Button btnHire;
        private Button btnFire;
       
        private List<Workers> workers;

        public EmployeesControl(List<Workers> sharedWorkers)
        {
            workers = sharedWorkers ?? new List<Workers>();
            InitUI();
            LoadData();

        }
        public void OnDayChanged()
        {
            UpdateData();
        }

        public void UpdateData()
        {
            LoadData();
        }
        // 🔒 Метод для обновления данных после загрузки сохранения
        public void UpdateData(List<Workers> newWorkers)
        {
            workers = newWorkers ?? new List<Workers>();
            LoadData();
        }
        private void LoadData()
        {
            dgv.Rows.Clear();

            foreach (var w in workers)
            {
                dgv.Rows.Add(
                    w.Name,
                    w.Name == "Администратор"
                        ? $"+{w.BonusPercent}% к производству"
                        : $"{w.ProductionPerDay} т руды",
                    $"{w.SalaryPerDay} грн",
                    w.Count
                );
            }

            UpdateSummary();
        }



        private void InitUI()
        {
            this.BackColor = Color.FromArgb(40, 40, 45);
            this.Padding = new Padding(10);

            // === Основной макет ===
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1
            };

            // Верхняя часть (таблица) - 50%
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            // Средняя часть (панель кнопок) - фиксированная высота
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            // Нижняя часть (сводка) - 20%
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

            // === Таблица сотрудников ===
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(45, 45, 48)
            };

            dgv.Columns.Add("Name", "Должность");
            dgv.Columns.Add("Production", "Производство/день");
            dgv.Columns.Add("Salary", "Зарплата/день");
            dgv.Columns.Add("Count", "Количество");

            // Стили таблицы
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            // === Панель кнопок ===
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(50, 50, 55)
            };

            btnHire = new Button
            {
                Text = "✅ Нанять",
                Width = 120,
                Height = 40,
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnHire.FlatAppearance.BorderSize = 0;
            btnHire.Click += BtnHire_Click;

            btnFire = new Button
            {
                Text = "❌ Уволить",
                Width = 120,
                Height = 40,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnFire.FlatAppearance.BorderSize = 0;
            btnFire.Click += BtnFire_Click;

            var btnBuildBattery = new Button
            {
                Text = "⚡ Построить офис (450,000 грн)",
                Width = 300,
                Height = 40,
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnBuildBattery.FlatAppearance.BorderSize = 0;
            btnBuildBattery.Click += BtnBuildBattery_Click;

            // Добавляем кнопки в панель
            buttonPanel.Controls.Add(btnHire);
            buttonPanel.Controls.Add(btnFire);
            buttonPanel.Controls.Add(btnBuildBattery);

            // === Сводка ===
            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(50, 50, 55)
            };

            // === Добавляем всё в макет ===
            layout.Controls.Add(dgv, 0, 0);
            layout.Controls.Add(buttonPanel, 0, 1);
            layout.Controls.Add(lblSummary, 0, 2);

            this.Controls.Add(layout);
        }


        private void BtnHire_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                int index = dgv.SelectedRows[0].Index;
                double cost = workers[index].SalaryPerDay;

                // Проверка лимита
                if (workers[index].Name == "Горняк" && workers[index].Count >= ((Mine1)FindForm()).gameState.MaxMiners)
                {
                    MessageBox.Show("Достигнут лимит горняков для текущего уровня!");
                    return;
                }
                if (workers[index].Name == "Администратор" && workers[index].Count >= ((Mine1)FindForm()).gameState.MaxAdmins)
                {
                    MessageBox.Show("Достигнут лимит администраторов для текущего уровня!");
                    return;
                }

                // Проверка баланса
                if (((Mine1)FindForm()).gameState.CanAfford(cost))
                {
                    workers[index].Count++;
                    dgv.Rows[index].Cells["Count"].Value = workers[index].Count;
                    UpdateSummary();
                    ((Mine1)FindForm()).UpdateBalanceLabel();
                }
                else
                {
                    MessageBox.Show("Недостаточно средств для найма!");
                }
            }
        }

        private void BtnFire_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                int index = dgv.SelectedRows[0].Index;
                if (workers[index].Count > 0)
                {
                    workers[index].Count--;
                    dgv.Rows[index].Cells["Count"].Value = workers[index].Count;
                    UpdateSummary();
                }
            }
        }
        private void BtnBuildBattery_Click(object sender, EventArgs e)
        {
            var game = ((Mine1)FindForm()).gameState;
            if (game.BuildBattery())
            {
                MessageBox.Show("Аккумуляторная построена! Лимит штата увеличен.",
                    "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ((Mine1)FindForm()).UpdateBalanceLabel();
                UpdateSummary();
            }
            else
            {
                MessageBox.Show("Недостаточно средств для постройки!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary()
        {
            int miners = workers[0].Count;
            int admins = workers[1].Count;

            double baseProduction = miners * workers[0].ProductionPerDay;
            double productionWithAdmins = baseProduction * (1 + admins * (workers[1].BonusPercent / 100));

            double salary = miners * workers[0].SalaryPerDay + admins * workers[1].SalaryPerDay;

            lblSummary.Text = $"Итого: {miners} горняков, {admins} администраторов | " +
                              $"Производство: {productionWithAdmins:F2} т/день | " +
                              $"Затраты: {salary} грн/день";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // EmployeesControl
            // 
            this.Name = "EmployeesControl";
            this.Size = new System.Drawing.Size(599, 375);
            this.Load += new System.EventHandler(this.EmployeesControl_Load);
            this.ResumeLayout(false);

        }

        private void EmployeesControl_Load(object sender, EventArgs e)
        {

        }
    }
}


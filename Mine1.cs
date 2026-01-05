using Mine;
using Miner;
using Miner.Miner;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;


namespace Miner
{
    public partial class Mine1 : Form
    {
        private EmployeesControl employeesControl;
        private EquipmentControl equipmentControl;
        private WarehouseControl warehouseControl;
        private ConsumptionControl consumptionControl;

        public GameState gameState = new GameState();
        private DateTime gameDate = DateTime.Now;

        public Mine1()
        {
            InitializeComponent();
            InitUI();
            gameState = new GameState();

            var consumptionControl = new ConsumptionControl(gameState);
            consumptionControl.Dock = DockStyle.Fill;

        }



        private void InitUI()
        {
            TreeNode rootManagement = new TreeNode("Управление Комбинатом");
            rootManagement.Nodes.Add("Сотрудники");
            rootManagement.Nodes.Add("Оборудование");
            rootManagement.Nodes.Add("Склад");
            rootManagement.Nodes.Add("Потребление");

            TreeNode rootFinance = new TreeNode("Финансовые операции");
            rootFinance.Nodes.Add("Доходы");
            rootFinance.Nodes.Add("Расходы");
            rootFinance.Nodes.Add("Прибыль");
            rootFinance.Nodes.Add("Налоги");
            rootFinance.Nodes.Add("Отчет");

            treeView1.Nodes.Add(rootManagement);
            treeView1.Nodes.Add(rootFinance);
            treeView1.ExpandAll();
        }

        public void UpdateBalanceLabel()
        {
            lblBalance.Text = $"Баланс: {gameState.Balance.ToString("N2")} грн";
        }

        private void LoadMine1(object sender, EventArgs e)
        {
            lblDate.Text = gameDate.ToString("dd.MM.yyyy");
            UpdateBalanceLabel();
        }

        private void BtnNextDay_Click1(object sender, EventArgs e)
        {
            // 👉 если налоговый день и налог не оплачен — блокируем
            if (gameState.DayCounter % 30 == 0 && !gameState.TaxPaid)
            {
                MessageBox.Show(
                    "Необходимо оплатить налог — 25% от прибыли за месяц.\n" +
                    "Шахта будет закрыта до оплаты!",
                    "Налоговое уведомление",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return; // не даём перейти на следующий день
            }

            // 👉 если налог оплачен или не налоговый день — продолжаем
            gameState.NextDay();
            gameDate = gameDate.AddDays(1);
            lblDate.Text = gameDate.ToString("dd.MM.yyyy");


            // 👉 считаем добычу руды
            double baseProduction = gameState.Workers.Sum(w => w.Count * w.ProductionPerDay);
            double equipmentBonus = gameState.Equipments.Sum(eq => eq.BonusPercent * eq.Count);
            double finalProduction = baseProduction * (1 + equipmentBonus / 100);

            // 👉 добыча руды + начисление воды/энергии
            gameState.MineOre(finalProduction);

            // 👉 расходы по зарплатам и оборудованию
            gameState.CalculateDailyExpenses();
            if (gameState.DayCounter % 30 == 0)
                gameState.CalculateMonthlyEquipmentExpenses();

            // 👉 отчёт (без списания ресурсов!)
            gameState.AddReport(gameDate, finalProduction, 0, 0);

            if (gameState.DayCounter % 30 == 0)
            {
                MessageBox.Show(
                    "Наступил конец месяца.\n" +
                    "Оплатите налог — 25% от прибыли, чтобы продолжить игру.",
                    "Налоговое уведомление",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                gameState.TaxPaid = false; // сбрасываем флаг оплаты
            }


            // 👉 обновляем баланс и панель потребления
            UpdateBalanceLabel();
            if (consumptionControl != null)
                consumptionControl.UpdateData();
            // 👉 если открыт склад — обновляем его
            warehouseControl?.UpdateData();
        }


        private void BtnSellOre_Click(object sender, EventArgs e)
        {
            double tonsToSell = gameState.RawOre.Quantity;

            if (tonsToSell <= 0)
            {
                MessageBox.Show("На складе нет руды для продажи.");
                return;
            }

            // считаем доход
            double revenue = tonsToSell * gameState.RawOre.PricePerTon;

            // добавляем деньги на баланс
            gameState.AddIncome(revenue);

            // добавляем запись в отчёт
            gameState.AddReport(gameDate, 0, tonsToSell, revenue);

            // очищаем склад (обнуляем количество руды)
            gameState.RawOre.Quantity = 0;

            // обновляем баланс на форме
            UpdateBalanceLabel();

            // обновляем склад-контрол
            warehouseControl?.UpdateData();
        }


        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // очищаем панель один раз
            panel1.Controls.Clear();
            UpdateBalanceLabel();

            switch (e.Node.Text)
            {
                case "Управление Комбинатом":
                    var managementControl = new MineManagementControl(gameState);
                    managementControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(managementControl);
                    break;

                case "Сотрудники":
                    employeesControl = new EmployeesControl(gameState.Workers);
                    employeesControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(employeesControl);
                    break;

                case "Оборудование":
                    equipmentControl = new EquipmentControl(gameState.Equipments);
                    equipmentControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(equipmentControl);
                    break;

                case "Склад":
                    warehouseControl = new WarehouseControl(gameState);
                    warehouseControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(warehouseControl);
                    break;

                case "Потребление":
                    consumptionControl = new ConsumptionControl(gameState);
                    consumptionControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(consumptionControl);
                    break;

                case "Доходы":
                    var incomeControl = new IncomeControl(gameState);
                    incomeControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(incomeControl);
                    break;

                case "Расходы":
                    var expensesControl = new ExpensesControl(gameState);
                    expensesControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(expensesControl);
                    break;

                case "Налоги":
                    var taxControl = new TaxControl(gameState);
                    taxControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(taxControl);
                    break;
               



                case "Прибыль":
                    var profitControl = new ProfitControl(gameState);
                    profitControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(profitControl);
                    break;

                case "Отчет":
                    var reportsControl = new ReportsControl(gameState);
                    reportsControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(reportsControl);
                    break;

                default:
                    Label lbl = new Label
                    {
                        Text = $"Вы выбрали: {e.Node.Text}",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    panel1.Controls.Add(lbl);
                    break;
            }
        }


        private void btnPayTax_Click(object sender, EventArgs e)
        {
            if (gameState.PayTax())
            {
                MessageBox.Show("Налог оплачен. Можно продолжать игру.",
                    "Оплата налога", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Недостаточно средств для оплаты налога!",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    

        private void DataTime_Click(object sender, EventArgs e)
        {
           
            lblDate.Text = DateTime.Now.ToString(" dd.MM.yyyy");

        }

        private void lblDate_Click(object sender, EventArgs e)
        {


        }
        private void BtnPayUtilities_Click(object sender, EventArgs e)
        {
            gameState.PayUtilities();

            // обновляем баланс в верхней панели
            UpdateBalanceLabel();

            // обновляем панель потребления
            if (consumptionControl != null)
                consumptionControl.UpdateData();
        }



        private void lblBalance_Click(object sender, EventArgs e)
        {

        }

        private void lblDate_Click_1(object sender, EventArgs e)
        {

        }

        private void lblBalance_Click_1(object sender, EventArgs e)
        {

        }

        private void btnSaveProgress_Click(object sender, EventArgs e)
        {
            // Пример данных для сохранения
            var gameState = new
            {
                Balance = 0, // сюда подставишь текущий баланс
                Employees = 5, // количество сотрудников
                Equipment = new List<string> { "Экскаватор", "Дробилка" }, // список оборудования
                MineLevel = 1 // уровень шахты
            };

            // Сохраняем в JSON
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(gameState, Newtonsoft.Json.Formatting.Indented);
            System.IO.File.WriteAllText("savegame.json", json);

            MessageBox.Show("Прогресс игры сохранён!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            btnSaveProgress.Text = "Сохранить прогресс";
            btnSaveProgress.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

        }

        private void lblDate_Click_2(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
// ===== Директивы для модулей =====
public class MineManagementControl : UserControl
{
    private Label lblTitle;
    private Label lblDepth;
    private Label lblOre;
    private Label lblUpgradeCost;
    private Button btnUpgradeMine;
    private GameState gameState;

    public MineManagementControl(GameState state)
    {
        gameState = state;
        InitUI();
        UpdateMineInfo();
    }

    private void InitUI()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.FromArgb(30, 30, 40); // тёмный фон

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Padding = new Padding(20),
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Заголовок
        lblTitle = new Label
        {
            Text = "⛏️ Управление Комбинатом",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(45, 45, 55)
        };

        lblDepth = CreateInfoLabel();
        lblOre = CreateInfoLabel();
        lblUpgradeCost = CreateInfoLabel();

        btnUpgradeMine = new Button
        {
            Text = "Углубить шахту",
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            BackColor = Color.DarkGoldenrod,
            ForeColor = Color.White,
            Height = 50,
            FlatStyle = FlatStyle.Flat
        };
        btnUpgradeMine.FlatAppearance.BorderSize = 0;
        btnUpgradeMine.MouseEnter += (s, e) => btnUpgradeMine.BackColor = Color.Gold;
        btnUpgradeMine.MouseLeave += (s, e) => btnUpgradeMine.BackColor = Color.DarkGoldenrod;
        btnUpgradeMine.Click += BtnUpgradeMine_Click;

        layout.Controls.Add(lblTitle, 0, 0);
        layout.Controls.Add(lblDepth, 0, 1);
        layout.Controls.Add(lblOre, 0, 2);
        layout.Controls.Add(lblUpgradeCost, 0, 3);
        layout.Controls.Add(btnUpgradeMine, 0, 4);

        this.Controls.Add(layout);
    }

    private Label CreateInfoLabel()
    {
        return new Label
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12, FontStyle.Regular),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(50, 50, 60)
        };
    }

    private void BtnUpgradeMine_Click(object sender, EventArgs e)
    {
        gameState.UpgradeMine();
        UpdateMineInfo();

        if (this.ParentForm is Mine1 mainForm)
            mainForm.UpdateBalanceLabel();
    }

    private void UpdateMineInfo()
    {
        var d = gameState.CurrentDeposit;
        lblDepth.Text = $"Глубина: {d.Depth} м";
        lblOre.Text = $"Запасы: {d.RemainingOre:N0} тонн из {d.OreReserve:N0}";
        lblUpgradeCost.Text = $"Стоимость перехода: {d.UpgradeCost:N0} грн";
    }



private void OnDayChanged() => UpdateMineInfo();

    protected override void Dispose(bool disposing)
    {
        if (disposing && gameState != null)
            gameState.DayChanged -= OnDayChanged;
        base.Dispose(disposing);
    }
}

public class EmployeesControl : UserControl
{
    private DataGridView dgv;
    private Label lblSummary;
    private Button btnHire;
    private Button btnFire;

    private List<Workers> workers; // используем твой отдельный класс Workers

    public EmployeesControl(List<Workers> sharedWorkers)
    {
        workers = sharedWorkers;
        InitUI();
        LoadData();
    }

    private void LoadData()
    {
        foreach (var w in workers)
        {
            dgv.Rows.Add(w.Name,
                         w.Name == "Администратор" ? $"+{w.BonusPercent}% к производству" : $"{w.ProductionPerDay} т руды",
                         $"{w.SalaryPerDay} грн",
                         w.Count);
        }
        UpdateSummary();
    }

    // 👉 Свойства для формы Mine1
    public int MinersCount => workers[0].Count;
    public int AdminsCount => workers[1].Count;

    private void InitUI()
    {
        this.BackColor = Color.FromArgb(40, 40, 45);
        this.Padding = new Padding(10);

        var layout = new TableLayoutPanel();
        layout.Dock = DockStyle.Fill;
        layout.RowCount = 3;
        layout.ColumnCount = 1;
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

        // === Таблица сотрудников ===
        dgv = new DataGridView();
        dgv.Dock = DockStyle.Fill;
        dgv.AllowUserToAddRows = false;
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        dgv.Columns.Add("Name", "Должность");
        dgv.Columns.Add("Production", "Производство/день");
        dgv.Columns.Add("Salary", "Зарплата/день");
        dgv.Columns.Add("Count", "Количество");

        // Стили таблицы
        dgv.BackgroundColor = Color.FromArgb(45, 45, 48);
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
            Text = "Нанять",
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
            Text = "Уволить",
            Width = 120,
            Height = 40,
            BackColor = Color.DarkRed,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnFire.FlatAppearance.BorderSize = 0;
        btnFire.Click += BtnFire_Click;

        buttonPanel.Controls.Add(btnHire);
        buttonPanel.Controls.Add(btnFire);

        // === Сводка ===
        lblSummary = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 55)
        };

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

            // Проверка баланса
            if (((Mine1)FindForm()).gameState.CanAfford(cost))
            {
                ((Mine1)FindForm()).UpdateBalanceLabel();
                workers[index].Count++;
                dgv.Rows[index].Cells["Count"].Value = workers[index].Count;
                UpdateSummary();
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
}



public class EquipmentControl : UserControl
{
    private DataGridView dgv;
    private Label lblSummary;
    private Button btnAdd;
    private Button btnRemove;

    private List<Equipment> equipments; // общий список из GameState

    public EquipmentControl(List<Equipment> sharedEquipments)
    {
        equipments = sharedEquipments;
        InitUI();
        LoadData();
    }

    private void InitUI()
    {
        this.BackColor = Color.FromArgb(40, 40, 45);
        this.Padding = new Padding(10);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

        // === Таблица оборудования ===
        dgv = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        dgv.Columns.Add("Name", "Оборудование");
        dgv.Columns.Add("Bonus", "Бонус к производительности");
        dgv.Columns.Add("Maintenance", "Обслуживание/день");
        dgv.Columns.Add("Price", "Цена покупки");
        dgv.Columns.Add("Count", "Количество");

        // Стили таблицы
        dgv.BackgroundColor = Color.FromArgb(45, 45, 48);
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

        btnAdd = new Button
        {
            Text = "Купить",
            Width = 120,
            Height = 40,
            BackColor = Color.ForestGreen,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.Click += BtnAdd_Click;

        btnRemove = new Button
        {
            Text = "Продать",
            Width = 120,
            Height = 40,
            BackColor = Color.DarkRed,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold)
        };
        btnRemove.FlatAppearance.BorderSize = 0;
        btnRemove.Click += BtnRemove_Click;

        buttonPanel.Controls.Add(btnAdd);
        buttonPanel.Controls.Add(btnRemove);

        // === Сводка ===
        lblSummary = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(50, 50, 55)
        };

        layout.Controls.Add(dgv, 0, 0);
        layout.Controls.Add(buttonPanel, 0, 1);
        layout.Controls.Add(lblSummary, 0, 2);

        this.Controls.Add(layout);
    }

    private void LoadData()
    {
        dgv.Rows.Clear();
        foreach (var eq in equipments)
        {
            dgv.Rows.Add(eq.Name,
                         $"{eq.BonusPercent} %",
                         $"{eq.MaintenanceCost} грн",
                         $"{eq.Price} грн",   // цена покупки
                         eq.Count);
        }
        UpdateSummary();
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count > 0)
        {
            int index = dgv.SelectedRows[0].Index;
            double price = equipments[index].Price;

            if (((Mine1)FindForm()).gameState.CanAfford(price))
            {
                ((Mine1)FindForm()).gameState.Spend(price);
                ((Mine1)FindForm()).UpdateBalanceLabel();

                equipments[index].Count++;
                dgv.Rows[index].Cells["Count"].Value = equipments[index].Count;
                UpdateSummary();
            }
            else
            {
                MessageBox.Show("Недостаточно средств для покупки оборудования!");
            }
        }
    }

    private void BtnRemove_Click(object sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count > 0)
        {
            int index = dgv.SelectedRows[0].Index;
            if (equipments[index].Count > 0)
            {
                // уменьшаем количество
                equipments[index].Count--;
                dgv.Rows[index].Cells["Count"].Value = equipments[index].Count;

                // возвращаем часть стоимости (например, 70%)
                double refund = equipments[index].Price * 0.7;
                ((Mine1)FindForm()).gameState.AddIncome(refund); // добавляем деньги на баланс
                ((Mine1)FindForm()).UpdateBalanceLabel();

                UpdateSummary();
            }
        }
    }


    private void UpdateSummary()
    {
        double totalBonus = equipments.Sum(eq => eq.BonusPercent * eq.Count);
        double totalMaintenance = equipments.Sum(eq => eq.MaintenanceCost * eq.Count);
        double totalValue = equipments.Sum(eq => eq.Price * eq.Count);

        lblSummary.Text = $"Итого: бонус {totalBonus:F2}% | " +
                          $"Обслуживание {totalMaintenance} грн/мес | " +
                          $"Стоимость оборудования {totalValue} грн";
    }
}


    public class WarehouseControl : UserControl
    {
        private DataGridView dgv;
        private Label lblSummary;
        private WarehouseItem item;   // один объект склада
        private GameState gameState;  // ссылка на состояние игры

        // Конструктор: только GameState
        public WarehouseControl(GameState state)
        {
            gameState = state ?? throw new ArgumentNullException(nameof(state));
            item = new WarehouseItem("Сырая руда", GetOreQuantityFromState(state), 0.0);
            InitUI();
            LoadData();
            gameState.DayChanged += OnDayChanged;
        }

        // Конструктор: GameState + WarehouseItem
        public WarehouseControl(GameState state, WarehouseItem rawOre)
        {
            gameState = state ?? throw new ArgumentNullException(nameof(state));
            item = rawOre ?? new WarehouseItem("Сырая руда", GetOreQuantityFromState(state), 0.0);
            InitUI();
            LoadData();
            gameState.DayChanged += OnDayChanged;
        }

        // Перегрузка с обратным порядком аргументов
        public WarehouseControl(WarehouseItem rawOre, GameState state)
            : this(state, rawOre) { }

        private double GetOreQuantityFromState(GameState state)
        {
            return state.RawOre.Count; // если нужно — замени на формулу перевода в тонны
        }

        private void InitUI()
        {
            this.BackColor = Color.FromArgb(40, 40, 45);
            this.Padding = new Padding(10);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 85));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 15));

            // === Таблица склада ===
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.FromArgb(45, 45, 48)
            };

            dgv.Columns.Add("Name", "📦 Ресурс");
            dgv.Columns.Add("Quantity", "⚖️ Количество");
            dgv.Columns.Add("Price", "💰 Цена за тонну");
            dgv.Columns.Add("Total", "💵 Общая стоимость");

            dgv.DefaultCellStyle.BackColor = Color.FromArgb(60, 60, 65);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkSlateBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 35);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            // === Сводка ===
            lblSummary = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
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
            dgv.Rows.Add(
                item.Name,
                $"{item.Quantity:F2}",        // количество
                $"{item.PricePerTon:F2} грн", // цена
                $"{item.TotalValue():F2} грн" // общая стоимость
            );

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            lblSummary.Text = $"Всего: {item.Quantity:F2} тонн | Общая стоимость: {item.TotalValue():F2} грн";
        }

    // 👉 метод обновления склада
    public void UpdateData()
    {
        // если RawOre — один объект
        item.Quantity = gameState.RawOre.Quantity;

        // если RawOre — список, то суммируем
        // double totalOre = gameState.RawOre.Sum(r => r.Quantity);
        // item.Quantity = totalOre;

        LoadData();
    }


    // 👉 обработчик события смены дня
    private void OnDayChanged()
        {
            UpdateData();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && gameState != null)
            {
                gameState.DayChanged -= OnDayChanged;
            }
            base.Dispose(disposing);
        }
    }




public class IncomeControl : UserControl
{
    private Label lbl;

    public IncomeControl(GameState state)
    {
        lbl = new Label();
        lbl.Dock = DockStyle.Fill;
        lbl.TextAlign = ContentAlignment.MiddleCenter;
        lbl.Text = $"Доходы: {state.TotalIncome:F2} грн";
        this.Controls.Add(lbl);
    }
}


public class ExpensesControl : UserControl
{
    private Label lbl;

    public ExpensesControl(GameState state)
    {
        lbl = new Label();
        lbl.Dock = DockStyle.Fill;
        lbl.TextAlign = ContentAlignment.MiddleCenter;
        lbl.Text = $"Расходы: {state.TotalExpenses:F2} грн";
        this.Controls.Add(lbl);
    }
}


public class ProfitControl : UserControl
{
    private Label lbl;

    public ProfitControl(GameState state)
    {
        lbl = new Label();
        lbl.Dock = DockStyle.Fill;
        lbl.TextAlign = ContentAlignment.MiddleCenter;
        lbl.Text = $"Прибыль: {state.TotalProfit:F2} грн";
        this.Controls.Add(lbl);
    }
}


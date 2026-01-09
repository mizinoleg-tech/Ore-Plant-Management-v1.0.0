
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Forms;
using Miner;


namespace Miner
{
    public partial class Mine1 : Form
    {
        private EmployeesControl employeesControl;
        private EquipmentControl equipmentControl;
        private WarehouseControl warehouseControl;
        private ConsumptionControl consumptionControl;
        private TaxControl taxControl;

        public GameState gameState = new GameState();
        private DateTime gameDate = DateTime.Now;

        public Mine1()
        {
            InitializeComponent();

            // 1. Создаём состояние игры
            gameState = new GameState();
            saveManager = new SaveManager();

            // 2. Инициализируем контролы
            taxControl = new TaxControl(gameState);
            taxControl.Location = new Point(500, 50);
            this.Controls.Add(taxControl);

            consumptionControl = new ConsumptionControl(gameState);
            consumptionControl.Dock = DockStyle.Fill;
            this.Controls.Add(consumptionControl);

            // 3. Создаём UiUpdater
            uiUpdater = new UiUpdater(lblBalance, lblEmployees, lblMineLevel,
                                      lstEquipment, warehouseControl, taxControl);

            // 4. Инициализация интерфейса
            InitUI();
        }

        private void LoadGame()
        {
            var saveManager = new SaveManager();
            var loadedState = saveManager.Load();

            if (loadedState == null)
            {
                MessageBox.Show("Сохранение не найдено.", "Загрузка игры",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Используем загруженное состояние
            gameState = loadedState;

            // Обновляем дату
            gameDate = DateTime.Now;
            lblDate.Text = gameDate.ToString("dd.MM.yyyy");

            // Обновляем баланс
            UpdateBalanceLabel();

            // Обновляем контролы (без пересоздания!)
            employeesControl.UpdateData(gameState.Workers);
            equipmentControl.UpdateData(gameState.Equipments);
            warehouseControl.UpdateData(warehouseControl.GetItem());
            consumptionControl.UpdateData();
            taxControl?.UpdateData();

            // Обновляем интерфейс через UiUpdater
            uiUpdater.Refresh(gameState);

            MessageBox.Show("Игра успешно загружена!", "Загрузка игры",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }




        private ListBox lstEquipment;
        private void InitUI()
        {


            // Метка сотрудников
            lblEmployees = new Label
            {
                Location = new Point(20, 50),
                AutoSize = true,
                Text = $"Сотрудники: {gameState.Workers}"
            };
            this.Controls.Add(lblEmployees);

            // Метка уровня шахты
            lblMineLevel = new Label
            {
                Location = new Point(20, 80),
                AutoSize = true,
                Text = $"Уровень шахты: {gameState.MineLevel}"
            };
            this.Controls.Add(lblMineLevel);

            // Список оборудования
            lstEquipment = new ListBox
            {
                Location = new Point(20, 120),
                Size = new Size(200, 200)
            };
            this.Controls.Add(lstEquipment);

            // Дерево управления
            treeView1.Nodes.Clear(); // очищаем, чтобы не дублировалось

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

        private void StartNewGame()
        {
            // создаём новое состояние игры
            gameState = new GameState();

            // сбрасываем дату
            gameDate = DateTime.Now;
            lblDate.Text = gameDate.ToString("dd.MM.yyyy");

            // обновляем баланс
            UpdateBalanceLabel();

            // пересоздаём контролы с новым gameState
            employeesControl = new EmployeesControl(gameState.Workers);
            equipmentControl = new EquipmentControl(gameState.Equipments);
            warehouseControl = new WarehouseControl(gameState);   // ✅ передаём весь GameState
            consumptionControl = new ConsumptionControl(gameState);
            taxControl = new TaxControl(gameState);

            // подписываем события заново
            gameState.DayChanged += employeesControl.OnDayChanged;
            gameState.DayChanged += equipmentControl.OnDayChanged;
            gameState.DayChanged += warehouseControl.OnDayChanged;
            gameState.DayChanged += consumptionControl.OnDayChanged;
            gameState.DayChanged += taxControl.OnDayChanged;

            // безопасный вызов taxControl
            taxControl?.UpdateData();

            // безопасный вызов UiUpdater
            if (lblBalance != null && lblEmployees != null && lblMineLevel != null && lstEquipment != null)
            {
                uiUpdater = new UiUpdater(lblBalance, lblEmployees, lblMineLevel,
                                          lstEquipment, warehouseControl, taxControl);
                uiUpdater.Refresh(gameState);
            }

            MessageBox.Show("Начата новая игра!", "Новая игра",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // День нумеруется с 1. Если у тебя с 0 — адаптируй проверки ниже.
            int day = gameState.DayCounter;
            // 1) Блокировка в начале нового месяца, если за прошлый налог не оплачен
            // Блокируем только с 31-го дня и далее (т.е. когда пытаемся уйти за границу месяца)
            bool isStartOfNewMonth = (day >= 30) && (day % 30 == 0);
            if (isStartOfNewMonth && !gameState.TaxPaid)
            {
                MessageBox.Show(
                    "Необходимо оплатить налог — 25% от прибыли за прошлый месяц.\n" +
                    "Шахта будет закрыта до оплаты!",
                    "Налоговое уведомление",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
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
            warehouseControl?.UpdateData(            // 👉 если открыт склад — обновляем его
            warehouseControl?.GetItem());
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

            // фиксируем прибыль месяца (важно для налогов)
            gameState.CurrentMonthProfit += (int)revenue;

            // добавляем запись в отчёт
            gameState.AddReport(gameDate, 0, tonsToSell, revenue);

            // очищаем склад (через SetQuantity)
            gameState.RawOre.SetQuantity(0);

            // обновляем баланс на форме
            UpdateBalanceLabel();

            // обновляем склад-контрол
            warehouseControl?.UpdateData(warehouseControl?.GetItem());

            // обновляем налоговый контрол
            taxControl?.UpdateTaxInfo();
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
                    if (warehouseControl == null)
                        warehouseControl = new WarehouseControl(gameState);

                    warehouseControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(warehouseControl);
                    warehouseControl.UpdateData(warehouseControl.GetItem()); // обновляем данные при открытии
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
                    taxControl.Dock = DockStyle.Fill;
                    panel1.Controls.Add(taxControl);
                    taxControl.UpdateData(); // обновляем данные при открытии
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

        private SaveManager saveManager = new SaveManager();
        private UiUpdater uiUpdater;
        private Label lblEmployees;
        private Label lblMineLevel;

        private void btnSaveProgress_Click(object sender, EventArgs e)
        {
            saveManager.Save(gameState);
            MessageBox.Show("Прогресс игры сохранён!", "Сохранение",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnLoadProgress_Click(object sender, EventArgs e)
        {
            var loadedState = saveManager.Load();
            if (loadedState != null)
            {
                gameState = loadedState;
                gameDate = DateTime.Now;
                lblDate.Text = gameDate.ToString("dd.MM.yyyy");
                UpdateBalanceLabel();

                employeesControl?.UpdateData(gameState.Workers);
                equipmentControl?.UpdateData(gameState.Equipments);
                warehouseControl?.UpdateData(gameState.RawOre);
                consumptionControl?.UpdateData();
                taxControl?.UpdateData();

                uiUpdater?.Refresh(gameState);

                MessageBox.Show("Игра успешно загружена!", "Загрузка",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Сохранение не найдено.", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }




        private void btnNewGame_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void lblBalance_Click_2(object sender, EventArgs e)
        {
            lblBalance.Text = $"Баланс: {gameState.Balance:N2} грн";

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



        public void OnDayChanged() => UpdateMineInfo();

        protected override void Dispose(bool disposing)
        {
            if (disposing && gameState != null)
                gameState.DayChanged -= OnDayChanged;
            base.Dispose(disposing);
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
            equipments = sharedEquipments ?? new List<Equipment>();
            InitUI();
            LoadData();
        }
        // обработчик события DayChanged
        public void OnDayChanged()
        {
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
               $"{eq.MaintenanceCost:#,##0} грн",
               $"{eq.Price:#,##0} грн",   // цена покупки
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
}




using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace Miner
{
    public partial class Mine : Form
    {
        public Mine()
        {
            InitializeComponent();
        }
        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Mine1 mine1 = new Mine1();

            mine1.FormClosed += (s, args) => this.Show();

            this.Hide();
            mine1.Show();
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

           


        }

        private void label1_Click(object sender, EventArgs e)
        {
            

        }
        ToolTip toolTip11 = new ToolTip();
        private void Mine_Load(object sender, EventArgs e)
        {
            richTextBox1.ReadOnly = true;
            richTextBox1.BackColor = Color.Black;
            richTextBox1.ForeColor = Color.Gold;
            richTextBox1.Font = new Font("Segoe UI", 12, FontStyle.Regular);

            richTextBox1.Text =
            "После разрухи и государственных волнений судьба страны оказалась в ваших руках.\n" +
            "Вы — бывший сотрудник железорудного комбината ПАТ «Заря», которому предстоит возродить добычу железной руды и заложить основу для восстановления инфраструктуры государства.\n\n" +
            "Ваш баланс — 0 грн. Всё начинается с пустоты.\n" +
            "Чтобы добиться успеха, вам придётся нанимать рабочих, постепенно закупать оборудование, расширять и углублять свой рудник. Каждое решение — шаг к процветанию или к краху.\n\n" +
            "Ресурсы не бесконечны, но их хватит на ваш век.\n" +
            "Сможете ли вы превратить руины в процветающий комбинат и вернуть стране силу?";

            // Выделение ключевых слов
            HighlightText("0 грн", FontStyle.Bold, Color.DarkRed);
            HighlightText("руины", FontStyle.Bold, Color.Gray);
            HighlightText("процветающий комбинат", FontStyle.Bold, Color.Green);

            toolTip11.SetToolTip(pictureBox1, "Нажмите чтобы начать !"); }
            private void LoadProgress()
        {
            if (System.IO.File.Exists("savegame.json"))
            {
                string json = System.IO.File.ReadAllText("savegame.json");
                var gameState = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

                // Пример восстановления
                int balance = gameState.Balance;
                int employees = gameState.Employees;
                int mineLevel = gameState.MineLevel;
                // и т.д.
            }
        }

        

        // Метод для выделения текста
        private void HighlightText(string word, FontStyle style, Color color)
        {
            int index = richTextBox1.Text.IndexOf(word);
            if (index >= 0)
            {
                richTextBox1.Select(index, word.Length);
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, style);
                richTextBox1.SelectionColor = color;




                toolTip11.SetToolTip(pictureBox1, "Нажмите чтобы начать !");


            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}


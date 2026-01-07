using System.Windows.Forms;
using System.Drawing;

namespace Miner
{
    /// <summary>
    /// ProfitControl — это пользовательский элемент управления (UserControl),
    /// который отображает финансовую информацию: доход, расходы и прибыль.
    /// Используется в интерфейсе игры для визуализации финансового состояния.
    /// </summary>
    public class ProfitControl : UserControl
    {
        /// <summary>
        /// Метка (Label), в которой выводится текст с доходами, расходами и прибылью.
        /// </summary>
        private Label lblProfit;

        /// <summary>
        /// Конструктор по умолчанию.
        /// Настраивает внешний вид контрола и создаёт метку для отображения прибыли.
        /// </summary>
        public ProfitControl()
        {
            // Цвет фона контрола (тёмный стиль)
            this.BackColor = Color.FromArgb(40, 40, 45);

            // Контрол занимает всё доступное пространство
            this.Dock = DockStyle.Fill;

            // Создаём метку для отображения текста
            lblProfit = new Label
            {
                Text = "Прибыль пока не рассчитана", // начальный текст
                Dock = DockStyle.Fill,              // растягиваем метку на весь контрол
                ForeColor = Color.White,            // белый текст
                TextAlign = ContentAlignment.MiddleCenter, // выравнивание по центру
                Font = new Font("Segoe UI", 12F, FontStyle.Bold) // шрифт
            };

            // Добавляем метку в контрол
            this.Controls.Add(lblProfit);
        }

        /// <summary>
        /// Метод для обновления информации о прибыли.
        /// Принимает доход и расходы, рассчитывает прибыль и выводит результат в метку.
        /// </summary>
        /// <param name="income">Доход (грн)</param>
        /// <param name="expenses">Расходы (грн)</param>
        public void UpdateProfit(double income, double expenses)
        {
            // Вычисляем прибыль
            double profit = income - expenses;

            // Формируем строку для отображения
            lblProfit.Text = $"Доход: {income:F2} грн | Расходы: {expenses:F2} грн | Прибыль: {profit:F2} грн";
        }
    }
}

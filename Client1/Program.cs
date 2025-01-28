using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client1 { 
    public  class Program
    {
       

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new StartForm()); // Убедитесь, что конструктор без параметров доступен
            }
            catch (Exception ex)
            {
                // Обработка исключений (например, логирование)
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Запускаем мониторинг бездействия
            ActivityMonitor.Start(OnInactivityLock);

            Application.Run(new FormAutorization());
        }

        /// <summary>
        /// Вызывается при бездействии пользователя — скрываем все формы, открываем авторизацию.
        /// </summary>
        private static void OnInactivityLock()
        {
            // Останавливаем таймер на время блокировки
            ActivityMonitor.Stop();

            // Скрываем все открытые формы кроме FormAutorization
            foreach (Form form in Application.OpenForms)
            {
                if (!(form is FormAutorization))
                {
                    form.Invoke(new Action(() => form.Hide()));
                }
            }

            // Показываем форму авторизации
            Application.OpenForms[0]?.Invoke(new Action(() =>
            {
                var loginForm = new FormAutorization();
                loginForm.ShowDialog();

                // После входа снова запускаем мониторинг
                ActivityMonitor.Start(OnInactivityLock);
            }));
        }
    }
}
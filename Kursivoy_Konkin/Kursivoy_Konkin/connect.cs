using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursivoy_Konkin
{
    // Класс для хранения строки подключения к базе данных MySQL
    internal class connect
    {
        // Статическая строка соединения, собирается из настроек проекта
        public static string con = $"host={Properties.Settings.Default.host};" +
                                   $"uid={Properties.Settings.Default.uid};" +
                                   $"pwd={Properties.Settings.Default.pwd};" +
                                   $"database={Properties.Settings.Default.database}";
        // Эта строка подключается к базе по параметрам, заданным в настройках проекта

        public static string conNoDb = "server=localhost;user=root;password=root;port=3306;";
    }
}
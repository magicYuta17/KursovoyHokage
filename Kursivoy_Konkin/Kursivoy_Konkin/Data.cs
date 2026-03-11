using MySql.Data.MySqlClient; // Для работы с MySQL базой
using System; // Общие системные функции
using System.Collections.Generic; // Коллекции, например, Dictionary
using System.Data; // Для DataTable
using System.Linq; // LINQ-запросы
using System.Security.Cryptography; // Для хеширования паролей
using System.Text; // Для работы со строками и кодировками
using System.Threading.Tasks; // Асинхронные задачи (здесь не используется явно)

namespace Kursivoy_Konkin
{
    // Статический класс для работы с данными и базой
    public static class Data
    {
        // Переменные, хранящие информацию о текущем пользователе
        public static string role;
        public static int userId;
        public static string name;
        public static string surname;
        public static string patronymic;
        static public Dictionary<string, int> MyBucket; // Не используется явно, вероятно, для хранения данных

        // Получает ID из базы по переданному SQL-запросу
        public static int GetID(string sql)
        {
            // Создаём новое подключение к базе данных
            MySqlConnection connection = new MySqlConnection(connect.con);
            connection.Open(); // Открываем соединение

            // Создаём команду SQL
            MySqlCommand command = new MySqlCommand(sql, connection);
            // Создаём адаптер для заполнения таблицы данными
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataTable dataTable = new DataTable(); // Таблица для данных
            adapter.Fill(dataTable); // Заполняем таблицу данными

            // Получаем первый элемент из первой строки таблицы (например, ID)
            int roleId = Convert.ToInt32(dataTable.Rows[0].ItemArray.GetValue(0));

            connection.Close(); // Закрываем соединение
            return roleId; // Возвращаем полученное значение
        }

        // Хеширует пароль с помощью SHA-256
        public static string GetHashPass(string password)
        {
            using (var sh = SHA256.Create()) // Создаём алгоритм SHA256
            {
                var shbyte = sh.ComputeHash(Encoding.UTF8.GetBytes(password)); // Вычисляем хеш
                password = BitConverter.ToString(shbyte).Replace("-", "").ToLower(); // Конвертируем в строку, убираем дефисы и делаем нижний регистр
            }

            return password; // Возвращаем хешированный пароль
        }

        // Выполняет SQL-запрос для вставки, обновления или удаления данных
        public static bool InsertUpdateDeleteData(string sql)
        {
            try
            {
                // Создаём подключение и открываем его
                using (MySqlConnection connection = new MySqlConnection(connect.con))
                {
                    connection.Open();
                    // Создаём команду с SQL
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery(); // Выполняем команду (без возвращаемых данных)
                        return true; // Возвращаем успех
                    }
                }
            }
            catch (Exception)
            {
                // В случае ошибки возвращаем false
                return false;
            }
        }
    }
}
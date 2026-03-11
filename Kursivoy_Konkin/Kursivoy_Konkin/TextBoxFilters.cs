
// Используем системные пространства имён, необходимые для работы с вводом, графикой и формами
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

// Объявляем пространство имён курса и интерфейсов работы с текстовыми полями
namespace Kursivoy_Konkin
{
    // Статический класс для набора утилит, связанных с фильтрацией и валидацией текста
    public static class TextBoxFilters
    {
        /// <summary>
        /// Набор утилит для валидации ввода в TextBox и ComboBox.
        /// Совместимо с C# 7.3 и WinForms.
        /// </summary>
        public static class InputValidators
        {
            // Перечисление типов валидаторов
            private enum ValidatorType
            {
                RussianLetters,
                NumericWithDecimal,
                NotEmpty
            }

            // Класс для хранения информации о валидаторе, связанного с контролом
            private class ValidatorInfo
            {
                public ValidatorType Type { get; set; }
                public Color OriginalBackColor { get; set; }
                public int MaxLength { get; set; }
            }

            // Словарь для хранения связки контролов и их валидаторов
            private static readonly Dictionary<Control, ValidatorInfo> _registered =
                new Dictionary<Control, ValidatorInfo>();

            // Цвет, выделяющий ошибочный ввод
            private static readonly Color ErrorBackColor = Color.MistyRose;

            // Метод применяет механизм для разрешения только русских букв в TextBox
            /// <summary>
            /// Только русские буквы (А-Я, а-я, Ё, ё) и пробел, максимум 50 символов.
            /// Блокирует английские буквы, цифры и любые знаки.
            /// </summary>
            public static void ApplyRussianLettersOnly(TextBox textBox)
            {
                // Проверка на null
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                const int maxLen = 50; // Максимальная длина
                textBox.MaxLength = maxLen; // Устанавливаем максимум

                // Переменная для отслеживания внутренних изменений, чтобы избежать рекурсии
                bool internalChange = false;

                // Обработчик для нажатий клавиш, фильтрующий ввод символов
                textBox.KeyPress += (sender, e) =>
                {
                    if (char.IsControl(e.KeyChar)) return; // Разрешаем управляющие символы

                    string ch = e.KeyChar.ToString();
                    // Разрешены только русские буквы и пробел
                    if (!Regex.IsMatch(ch, "^[А-ЯЁа-яё ]$"))
                    {
                        e.Handled = true; // Блокируем некорректный ввод
                    }
                };

                // Обработчик изменения текста
                textBox.TextChanged += (sender, e) =>
                {
                    if (internalChange) return; // Избегаем бесконечной рекурсии
                    internalChange = true;

                    int selStart = textBox.SelectionStart; // Сохраняем позицию курсора
                    string cleaned = FilterRussianLetters(textBox.Text); // Фильтрация

                    // Ограничение по длине
                    if (cleaned.Length > maxLen)
                        cleaned = cleaned.Substring(0, maxLen);

                    // Если текст изменился, обновляем его и позицию курсора
                    if (cleaned != textBox.Text)
                    {
                        textBox.Text = cleaned;
                        textBox.SelectionStart = Math.Min(selStart, textBox.Text.Length);
                    }

                    internalChange = false; // Снимаем флаг
                };

                // Регистрация контролла в системе валидаторов
                RegisterControl(textBox, ValidatorType.RussianLetters, maxLen);
            }

            // Метод для применения фильтрации чисел с точкой или запятой
            /// <summary>
            /// Только целые числа или десятичные с '.' или ',', максимум 10 символов.
            /// </summary>
            public static void ApplyNumericWithDecimal(TextBox textBox)
            {
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                const int maxLen = 10; // Максимальная длина
                textBox.MaxLength = maxLen; // Установка

                bool internalChange = false; // Для избегания рекурсии

                // Обработчик нажатий, разрешает только цифры и запятую/точку
                textBox.KeyPress += (sender, e) =>
                {
                    if (char.IsControl(e.KeyChar)) return; // Разрешаем управляющие символы

                    char c = e.KeyChar;
                    if (char.IsDigit(c)) return; // Разрешены цифры

                    if (c == '.' || c == ',')
                    {
                        string current = textBox.Text;
                        // Не разрешаем более одного разделителя
                        if (current.Contains('.') || current.Contains(','))
                        {
                            e.Handled = true;
                        }
                        return; // Разрешается только один разделитель
                    }

                    e.Handled = true; // Все остальное блокируем
                };

                // Обработчик для фильтрации текста
                textBox.TextChanged += (sender, e) =>
                {
                    if (internalChange) return;
                    internalChange = true;

                    int selStart = textBox.SelectionStart;
                    string cleaned = FilterNumericWithDecimal(textBox.Text);

                    if (cleaned.Length > maxLen)
                        cleaned = cleaned.Substring(0, maxLen);

                    if (cleaned != textBox.Text)
                    {
                        textBox.Text = cleaned;
                        textBox.SelectionStart = Math.Min(selStart, textBox.Text.Length);
                    }

                    internalChange = false;
                };

                // Регистрация
                RegisterControl(textBox, ValidatorType.NumericWithDecimal, maxLen);
            }

            // Метод для проверки, что поле не пустое
            /// <summary>
            /// Проверка на заполненность поля (не пустое значение).
            /// </summary>
            public static void ApplyNotEmptyValidation(Control control)
            {
                if (control == null) throw new ArgumentNullException(nameof(control));

                // Регистрация контролла для проверки некорректности (пустое)
                RegisterControl(control, ValidatorType.NotEmpty, 0);
            }

            // Валидация номера телефона с проверками начала и длины
            /// <summary>
            /// Валидация поля телефона: только цифры и знак "+", удаление пробелов, проверка начала номера и длины.
            /// Теперь поддерживает как TextBox, так и MaskedTextBox.
            /// </summary>
            public static void ApplyPhoneValidation(Control control)
            {
                if (control == null) throw new ArgumentNullException(nameof(control));
                // Проверка на допустимый тип контролла
                if (!(control is TextBox || control is MaskedTextBox))
                    throw new ArgumentException("Control должен быть TextBox или MaskedTextBox.");

                // Обработчик для фильтрации ввода
                control.KeyPress += (sender, e) =>
                {
                    // Разрешают только цифры, "+" и управляющие
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                };

                // Обработчик для валидации содержимого
                control.TextChanged += (sender, e) =>
                {
                    string text = control.Text;

                    // Удаляем пробелы
                    text = text.Replace(" ", "");

                    // Проверка начала номера
                    if (!text.StartsWith("+7") && !text.StartsWith("8"))
                    {
                        control.BackColor = ErrorBackColor; // Если неправильно
                        return;
                    }

                    // Ограничение по длине
                    if ((text.StartsWith("+7") && text.Length > 12) || (text.StartsWith("8") && text.Length > 11))
                    {
                        text = text.Substring(0, text.StartsWith("+7") ? 12 : 11);
                    }

                    // Обновление текста и установление курсора
                    if (control is TextBox tb)
                    {
                        tb.TextChanged -= (EventHandler)((sender2, e2) => { });
                        tb.Text = text;
                        tb.SelectionStart = text.Length;
                        tb.TextChanged += (EventHandler)((sender2, e2) => { });
                    }
                    else if (control is MaskedTextBox mtb)
                    {
                        mtb.TextChanged -= (EventHandler)((sender2, e2) => { });
                        mtb.Text = text;
                        mtb.SelectionStart = text.Length;
                        mtb.TextChanged += (EventHandler)((sender2, e2) => { });
                    }

                    // Сброс цвета при правильном номере
                    control.BackColor = SystemColors.Window;
                };
            }

            // Метод регистрации контролов в системе валидаторов
            private static void RegisterControl(Control control, ValidatorType type, int maxLength)
            {
                if (!_registered.ContainsKey(control))
                {
                    _registered.Add(control, new ValidatorInfo
                    {
                        Type = type,
                        OriginalBackColor = control.BackColor,
                        MaxLength = maxLength
                    });
                }
                else
                {
                    // Обновляем информацию для уже зарегистрированного контрола
                    _registered[control].Type = type;
                    _registered[control].MaxLength = maxLength;
                }
            }

            /// <summary>
            /// Валидирует все зарегистрированные контролы.
            /// Подсвечивает незаполненные или некорректные поля красным цветом.
            /// </summary>
            public static bool ValidateAll(out Control[] invalidControls)
            {
                var invalid = new List<Control>();
                // Перебираем все зарегистрированные контролы
                foreach (var kv in _registered)
                {
                    Control ctrl = kv.Key;
                    ValidatorInfo info = kv.Value;

                    string text = (ctrl.Text ?? string.Empty).Trim(); // очищенный текст

                    bool isValid = true;

                    // Проверка типа валидатора и условий
                    if (info.Type == ValidatorType.NotEmpty && string.IsNullOrWhiteSpace(text))
                    {
                        isValid = false; // Если поле пустое при обязательности
                    }
                    else if (info.Type == ValidatorType.RussianLetters && !Regex.IsMatch(text, "^[А-ЯЁа-яё ]+$"))
                    {
                        isValid = false; // Если содержит не только русские буквы
                    }
                    else if (info.Type == ValidatorType.NumericWithDecimal && !Regex.IsMatch(text, "^[0-9]+([.,][0-9]+)?$"))
                    {
                        isValid = false; // Не соответствует числовому формату
                    }

                    // Специальная проверка для MaskedTextBox, если есть
                    if (ctrl.Name == "maskerTextBox1" && ctrl is MaskedTextBox maskedTextBox)
                    {
                        // Проверка на полноту маски
                        if (!maskedTextBox.MaskFull)
                        {
                            isValid = false; // Маска не заполнена полностью
                        }
                    }

                    try
                    {
                        // В случае некорректного поля устанавливаем красный цвет
                        if (!isValid)
                        {
                            ctrl.BackColor = ErrorBackColor;
                            invalid.Add(ctrl);
                        }
                        else
                        {
                            // В противном случае возвращаем исходный цвет
                            ctrl.BackColor = info.OriginalBackColor;
                        }
                    }
                    catch
                    {
                        // Игнорируем исключения при доступе к BackColor
                    }
                }

                // Выводим список некорректных
                invalidControls = invalid.ToArray();
                return invalid.Count == 0; // Возвращаем есть ли ошибки
            }

            // Внутренние методы фильтрации нежелательных символов
            private static string FilterRussianLetters(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;
                // Оставляем только русские буквы и пробелы
                return Regex.Replace(input, "[^А-Яа-яЁё ]+", "");
            }

            private static string FilterNumericWithDecimal(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;

                // Оставляем только цифры, запятые и точки
                string cleaned = Regex.Replace(input, "[^0-9.,]+", "");

                int firstSepIndex = -1;
                for (int i = 0; i < cleaned.Length; i++)
                {
                    if (cleaned[i] == '.' || cleaned[i] == ',')
                    {
                        firstSepIndex = i; // находим первый разделитель
                        break;
                    }
                }

                if (firstSepIndex >= 0)
                {
                    // Разделитель найден, оставляем только один
                    char sep = cleaned[firstSepIndex];
                    var digitsBefore = Regex.Replace(cleaned.Substring(0, firstSepIndex), "[^0-9]", "");
                    var digitsAfter = Regex.Replace(cleaned.Substring(firstSepIndex + 1), "[^0-9]", "");
                    cleaned = digitsBefore + sep + digitsAfter;
                }
                else
                {
                    // Нет разделителя, оставляем только цифры
                    cleaned = Regex.Replace(cleaned, "[^0-9]", "");
                }

                return cleaned; // Возвращаем очищенный ввод
            }
        }
    }
}

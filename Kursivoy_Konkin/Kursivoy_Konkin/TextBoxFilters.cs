using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    public static class TextBoxFilters
    {
        /// <summary>
        /// Набор утилит для валидации ввода в TextBox и ComboBox.
        /// Совместимо с C# 7.3 и WinForms.
        /// </summary>
        public static class InputValidators
        {
            private enum ValidatorType
            {
                RussianLetters,
                NumericWithDecimal,
                NotEmpty
            }

            private class ValidatorInfo
            {
                public ValidatorType Type { get; set; }
                public Color OriginalBackColor { get; set; }
                public int MaxLength { get; set; }
            }

            private static readonly Dictionary<Control, ValidatorInfo> _registered =
                new Dictionary<Control, ValidatorInfo>();

            private static readonly Color ErrorBackColor = Color.MistyRose;

            /// <summary>
            /// Только русские буквы (А-Я, а-я, Ё, ё) и пробел, максимум 50 символов.
            /// Блокирует английские буквы, цифры и любые знаки.
            /// </summary>
            public static void ApplyRussianLettersOnly(TextBox textBox)
            {
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                const int maxLen = 50;
                textBox.MaxLength = maxLen;

                bool internalChange = false;

                textBox.KeyPress += (sender, e) =>
                {
                    if (char.IsControl(e.KeyChar)) return;

                    string ch = e.KeyChar.ToString();
                    if (!Regex.IsMatch(ch, "^[А-ЯЁа-яё ]$"))
                    {
                        e.Handled = true;
                    }
                };

                textBox.TextChanged += (sender, e) =>
                {
                    if (internalChange) return;
                    internalChange = true;

                    int selStart = textBox.SelectionStart;
                    string cleaned = FilterRussianLetters(textBox.Text);

                    if (cleaned.Length > maxLen)
                        cleaned = cleaned.Substring(0, maxLen);

                    if (cleaned != textBox.Text)
                    {
                        textBox.Text = cleaned;
                        textBox.SelectionStart = Math.Min(selStart, textBox.Text.Length);
                    }

                    internalChange = false;
                };

                RegisterControl(textBox, ValidatorType.RussianLetters, maxLen);
            }

            /// <summary>
            /// Только целые числа или десятичные с '.' или ',', максимум 10 символов.
            /// </summary>
            public static void ApplyNumericWithDecimal(TextBox textBox)
            {
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                const int maxLen = 10;
                textBox.MaxLength = maxLen;

                bool internalChange = false;

                textBox.KeyPress += (sender, e) =>
                {
                    if (char.IsControl(e.KeyChar)) return;

                    char c = e.KeyChar;
                    if (char.IsDigit(c)) return;

                    if (c == '.' || c == ',')
                    {
                        string current = textBox.Text;
                        if (current.Contains('.') || current.Contains(','))
                        {
                            e.Handled = true;
                        }
                        return;
                    }

                    e.Handled = true;
                };

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

                RegisterControl(textBox, ValidatorType.NumericWithDecimal, maxLen);
            }

            /// <summary>
            /// Проверка на заполненность поля (не пустое значение).
            /// </summary>
            public static void ApplyNotEmptyValidation(Control control)
            {
                if (control == null) throw new ArgumentNullException(nameof(control));

                RegisterControl(control, ValidatorType.NotEmpty, 0);
            }

            /// <summary>
            /// Валидация поля телефона: только цифры и знак "+", удаление пробелов, проверка начала номера и длины.
            /// </summary>
            public static void ApplyPhoneValidation(TextBox textBox)
            {
                if (textBox == null) throw new ArgumentNullException(nameof(textBox));

                textBox.KeyPress += (sender, e) =>
                {
                    // Разрешаем только цифры, знак "+" и управляющие символы
                    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '+')
                    {
                        e.Handled = true;
                    }
                };

                textBox.TextChanged += (sender, e) =>
                {
                    string text = textBox.Text;

                    // Удаляем пробелы
                    text = text.Replace(" ", "");

                    // Проверяем начало номера
                    if (!text.StartsWith("+7") && !text.StartsWith("8"))
                    {
                        textBox.BackColor = ErrorBackColor;
                        return;
                    }

                    // Проверяем длину номера
                    if ((text.StartsWith("+7") && text.Length > 12) || (text.StartsWith("8") && text.Length > 11))
                    {
                        text = text.Substring(0, text.StartsWith("+7") ? 12 : 11);
                    }

                    // Применяем изменения
                    textBox.TextChanged -= (EventHandler)((sender2, e2) => { }); // Исправлено удаление обработчика
                    textBox.Text = text;
                    textBox.SelectionStart = text.Length;
                    textBox.TextChanged += (EventHandler)((sender2, e2) => { }); // Исправлено добавление обработчика

                    // Сбрасываем цвет, если номер корректен
                    textBox.BackColor = SystemColors.Window;
                };
            }

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

                foreach (var kv in _registered)
                {
                    Control ctrl = kv.Key;
                    ValidatorInfo info = kv.Value;

                    string text = (ctrl.Text ?? string.Empty).Trim();

                    bool isValid = true;

                    if (info.Type == ValidatorType.NotEmpty && string.IsNullOrWhiteSpace(text))
                    {
                        isValid = false;
                    }
                    else if (info.Type == ValidatorType.RussianLetters && !Regex.IsMatch(text, "^[А-ЯЁа-яё ]+$"))
                    {
                        isValid = false;
                    }
                    else if (info.Type == ValidatorType.NumericWithDecimal && !Regex.IsMatch(text, "^[0-9]+([.,][0-9]+)?$"))
                    {
                        isValid = false;
                    }

                    try
                    {
                        if (!isValid)
                        {
                            ctrl.BackColor = ErrorBackColor;
                            invalid.Add(ctrl);
                        }
                        else
                        {
                            ctrl.BackColor = info.OriginalBackColor;
                        }
                    }
                    catch
                    {
                        // Игнорируем возможные исключения при доступе к BackColor
                    }
                }

                invalidControls = invalid.ToArray();
                return invalid.Count == 0;
            }

            private static string FilterRussianLetters(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;
                return Regex.Replace(input, "[^А-Яа-яЁё ]+", "");
            }

            private static string FilterNumericWithDecimal(string input)
            {
                if (string.IsNullOrEmpty(input)) return string.Empty;

                string cleaned = Regex.Replace(input, "[^0-9.,]+", "");

                int firstSepIndex = -1;
                for (int i = 0; i < cleaned.Length; i++)
                {
                    if (cleaned[i] == '.' || cleaned[i] == ',')
                    {
                        firstSepIndex = i;
                        break;
                    }
                }

                if (firstSepIndex >= 0)
                {
                    char sep = cleaned[firstSepIndex];
                    var digitsBefore = Regex.Replace(cleaned.Substring(0, firstSepIndex), "[^0-9]", "");
                    var digitsAfter = Regex.Replace(cleaned.Substring(firstSepIndex + 1), "[^0-9]", "");
                    cleaned = digitsBefore + sep + digitsAfter;
                }
                else
                {
                    cleaned = Regex.Replace(cleaned, "[^0-9]", "");
                }

                return cleaned;
            }
        }
    }
}


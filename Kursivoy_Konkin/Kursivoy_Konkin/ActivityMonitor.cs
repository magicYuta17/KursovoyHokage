using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;

namespace Kursivoy_Konkin
{
    /// <summary>
    /// Глобальный монитор активности пользователя.
    /// Подключается один раз в Program.cs и работает на протяжении всей сессии.
    /// </summary>
    public static class ActivityMonitor
    {
        private static Timer _timer;
        private static int _timeoutMs;
        private static Action _onLockAction;

        /// <summary>
        /// Запустить мониторинг.
        /// </summary>
        /// <param name="onLock">Действие при блокировке (показать форму входа)</param>
        public static void Start(Action onLock)
        {
            _onLockAction = onLock;

            // Читаем таймаут из App.config (по умолчанию 3 минуты)
            int minutes = GetTimeoutMinutes();
            _timeoutMs = minutes * 60 * 1000;

            // Подключаем фильтр сообщений для перехвата мыши и клавиатуры
            Application.AddMessageFilter(new ActivityMessageFilter(ResetTimer));

            // Создаём таймер
            _timer = new Timer();
            _timer.Interval = _timeoutMs;
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }

        /// <summary>
        /// Сбросить таймер (вызывается при любой активности).
        /// </summary>
        public static void ResetTimer()
        {
            if (_timer == null) return;
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// Остановить мониторинг (например, при выходе из системы).
        /// </summary>
        public static void Stop()
        {
            _timer?.Stop();
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            _onLockAction?.Invoke();
        }

        /// <summary>
        /// Читает таймаут из App.config.
        /// Если значение некорректно — возвращает 3 минуты.
        /// </summary>
        public static int GetTimeoutMinutes()
        {
            string value = ConfigurationManager.AppSettings["InactivityTimeoutMinutes"];
            if (int.TryParse(value, out int minutes) && minutes > 0)
                return minutes;
            return 3;
        }

        /// <summary>
        /// Сохраняет новый таймаут в App.config.
        /// </summary>
        public static void SetTimeoutMinutes(int minutes)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["InactivityTimeoutMinutes"].Value = minutes.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");

            // Обновляем интервал живого таймера
            if (_timer != null)
            {
                _timeoutMs = minutes * 60 * 1000;
                _timer.Interval = _timeoutMs;
                ResetTimer();
            }
        }
    }

    /// <summary>
    /// Перехватчик сообщений Windows — реагирует на мышь и клавиатуру.
    /// </summary>
    internal class ActivityMessageFilter : IMessageFilter
    {
        private readonly Action _onActivity;

        // WM_MOUSEMOVE, WM_LBUTTONDOWN, WM_RBUTTONDOWN,
        // WM_MBUTTONDOWN, WM_KEYDOWN, WM_SYSKEYDOWN
        private static readonly int[] ActivityMessages = { 0x0200, 0x0201, 0x0204, 0x0207, 0x0100, 0x0104 };

        public ActivityMessageFilter(Action onActivity)
        {
            _onActivity = onActivity;
        }

        public bool PreFilterMessage(ref Message m)
        {
            foreach (int msg in ActivityMessages)
            {
                if (m.Msg == msg)
                {
                    _onActivity();
                    break;
                }
            }
            return false; // не блокируем сообщения
        }
    }
}
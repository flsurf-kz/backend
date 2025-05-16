using Microsoft.EntityFrameworkCore;

namespace Flsurf.Domain.User.ValueObjects
{
    [Owned]
    public class NotificationSettings : ValueObject
    {
        // 1) Десктоп
        public bool DesktopNotificationsEnabled { get; private set; }
        public bool DesktopBadgeCountEnabled { get; private set; }

        // 2) Веб-сайт
        public bool WebNotificationsEnabled { get; private set; }
        public bool WebBadgeCountEnabled { get; private set; }

        // 3) E-mail
        public bool EmailNotificationsEnabled { get; private set; }
        public bool EmailWhenOfflineEnabled { get; private set; }

        // 4) Дополнительные настройки
        public bool PushNotificationsEnabled { get; private set; }       // push-уведомления (мобильные)
        public bool PushWhenOfflineEnabled { get; private set; }
        public bool DailySummaryEmailEnabled { get; private set; }       // раз в день сводка
        public TimeSpan? DoNotDisturbStart { get; private set; }         // «тишина» от–до
        public TimeSpan? DoNotDisturbEnd { get; private set; }
        public string? PreferredLanguage { get; private set; }       // язык уведомлений

        // EF Core требует конструктор без параметров
        private NotificationSettings() { }

        /// <summary>Заводские дефолтные настройки</summary>
        public static NotificationSettings CreateDefault() =>
            new NotificationSettings
            {
                DesktopNotificationsEnabled = true,
                DesktopBadgeCountEnabled = true,
                WebNotificationsEnabled = true,
                WebBadgeCountEnabled = true,
                EmailNotificationsEnabled = true,
                EmailWhenOfflineEnabled = false,
                PushNotificationsEnabled = false,
                PushWhenOfflineEnabled = false,
                DailySummaryEmailEnabled = false,
                DoNotDisturbStart = null,
                DoNotDisturbEnd = null,
                PreferredLanguage = "en"
            };

        // Примеры «сеттеров» – возвращают новый объект или модифицируют текущий
        public NotificationSettings ConfigureDesktop(bool enabled, bool badgeCount)
        {
            DesktopNotificationsEnabled = enabled;
            DesktopBadgeCountEnabled = badgeCount;
            return this;
        }

        public NotificationSettings ConfigureWeb(bool enabled, bool badgeCount)
        {
            WebNotificationsEnabled = enabled;
            WebBadgeCountEnabled = badgeCount;
            return this;
        }

        public NotificationSettings ConfigureEmail(bool enabled, bool whenOffline)
        {
            EmailNotificationsEnabled = enabled;
            EmailWhenOfflineEnabled = whenOffline;
            return this;
        }

        public NotificationSettings ConfigurePush(bool enabled, bool whenOffline)
        {
            PushNotificationsEnabled = enabled;
            PushWhenOfflineEnabled = whenOffline;
            return this;
        }

        public NotificationSettings ConfigureDailySummary(bool enabled)
        {
            DailySummaryEmailEnabled = enabled;
            return this;
        }

        public NotificationSettings ConfigureDoNotDisturb(TimeSpan? start, TimeSpan? end)
        {
            DoNotDisturbStart = start;
            DoNotDisturbEnd = end;
            return this;
        }

        public NotificationSettings SetPreferredLanguage(string lang)
        {
            PreferredLanguage = lang;
            return this;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DesktopNotificationsEnabled;
            yield return DesktopBadgeCountEnabled;
            yield return WebNotificationsEnabled;
            yield return WebBadgeCountEnabled;
            yield return EmailNotificationsEnabled;
            yield return EmailWhenOfflineEnabled;
            yield return PushNotificationsEnabled;
            yield return PushWhenOfflineEnabled;
            yield return DailySummaryEmailEnabled;
            yield return DoNotDisturbStart ?? TimeSpan.Zero;
            yield return DoNotDisturbEnd ?? TimeSpan.Zero;
            yield return PreferredLanguage ?? "RU";
        }
    }
}

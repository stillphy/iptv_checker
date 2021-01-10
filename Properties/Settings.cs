using System.Configuration;

namespace Properties
{
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

        public static Settings Default => Settings.defaultInstance;
    }
}

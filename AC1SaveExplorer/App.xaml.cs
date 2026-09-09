using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace AC1SaveExplorer
{
    public partial class App : Application
    {
        public const string Version = "1.0.0";

        public static readonly string[] AvailableThemes = { "Dark", "Light", "Solarized" };

        private static string SettingsDir =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AC1SaveExplorer");
        private static string SettingsPath => Path.Combine(SettingsDir, "settings.json");

        public static AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    var settings = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath));
                    if (settings != null && Array.IndexOf(AvailableThemes, settings.Theme) >= 0)
                        return settings;
                }
            }
            catch { /* fall through to defaults */ }
            return new AppSettings();
        }

        public static void SaveSettings(AppSettings settings)
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings));
            }
            catch { /* not fatal — just won't remember the choice next launch */ }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var settings = LoadSettings();
            ApplyTheme(settings.Theme);

            var window = new MainWindow();
            window.Show();
        }

        // Mutates the SAME Application.Resources dictionary in place (Clear + Add) rather than
        // replacing Resources wholesale. Combined with every Style/Setter in Themes/*.xaml using
        // DynamicResource instead of StaticResource, this is what makes theme switching apply
        // instantly to whatever's already on screen — no restart needed.
        public static void ApplyTheme(string theme)
        {
            if (Array.IndexOf(AvailableThemes, theme) < 0) theme = "Dark";
            var dict = new ResourceDictionary { Source = new Uri($"Themes/{theme}.xaml", UriKind.Relative) };
            Current.Resources.MergedDictionaries.Clear();
            Current.Resources.MergedDictionaries.Add(dict);
        }
    }

    public class AppSettings
    {
        public string Theme { get; set; } = "Dark";
        public bool AlwaysOnTop { get; set; } = false;
    }
}

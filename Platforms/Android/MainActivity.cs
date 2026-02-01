using Android.App;
using Android.Content.PM;
using Android.Content.Res; // Needed for Configuration
using Android.OS;
using AndroidX.Core.View;  // Needed for WindowCompat (Icon colors)

namespace SNotes
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}

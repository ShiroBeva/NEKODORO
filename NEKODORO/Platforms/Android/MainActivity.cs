using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views; // WindowManagerFlags için mutlaka ekle

namespace NEKODORO
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Android'de ekranın en üst ve en altındaki siyah boşlukları kaldırır
            // İçeriği (garden.png) durum çubuğu ve navigasyon çubuğunun altına yayar
            Window.SetFlags(WindowManagerFlags.LayoutNoLimits, WindowManagerFlags.LayoutNoLimits);

            // Eğer Android 11+ kullanıyorsan, ekranın daha stabil görünmesi için:
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                Window.SetDecorFitsSystemWindows(false);
            }
        }
    }
}
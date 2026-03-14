namespace NEKODORO;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Ana sayfayı bir navigasyon tüneli içinde açıyoruz.
        MainPage = new NavigationPage(new MainPage());
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // KRİTİK DÜZELTME: Windows MSIX servisleri (Preferences) için 1.5 saniye bekliyoruz.
        // Bu, image_d96fc7.png'deki InvalidOperationException hatasını çözer.
        await Task.Delay(1500);

        try
        {
            // Preferences erişimini güvenli bir şekilde ana thread üzerinden yapıyoruz.
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Hata Listesindeki CS0121'e sebep olan MainPage metodlarını buradan sildim.
                string lang = Preferences.Default.Get("App_Language", "tr-TR");
                var culture = new System.Globalization.CultureInfo(lang);
                System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
                System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Başlangıç ayarları yüklenemedi: {ex.Message}");
        }
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(MainPage ?? new NavigationPage(new MainPage()));

#if WINDOWS
        window.HandlerChanged += (s, e) =>
        {
            // Pencere oluşturulduktan sonra başlık çubuğunu şeffaf yapıyoruz.
            var nativeWindow = window.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
            if (nativeWindow != null)
            {
                nativeWindow.ExtendsContentIntoTitleBar = true;
                var titleBar = nativeWindow.AppWindow.TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Microsoft.UI.Colors.Transparent;
                    titleBar.ButtonInactiveBackgroundColor = Microsoft.UI.Colors.Transparent;
                    titleBar.ButtonForegroundColor = Microsoft.UI.Colors.White;
                }
            }
        };
#endif
        return window;
    }
}
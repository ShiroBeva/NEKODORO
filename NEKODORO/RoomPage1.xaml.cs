using Microsoft.Maui.Layouts;
using NEKODORO.Resources;

namespace NEKODORO;

public partial class RoomPage1 : ContentPage
{
    private Image _selectedItem; // Kaldırılmak üzere seçilen eşya
    private double _startX, _startY; 
    public RoomPage1()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadUserData();
        LoadDecorations();
    }

    private void LoadDecorations()
    {
        // Tavşanı koruyarak her şeyi temizle
        var bunny = DecorationArea.Children.FirstOrDefault(c => (c as Image)?.Source?.ToString().Contains("bunny") == true);
        DecorationArea.Children.Clear();
        StorageItemsContainer.Children.Clear();
        if (bunny != null) DecorationArea.Children.Add(bunny);

        string[] ids = { "item_1", "item_2", "item_3", "item_4", "item_5", "item_6", "item_7", "item_8", "item_9", "item_10", "item_11"};
        string[] images = { "bunnyattack.gif", "bunnycarrot.gif", "bunnydead.gif", "bunnyhurt.gif", "bunnyidle.gif", "bunnyjump.gif", "bunnyliedown.gif", "bunnyrun.gif", "bunnysitting.gif", "bunnysleep.gif" };

        for (int i = 0; i < ids.Length; i++)
        {
            string currentId = ids[i];

            if (Preferences.Get($"Owned_{currentId}", false))
            {
                var img = CreateDecorativeImage(currentId, images[i]);

                if (Preferences.Get($"Placed_{currentId}", false))
                {
                    PlaceInRoom(img, currentId);
                }
                else
                {
                    PlaceInStorage(img);
                }
            }
        }
    }

    private Image CreateDecorativeImage(string id, string source)
    {
        // 1. GIF desteği ve temel ayarlar
        var img = new Image
        {
            Source = source,
            WidthRequest = 80,
            HeightRequest = 80,
            AutomationId = id,
            IsAnimationPlaying = true // GIF'lerin marketten geldiğinde oynamasını sağlar
        };

        // 2. Sürükleme (Pan) Tanımlama
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated; // Daha önce düzelttiğimiz _startX/_startY mantığını kullanır
        img.GestureRecognizers.Add(panGesture);

        // 3. Dokunma (Tap) Tanımlama
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += (s, e) => {
            // Dekorasyon modu (StorageBar) açık değilse eşya seçilemez
            if (!StorageBar.IsVisible) return;

            _selectedItem = img;
            RemoveItemBtn.IsVisible = true; // Silme butonunu gösterir
        };
        img.GestureRecognizers.Add(tapGesture);

        return img;
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (!StorageBar.IsVisible) return;

        var view = sender as View;
        if (view == null) return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                // Hareket başladığında mevcut pozisyonu kaydet
                _startX = view.TranslationX;
                _startY = view.TranslationY;
                break;

            case GestureStatus.Running:
                // Mevcut pozisyona toplam hareketi ekle
                view.TranslationX = _startX + e.TotalX;
                view.TranslationY = _startY + e.TotalY;
                break;

            case GestureStatus.Completed:
                // Hareket bittiğinde yeni yerini Preferences'a kaydet
                string itemId = view.AutomationId;
                if (!string.IsNullOrEmpty(itemId))
                {
                    Preferences.Set($"TransX_{itemId}", view.TranslationX);
                    Preferences.Set($"TransY_{itemId}", view.TranslationY);
                }
                break;
        }
    }

    private void PlaceInRoom(Image img, string id)
    {
        Preferences.Set($"Placed_{id}", true);

        img.TranslationX = Preferences.Get($"TransX_{id}", 0.0);
        img.TranslationY = Preferences.Get($"TransY_{id}", 0.0);

        if (StorageItemsContainer.Children.Contains(img))
            StorageItemsContainer.Children.Remove(img);

        if (!DecorationArea.Children.Contains(img))
            DecorationArea.Children.Add(img);

        img.GestureRecognizers.Clear();

        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        img.GestureRecognizers.Add(pan);

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => {
            if (!StorageBar.IsVisible) return;
            _selectedItem = img;
            RemoveItemBtn.IsVisible = true;
        };
        img.GestureRecognizers.Add(tap);

        AbsoluteLayout.SetLayoutBounds(img, new Rect(0.5, 0.5, 80, 80));
        AbsoluteLayout.SetLayoutFlags(img, AbsoluteLayoutFlags.PositionProportional);
    }

    private void PlaceInStorage(Image img)
    {
        string id = img.AutomationId;
        Preferences.Set($"Placed_{id}", false);

        if (DecorationArea.Children.Contains(img))
            DecorationArea.Children.Remove(img);

        if (!StorageItemsContainer.Children.Contains(img))
            StorageItemsContainer.Children.Add(img);

        img.GestureRecognizers.Clear();
        var storageTap = new TapGestureRecognizer();
        storageTap.Tapped += (s, e) => PlaceInRoom(img, id);
        img.GestureRecognizers.Add(storageTap);

        img.TranslationX = 0;
        img.TranslationY = 0;
    }

    private void OnRemoveItemClicked(object sender, EventArgs e)
    {
        if (_selectedItem != null)
        {
            PlaceInStorage(_selectedItem); // Seçili eşyayı depoya geri gönderir
            RemoveItemBtn.IsVisible = false; // Butonu gizler
            _selectedItem = null;
        }
    }

    private void OnDecorateClicked(object sender, EventArgs e)
    {
        StorageBar.IsVisible = !StorageBar.IsVisible;
        RemoveItemBtn.IsVisible = false;

        if (StorageBar.IsVisible) {
            LoadDecorations();
            
        }
    }

    private void LoadUserData()
    {
        RoomUserNameLabel.Text = Preferences.Get("User_Name", "Tavşansever");
        CoinLabel.Text = Preferences.Get("User_Coin", 0).ToString();
        string profilePath = Preferences.Get("User_ProfilePath", "user_avatar.png");
        RoomProfileImg.Source = profilePath;
    }

    private async void OnPomoClicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet(AppResources.SelectTimeTitle, AppResources.CancelBtn, null,
         AppResources.Pomo30Min, AppResources.Pomo1Hour, AppResources.Pomo2Hours);

        if (action == AppResources.Pomo30Min) await Navigation.PushAsync(new TimerPage(30, 5));
        else if (action == AppResources.Pomo1Hour) await Navigation.PushAsync(new TimerPage(60, 10));
        else if (action == AppResources.Pomo2Hours) await Navigation.PushAsync(new TimerPage(120, 25));
    }

    // --- DİL DEĞİŞTİRME BURADA GÜNCELLENDİ ---
    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        string action = await DisplayActionSheet(AppResources.SettingsTitle, AppResources.CloseBtn, null,
            AppResources.ChangeLang, AppResources.ResetData);

        if (action == AppResources.ChangeLang)
        {
            OnChangeLanguageClicked(sender, e);
        }
        else if (action == AppResources.ResetData)
        {
            bool confirm = await DisplayAlert(AppResources.ResetConfirmTitle, AppResources.ResetConfirmMsg, AppResources.YesBtn, AppResources.NoBtn);
            if (confirm)
            {
                Preferences.Clear();
                // Veriler silindikten sonra direkt ana girişe atar
                await Navigation.PopToRootAsync();
            }
        }
    }

    // XAML'dan butonla çağrılabilmesi için (object sender, EventArgs e) eklendi
    private async void OnChangeLanguageClicked(object sender, EventArgs e)
    {
        try
        {
            string action = await DisplayActionSheet(AppResources.SelectLangTitle, AppResources.CancelBtn, null, "Türkçe", "English");

            if (action == AppResources.CancelBtn || string.IsNullOrEmpty(action)) return;

            string cultureCode = (action == "Türkçe") ? "tr-TR" : "en-US";
            var culture = new System.Globalization.CultureInfo(cultureCode);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            Preferences.Set("App_Language", cultureCode);

            // UI iş parçacığında (MainThread) güvenli bir şekilde yönlendirme yap
            MainThread.BeginInvokeOnMainThread(() => {
                Application.Current.MainPage = new NavigationPage(new MainPage());
            });
        }
        catch (Exception ex)
        {
            // Hata varsa burada yakala ve mesaj bas ki neden çöktüğünü görelim
            await DisplayAlert("Error", ex.Message, "OK");
        }

    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        // En baştaki sayfaya (MainPage) güvenli bir şekilde döner
        await Navigation.PopToRootAsync();
    }

    private async void OnShopClicked(object sender, EventArgs e) => await Navigation.PushAsync(new ShopPage());
    private async void OnCalendarClicked(object sender, EventArgs e) => await Navigation.PushAsync(new CalendarPage());
}
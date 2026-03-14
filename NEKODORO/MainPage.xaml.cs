using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;
using NEKODORO.Resources;

namespace NEKODORO;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Windows servislerinin (Preferences) hazır olması için bekliyoruz.
        // 1.5 saniye MSIX paketli uygulamalar için güvenli bir süredir.
        await Task.Delay(1500);

        // UI elemanlarına güvenli erişim için Dispatcher kullanıyoruz.
        Dispatcher.Dispatch(() =>
        {
            LoadSavedProfileImage();
        });
    }

    private void LoadSavedProfileImage()
    {
        try
        {
            // Preferences.Default kullanımı Windows üzerinde daha kararlıdır.
            string savedPath = Preferences.Default.Get("User_ProfilePath", "");

            // ProfileImg null kontrolü, sayfa henüz tamamen oluşmadıysa çökmesini önler.
            if (!string.IsNullOrEmpty(savedPath) && ProfileImg != null)
            {
                ProfileImg.Source = ImageSource.FromFile(savedPath);
            }
        }
        catch (Exception ex)
        {
            // Hata olsa bile uygulama kapanmaz, sadece Debug konsoluna yazar.
            System.Diagnostics.Debug.WriteLine($"Profil resmi yükleme hatası: {ex.Message}");
        }
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync();
            if (result != null)
            {
                var localFilePath = result.FullPath;
                ProfileImg.Source = ImageSource.FromFile(localFilePath);
                Preferences.Default.Set("User_ProfilePath", localFilePath);
            }
        }
        catch (Exception ex)
        {
            // AppResources kullanarak yerelleştirilmiş hata mesajı gösterir.
            await DisplayAlert(AppResources.ErrorTitle, $"{AppResources.ImageErrorMsg} {ex.Message}", AppResources.OkBtn);
        }
    }

    private async void Button_Click1(object sender, EventArgs e)
    {
        string name = ProfileNameEntry.Text;
        string savedName = Preferences.Default.Get("User_Name", "");

        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(savedName))
        {
            await DisplayAlert(AppResources.PawsErrorTitle, AppResources.EnterNameMsg, AppResources.OkBtn);
            return;
        }

        if (!string.IsNullOrWhiteSpace(name))
            Preferences.Default.Set("User_Name", name);

        // İlk kez giriş yapan kullanıcıya başlangıç coin'i veriyoruz.
        if (Preferences.Default.Get("User_Coin", -1) == -1)
            Preferences.Default.Set("User_Coin", 50);

        await Navigation.PushAsync(new RoomPage1());
    }

    private async void OnSaveProfileClicked(object sender, EventArgs e)
    {
        string newName = ProfileNameEntry.Text;
        if (string.IsNullOrWhiteSpace(newName))
        {
            await DisplayAlert(AppResources.WarningTitle, AppResources.ChooseNameMsg, AppResources.OkBtn);
            return;
        }

        Preferences.Default.Set("User_Name", newName);

        // Başarı mesajı eklenebilir.
        await DisplayAlert(AppResources.SuccessTitle, AppResources.ProfileUpdatedMsg, AppResources.GreatBtn);

        ProfilePopup.IsVisible = false;
    }

    private void OnClosePopupClicked(object sender, EventArgs e) => ProfilePopup.IsVisible = false;

    private void OnProfileClicked(object sender, EventArgs e)
    {
        string savedName = Preferences.Default.Get("User_Name", "");
        if (!string.IsNullOrEmpty(savedName))
            ProfileNameEntry.Text = savedName;

        ProfilePopup.IsVisible = true;
    }
}
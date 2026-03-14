using NEKODORO.Models;
using NEKODORO.Resources;

namespace NEKODORO;

public partial class TimerPage : ContentPage
{
    TimeSpan _leftTime;
    IDispatcherTimer _timer;
    int _earnedCoins;
    int _totalMinutes;

    public TimerPage(int minutes, int coins)
    {
        InitializeComponent();

        _totalMinutes = minutes;
        _earnedCoins = coins;
        _leftTime = TimeSpan.FromMinutes(minutes);

        // İlk gösterimi yap (Timer başlamadan önce ekranda 1.23.13 formatında görünsün)
        UpdateTimerDisplay();

        // Zamanlayıcıyı kur
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => {
            _leftTime = _leftTime.Subtract(TimeSpan.FromSeconds(1));

            UpdateTimerDisplay();

            if (_leftTime.TotalSeconds <= 0)
            {
                _timer.Stop();
                MainThread.BeginInvokeOnMainThread(async () => await CompleteSession());
            }
        };
        _timer.Start();
    }

    private void UpdateTimerDisplay()
    {
        // İsteğin üzerine: Saat.Dakika.Saniye (Örn: 1.23.13) formatı
        if (_leftTime.TotalHours >= 1)
        {
            // %h: Tek haneli saat, \.: Sabit nokta, mm: Çift haneli dakika, ss: Çift haneli saniye
            TimerLabel.Text = _leftTime.ToString(@"%h\.mm\.ss");
        }
        else
        {
            // 1 saatin altındaysa sadece Dakika.Saniye (Örn: 23.13)
            TimerLabel.Text = _leftTime.ToString(@"mm\.ss");
        }
    }

    private async Task CompleteSession()
    {
        // 1. Coinleri Kaydet (Preferences)
        int currentCoins = Preferences.Get("User_Coin", 0);
        Preferences.Set("User_Coin", currentCoins + _earnedCoins);

        // 2. Veritabanına Kaydet (SQLite)
        var db = new DatabaseService();
        await db.AddSessionAsync(_totalMinutes, _earnedCoins);

        // Dil desteğiyle tebrik mesajı
        string message = string.Format(AppResources.EarnedCoinMsg, _earnedCoins);
        await DisplayAlert(AppResources.CongratsTitle, message, AppResources.GreatBtn);

        await Navigation.PopAsync();
    }

    private async void OnQuitClicked(object sender, EventArgs e)
    {
        // Dil desteğiyle vazgeçme onayı
        bool answer = await DisplayAlert(
            AppResources.QuitConfirmTitle,
            AppResources.QuitConfirmMsg,
            AppResources.YesBtn,
            AppResources.NoBtn);

        if (answer)
        {
            _timer.Stop();
            await Navigation.PopAsync();
        }
    }
}
using NEKODORO.Models;

namespace NEKODORO;

public partial class CalendarPage : ContentPage
{
    DatabaseService _dbService;

    public CalendarPage()
    {
        InitializeComponent();
        _dbService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Veritabanındaki tüm seansları getir
        var sessions = await _dbService.GetSessionsAsync();

        // Listeyi tarihe göre en yeniden en eskiye sırala
        SessionsList.ItemsSource = sessions.OrderByDescending(s => s.Date).ToList();
    }
}
using NEKODORO.Resources; // Dil desteği için

namespace NEKODORO;

// ShopItem sınıfını sayfa dışında ama aynı namespace içinde tutuyoruz.
public class ShopItem
{
    public string Name { get; set; }
    public int Price { get; set; }
    public string Image { get; set; }
    public string Id { get; set; }
}

public partial class ShopPage : ContentPage
{
    // Items listesini observable hale getirdik veya direkt Property olarak tanımladık
    public List<ShopItem> Items { get; set; }

    public ShopPage()
    {
        InitializeComponent();
        BindingContext = this;

        LoadItems();
        UpdateCoinDisplay();
    }

    void LoadItems()
    {
        Items = new List<ShopItem>
        {
            new ShopItem { Id="item_1", Name = "Bunny Attack", Price = 20, Image = "bunnyattack.gif" },
            new ShopItem { Id="item_2", Name = "Bunny Carrot", Price = 20, Image = "bunnycarrot.gif" },
            new ShopItem { Id="item_3", Name = "Bunny..", Price = 20, Image = "bunnydead.gif" },
            new ShopItem { Id="item_4", Name = "Bunny Hurt", Price = 20, Image = "bunnyhurt.gif" },
            new ShopItem { Id="item_5", Name = "Bunny Standing", Price = 20, Image = "bunnyidle.gif" },
            new ShopItem { Id="item_6", Name = "Bunny Jump", Price = 20, Image = "bunnyjump.gif" },
            new ShopItem { Id="item_7", Name = "Bunny Lie Down", Price = 20, Image = "bunnyliedown.gif" },
            new ShopItem { Id="item_8", Name = "Bunny Runn", Price = 20, Image = "bunnyrun.gif" },
            new ShopItem { Id="item_9", Name = "Bunny Sitting", Price = 20, Image = "bunnysitting.gif" },
            new ShopItem { Id="item_10", Name = "Bunny Sleep", Price = 20, Image = "bunnysleep.gif" },
        };

        // UI elementine bağlama işlemini metodun İÇİNDE yapıyoruz
        if (ShopItemsList != null)
        {
            ShopItemsList.ItemsSource = Items;
        }
    }

    void UpdateCoinDisplay()
    {
        // Preferences'tan coin bilgisini güvenli bir şekilde alıp ekrana yazıyoruz
        int coins = Preferences.Get("User_Coin", 0);
        CurrentCoinLabel.Text = coins.ToString();
    }

    private async void OnBuyClicked(object sender, EventArgs e)
    {
        try
        {
            var button = sender as Button;

            // Veri tipi dönüşümü sırasında hata oluşmaması için güvenli 'as' kullanımı
            var item = button?.CommandParameter as ShopItem;

            if (item == null)
            {
                // Eğer buton parametresi boş gelirse uygulamayı kırmadan çıkıyoruz
                return;
            }

            int currentCoins = Preferences.Get("User_Coin", 0);

            if (currentCoins >= item.Price)
            {
                // 1. Coin düşür
                Preferences.Set("User_Coin", currentCoins - item.Price);

                // 2. Eşyayı envantere ekle (Kalıcı olarak)
                Preferences.Set($"Owned_{item.Id}", true);

                // 3. Odaya eklenecek resim listesine ekle
                string currentPlacedItems = Preferences.Get("Placed_Items_Images", "");
                if (string.IsNullOrEmpty(currentPlacedItems))
                    currentPlacedItems = item.Image;
                else
                    currentPlacedItems += "," + item.Image;

                Preferences.Set("Placed_Items_Images", currentPlacedItems);

                // Ekranı güncelle
                UpdateCoinDisplay();

                // Dil destekli başarı mesajı - AppResources içinde bu anahtarların olduğundan emin ol!
                string successMsg = string.Format(AppResources.BuySuccessMsg, item.Name);
                await DisplayAlert(AppResources.BuySuccessTitle, successMsg, AppResources.HoorayBtn);
            }
            else
            {
                // Dil destekli hata mesajı
                await DisplayAlert(AppResources.NotEnoughCoinsTitle, AppResources.NotEnoughCoinsMsg, AppResources.OkBtn);
            }
        }
        catch (Exception ex)
        {
            // Eğer hala bir yerde "Debugger.Break" alırsan, bu mesaj sana hatayı söyleyecek
            await DisplayAlert("Hata!", "Market işlemi sırasında bir sorun oluştu: " + ex.Message, "Tamam");
        }
    }
}
using PizzApp.Models;
using PizzApp.Services;
using PizzApp.Utils;
using Xamarin.Forms;

namespace PizzApp.Views
{
    public partial class PizzeriaPage : ContentPage
    {
        public PizzeriaPage()
        {
            InitializeComponent();            

        }

        protected override void OnDisappearing()
        {
           
            MessagingCenter.Unsubscribe<string>(this, "CaricaPizzerie");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<string>(this, "CaricaPizzerie", async (item) =>
            {
                bool ok = await RealmDataStore.Connect(Settings.PUBLIC_PARTITION);

                var lista = RealmDataStore.ListaPizzerie();

                ListaPizzerie.ItemsSource = lista;

            });

            if (ListaPizzerie.ItemsSource == null)
            {
                MessagingCenter.Send("MESSAGGIO", "CaricaPizzerie");

            }

            base.OnAppearing();
        }

        async void ListaPizzerie_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            await Navigation.PushAsync(new OrdinePage((e.Item as Pizzeria).ID));
        }

        void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}

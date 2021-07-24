using PizzApp.Models;
using PizzApp.Services;
using Xamarin.Forms;
using System.Linq;

namespace PizzApp.Views
{
    public partial class OrdinePage : ContentPage
    {
        private string Partition = null;
        private Pizzeria Pizzeria = null;
        public Ordine Ordine { get; set; }
        public string NPizze { get; set; }

        public OrdinePage(string partition)
        {
            InitializeComponent();

            BindingContext = this;

            Partition = partition;

        }

        protected override void OnDisappearing()
        {

            MessagingCenter.Unsubscribe<string>(this, "CaricaPizze");

            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<string>(this, "CaricaPizze", async (item) =>
            {
                bool ok = await RealmDataStore.Connect(Partition);

                Pizzeria = RealmDataStore.Pizzeria(Partition);

                Title = Pizzeria.Nome;

                var lista = RealmDataStore.ListaPizze();

                ListaPizze.ItemsSource = lista;

                //Verifico se esiste un'ordine aperto di questo utente in questa pizzeria

                Ordine =  RealmDataStore.CercaCreaOrdine(App.Username, Pizzeria);

                var listaPizze = RealmDataStore.ListaPizzeOrdine(Ordine);

                NPizze = "Numero Pizze: " + listaPizze.ToList().Sum(ro => ro.Quantita);

                OnPropertyChanged(nameof(NPizze));

            });

            MessagingCenter.Send("MESSAGGIO", "CaricaPizze");            

            base.OnAppearing();
        }

        async void ListaPizze_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            Pizza pizza = e.Item as Pizza;
            string titolo = "Aggiungi pizza " + pizza.Nome;
            string[] msg= { "Sì" };
            string risposta = await DisplayActionSheet(titolo, "No", null, msg);
            if (risposta == msg[0])
            {
                var listaPizze = RealmDataStore.AggiungiPizzaInOrdine(Ordine, pizza);
                NPizze = "Numero Pizze: " + listaPizze.ToList().Sum(ro => ro.Quantita);
                OnPropertyChanged(nameof(NPizze));
            }
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DettaglioOrdinePage(Partition, Ordine));
        }
    }
}

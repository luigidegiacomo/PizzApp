using PizzApp.Models;
using PizzApp.Services;
using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using System;

namespace PizzApp.Views
{
    public partial class DettaglioOrdinePage : ContentPage
    {
        public Ordine Ordine { get; set; }

        private string Partition = null;

        public string NPizze { get; set; }
        public string Totale { get; set; }


        private IEnumerable<RigaOrdine> ListaPizzeOrdine = null;

        public DettaglioOrdinePage(string partition, Ordine ordine)
        {
            InitializeComponent();

            Partition = partition;

            Ordine = ordine;

            BindingContext = this;

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

                ListaPizzeOrdine = RealmDataStore.ListaPizzeOrdine(Ordine);

                ListaPizze.ItemsSource = ListaPizzeOrdine;

                NPizze = "Numero Pizze: " + ListaPizzeOrdine.ToList().Sum(ro => ro.Quantita);
                Totale = "Totale: " + ListaPizzeOrdine.ToList().Sum(ro => ro.Prezzo).ToString("€ #.#0");
                OnPropertyChanged(nameof(NPizze));
                OnPropertyChanged(nameof(Totale));
            });

            MessagingCenter.Send("MESSAGGIO", "CaricaPizze");            

            base.OnAppearing();
        }

        async void ListaPizze_ItemTapped(System.Object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            var rigaOrdine = e.Item as RigaOrdine;
            string msg1 = "Aggiungi una pizza " + rigaOrdine.Pizza.Nome;
            string msg2 = "Elimina una pizza " + rigaOrdine.Pizza.Nome;
            string msg3 = "Elimina";

            string[] msg= { msg1,msg2 };
            string risposta = await DisplayActionSheet("Modifica Ordine", "Annulla", msg3, msg);
            if (risposta == msg1)
            {
                RealmDataStore.AggiungiPizzaInRigaOrdine(rigaOrdine);               
            }
            if (risposta == msg2)
            {
                RealmDataStore.EliminaPizzaInRigaOrdine(rigaOrdine);
            }
            if (risposta == msg3)
            {
                RealmDataStore.EliminaRigaOrdine(rigaOrdine);
            }
            
            NPizze = "Numero Pizze: " + ListaPizzeOrdine.ToList().Sum(ro => ro.Quantita);
            Totale = "Totale: " + String.Format("{0:C}", ListaPizzeOrdine.ToList().Sum(ro => ro.Prezzo));
            OnPropertyChanged(nameof(NPizze));
            OnPropertyChanged(nameof(Totale));
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            RealmDataStore.ComfermaOrdine(Ordine);
            await Navigation.PopAsync();
        }

        async void ToolbarItem_Clicked(System.Object sender, System.EventArgs e)
        {
            RealmDataStore.EliminaOrdine(Ordine);
            await Navigation.PopAsync();
        }
    }
}

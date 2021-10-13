using System;
using System.Collections.Generic;
using PizzApp.Services;
using PizzApp.Utils;
using Xamarin.Forms;

namespace PizzApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();

            Username.Text = App.Username;
            Password.Text = App.Password;
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {

            if(String.IsNullOrEmpty(Username.Text) || String.IsNullOrEmpty(Password.Text)) {
                await DisplayAlert("Errore", "Credenziali non valide", "Ok");
                return;
            }
                      
            try
            {
                bool ok = await RealmDataStore.Login(Username.Text.ToLower(), Password.Text);

                if (!ok)
                {
                    await DisplayAlert("Errore", "Credenziali non valide", "Ok");
                    return;
                }

                App.Username = Username.Text;
                App.Password = Password.Text;
                Application.Current.MainPage = new NavigationPage(new PizzeriaPage());

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                await DisplayAlert("Errore", "Credenziali non valide", "Ok");
                return;
            }
            

        }
    }
}

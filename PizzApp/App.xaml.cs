using System;
using PizzApp.Services;
using PizzApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PizzApp
{
    public partial class App : Application
    {

        public static string Username { get; set; }
        public static string Password { get; set; }

        public App()
        {
            InitializeComponent();

            RealmDataStore.SetApp(true);

            MainPage = new LoginPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

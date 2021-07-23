using System;
using System.Threading.Tasks;
using Realms;
using Realms.Sync;
using PizzApp.Utils;
using PizzApp.Models;
using System.Linq;
using System.Collections.Generic;

namespace PizzApp.Services
{
    public static class RealmDataStore
    {
        private static Realm Realm;
        private static string Partition;

        async public static Task<bool> Connect(string partition)
        {
            try
            {
                var app = Realms.Sync.App.Create(new AppConfiguration(Settings.MongoDbRealmAppId));
                var user = await app.LogInAsync(Credentials.Anonymous());
                var config = new SyncConfiguration(partition, user);
                Realm = await Realm.GetInstanceAsync(config);
                Partition = partition;
                Console.WriteLine(config.DatabasePath);
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        internal static Pizzeria Pizzeria(string partition)
        {
            var lista = Realm.All<Pizzeria>().Where(p => p.Partition == partition);
            return lista.First();
        }

        internal static Ordine CercaCreaOrdine(string username, Pizzeria pizzeria)
        {
            var lista = Realm.All<Utente>().Where(u => u.Username == username);

            Utente utente = null;

            if(lista.Any())
            {
                utente = lista.First();
            }
            else 
            {
                var trans = Realm.BeginWrite();
                try
                {                   
                    utente = new Utente
                    {
                        Partition = Partition,
                        Username = username
                    };
                    Realm.Add(utente);
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                }                
            }
            //Cerco ordine
            var listaOrdini = Realm.All<Ordine>().Where(o => o.Utente == utente && o.Pizzeria == pizzeria);

            Ordine ordine = null;
            if (listaOrdini.Any())
            {
                ordine = listaOrdini.First();
            }
            else
            {
                var trans = Realm.BeginWrite();
                try
                {
                    ordine = new Ordine
                    {
                        Partition = Partition,
                        Utente = utente,
                        Pizzeria = pizzeria,
                        Data = DateTime.Now,
                        Chiuso = false
                    };
                    Realm.Add(ordine);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                }
            }

            return ordine;

        }

        internal static IEnumerable<RigaOrdine> AggiungiPizzaInOrdine(Ordine ordine, Pizza pizza)
        {
            var lista = Realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine && ro.Pizza==pizza);
            RigaOrdine rigaOrdine = null;
            var trans = Realm.BeginWrite();
            try
            {

                if (lista.Any())
                {
                    rigaOrdine = lista.First();
                    rigaOrdine.Quantita++;
                }
                else
                {
                    rigaOrdine = new RigaOrdine
                    {
                        Partition = Partition,
                        Ordine = ordine,
                        Pizza = pizza,
                        Quantita = 1
                    };
                    Realm.Add(rigaOrdine);
                }
                trans.Commit();

                var listaPizze = Realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine);
                return listaPizze;
            }
            catch(Exception)
            {
                trans.Rollback();
                return null;
            }           
        }

        internal static IEnumerable<RigaOrdine> ListaPizzeOrdine(Ordine ordine)
        {
            var lista = Realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine);
            return lista;
        }

        internal static IEnumerable<Pizza> ListaPizze()
        {
            var lista = Realm.All<Pizza>().OrderBy(p => p.Prezzo).ThenBy(p=>p.Nome);
            return lista;
        }

        internal static IEnumerable<Pizzeria> ListaPizzerie()
        {
            var lista = Realm.All<Pizzeria>().OrderBy(p => p.Nome);
            return lista;
        }

        internal static bool Login(string username, string password)
        {
            var lista = Realm.All<Utente>().Where(u => u.Username == username && u.Password == password);
            return lista.Any();
        }
      
    }
}

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

        private static bool IsApp;

        public static void SetApp(bool isApp)
        {
            IsApp = isApp;
        }

        async private static Task<Realm> Connect(string partition)
        {
            try
            {
                if (IsApp && Realm != null && Partition == partition) return Realm;

                var app = Realms.Sync.App.Create(new AppConfiguration(Settings.MongoDbRealmAppId));
                var user = await app.LogInAsync(Credentials.Anonymous());
                var config = new SyncConfiguration(partition, user);
                var realm = await Realm.GetInstanceAsync(config);
                realm.Refresh();
                Partition = partition;
                Realm = realm;
                Console.WriteLine(config.DatabasePath);
                return Realm;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }       

        internal async static Task<Pizzeria> Pizzeria(string partition)
        {
            var realm = await Connect(partition);
            var lista = realm.All<Pizzeria>().Where(p => p.Partition == partition);
            return lista.First();
        }

        internal async static Task<Ordine> CercaCreaOrdine(string partition,string username, Pizzeria pizzeria)
        {
            var realm = await Connect(partition);

            var lista = realm.All<Utente>().Where(u => u.Username == username);

            Utente utente = null;

            if(lista.Any())
            {
                utente = lista.First();
            }
            else 
            {
                var trans = realm.BeginWrite();
                try
                {                   
                    utente = new Utente
                    {
                        Partition = Partition,
                        Username = username
                    };
                    realm.Add(utente);
                    trans.Commit();
                }
                catch(Exception)
                {
                    trans.Rollback();
                }                
            }
            //Cerco ordine
            var listaOrdini = realm.All<Ordine>().Where(o => o.Utente == utente && o.Pizzeria == pizzeria);

            Ordine ordine = null;
            if (listaOrdini.Any())
            {
                ordine = listaOrdini.First();
            }
            else
            {
                var trans = realm.BeginWrite();
                try
                {
                    ordine = new Ordine
                    {
                        Partition = Partition,
                        Utente = utente,
                        Pizzeria = pizzeria,
                        Data = DateTime.Now,
                        Confermato = false,
                        Chiuso = false
                    };
                    realm.Add(ordine);
                    trans.Commit();
                }
                catch (Exception)
                {
                    trans.Rollback();
                }
            }

            return ordine;

        }

        internal async static Task<bool> ConfermaOrdine(string partition, Ordine ordine)
        {
            var realm = await Connect(partition);

            var trans = realm.BeginWrite();
            try
            {

                ordine.Confermato = true; 

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();

                return false;
            }
        }

        internal async static Task<bool> EliminaOrdine(string partition, Ordine ordine)
        {
            var realm = await Connect(partition);

            var trans = realm.BeginWrite();
            try
            {

                var lista = realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine);

                realm.RemoveRange<RigaOrdine>(lista);

                realm.Remove(ordine);

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                return false;
            }
        }

        internal async static Task<bool> EliminaRigaOrdine(string partition, RigaOrdine rigaOrdine)
        {
            var realm = await Connect(partition);

            var trans = realm.BeginWrite();
            try
            {

                realm.Remove(rigaOrdine);

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();

                return false;
            }
        }

        internal async static Task<bool> AggiungiPizzaInRigaOrdine(string partition, RigaOrdine rigaOrdine)
        {
            var realm = await Connect(partition);

            var trans = realm.BeginWrite();
            try
            {
                rigaOrdine.Quantita++;

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();

                return false;
            }
        }

        internal async static Task<bool> EliminaPizzaInRigaOrdine(string partition ,RigaOrdine rigaOrdine)
        {
            var realm = await Connect(partition);

            var trans = realm.BeginWrite();
            try
            {
                rigaOrdine.Quantita--;

                if (rigaOrdine.Quantita == 0) realm.Remove(rigaOrdine);

                trans.Commit();

                return true;
            }
            catch (Exception)
            {
                trans.Rollback();
                return false;
            }
        }

        internal async static Task<IEnumerable<RigaOrdine>> AggiungiPizzaInOrdine(string partition, Ordine ordine, Pizza pizza)
        {
            var realm = await Connect(partition);

            var lista = realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine && ro.Pizza==pizza);
            RigaOrdine rigaOrdine = null;
            var trans = realm.BeginWrite();
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
                        Partition = partition,
                        Ordine = ordine,
                        Pizza = pizza,
                        Quantita = 1
                    };
                    realm.Add(rigaOrdine);
                }
                trans.Commit();

                var listaPizze = realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine);
                return listaPizze;
            }
            catch(Exception)
            {
                trans.Rollback();
                return null;
            }           
        }

        internal async static Task<IEnumerable<RigaOrdine>> ListaPizzeOrdine(string partition, Ordine ordine)
        {
            var realm = await Connect(partition);

            var lista = realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine);
            return lista;
        }

        internal async static Task<IEnumerable<Pizza>> ListaPizze(string partition)
        {
            var realm = await Connect(partition);

            var lista = realm.All<Pizza>().OrderBy(p => p.Prezzo).ThenBy(p=>p.Nome);
            return lista;
        }

        internal async static Task<IEnumerable<Pizzeria>> ListaPizzerie()
        {
            var realm = await Connect(Settings.PUBLIC_PARTITION);

            var lista = realm.All<Pizzeria>().OrderBy(p => p.Partition);
            return lista;
        }

        internal async static Task<bool> Login(string username, string password)
        {
            var realm = await Connect(Settings.PUBLIC_PARTITION);

            var lista = realm.All<Utente>().Where(u => u.Username == username && u.Password == password);
            return lista.Any();
        }

        internal async static Task<IEnumerable<RigaOrdine>> ListaOrdiniDaEvadere(string partition)
        {
            var realm = await Connect(partition);

            var listaOrdini = realm.All<Ordine>().Where(o => o.Confermato && !o.Chiuso).OrderBy(o => o.Data);

            var listaRigheOrdine = new List<RigaOrdine>();

            foreach(Ordine ordine in listaOrdini)
            {
                listaRigheOrdine = listaRigheOrdine.Union(realm.All<RigaOrdine>().Where(ro => ro.Ordine == ordine).ToList()).ToList();
            }

            return listaRigheOrdine;
        }

    }
}

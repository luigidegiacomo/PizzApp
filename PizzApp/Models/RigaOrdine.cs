 using System;
using Realms;

namespace PizzApp.Models
{
    public class RigaOrdine : RealmObject
    {
        [Required]
        [MapTo("_partition")]
        public string Partition { get; set; }

        [PrimaryKey]
        [MapTo("_id")]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public Ordine Ordine { get; set; } 

        public Pizza Pizza { get; set; }

        public int Quantita { get; set; }

        public double Prezzo { get { return Pizza.Prezzo * Quantita; ; } }

        public string PrezzoEuro { get { return String.Format("{0:C}", Prezzo); } }

        //Gestione override per APP
        protected override void OnPropertyChanged(string propertyName)
        {

            try
            {
                base.OnPropertyChanged(propertyName);

                if (propertyName.Equals(nameof(Quantita)))
                {
                    RaisePropertyChanged(nameof(Prezzo));
                    RaisePropertyChanged(nameof(PrezzoEuro));
                }

            }
            catch (Exception) { }

        }


    }
}
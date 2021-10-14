 using System;
using PizzApp.Utils;
using Realms;

namespace PizzApp.Models
{
    public class Pizza : RealmObject
    {
        [Required]
        [MapTo("_partition")]
        public string Partition { get; set; }

        [PrimaryKey]
        [MapTo("_id")]
        public string ID { get; set; } 

        public string Nome { get; set; }

        public string Ingredienti { get; set; }

        public double Prezzo { get; set; }

        public string PathImg { get; set; } 

        public string PrezzoEuro
        {
            get
            {
                return Prezzo.ToString("€ #.#0");

            }
        }

    }
}
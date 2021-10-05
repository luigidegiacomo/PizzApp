 using System;
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
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string Nome { get; set; }

        public string Ingredienti { get; set; }

        public double Prezzo { get; set; }

        public string PrezzoEuro
        {
            get
            {
                return Prezzo.ToString("€ #.#0");

            }
        }

    }
}
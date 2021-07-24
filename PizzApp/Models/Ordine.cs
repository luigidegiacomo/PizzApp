using System;
using Realms;

namespace PizzApp.Models
{
    public class Ordine : RealmObject
    {
        [Required]
        [MapTo("_partition")]
        public string Partition { get; set; }

        [PrimaryKey]
        [MapTo("_id")]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public DateTimeOffset Data { get; set; } = DateTimeOffset.Now;

        public Pizzeria Pizzeria { get; set; }

        public Utente Utente { get; set; }

        public bool Confermato { get; set; }

        public bool Chiuso { get; set; }

    }
}
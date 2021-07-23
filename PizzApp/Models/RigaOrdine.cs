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

    }
}
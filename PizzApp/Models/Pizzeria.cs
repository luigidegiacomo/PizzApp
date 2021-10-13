 using System;
using PizzApp.Utils;
using Realms;

namespace PizzApp.Models
{
    public class Pizzeria : RealmObject
    {
        [Required]
        [MapTo("_partition")]
        public string Partition { get; set; }

        [PrimaryKey]
        [MapTo("_id")]
        public string ID { get; set; } 

        public string Nome { get; set; }
        public string Indirizzo { get; set; }

        public string PathImg { get; set; }       

    }
}
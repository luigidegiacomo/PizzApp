 using System;
using Realms;

namespace PizzApp.Models
{
    public class Utente : RealmObject
    {
        [Required]
        [MapTo("_partition")]
        public string Partition { get; set; }

        [PrimaryKey]
        [MapTo("_id")]
        public string ID { get; set; } = Guid.NewGuid().ToString();

        public string Username { get; set; } 

        public string Password { get; set; }   


    }
}
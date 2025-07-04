﻿using GestBurOrdAPI.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace GestBurOrdAPI.Models
{
    public class NewUser
    {
        public int Id { get; set; }

        [Required]
        public string userName { get; set; }
        [Required] 
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }

        public List<Demande> Demandes { get; set; }

    }
}

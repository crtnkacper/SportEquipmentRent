using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SportEquipmentRent.Models
{
    public class Renting
    {
        public int Id { get; set; }
        [Display(Name = "Początek") ]
        public DateTime Start { get; set; } = DateTime.Now;
        [Display(Name = "Koniec")]
        public DateTime End { get; set; }
        [Display(Name = "Sprzęt sportowy")]

        public int SportEquipmentId { get; set; }
        [Display(Name = "Sprzęt sportowy")]

        public SportEquipment? SportEquipment { get; set; }
        public string? UserId { get; set; }
        [Display(Name = "Użytkownik")]

        public IdentityUser? User { get; set; }
    }
}

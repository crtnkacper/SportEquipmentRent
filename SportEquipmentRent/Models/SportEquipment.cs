using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportEquipmentRent.Models
{
    public class SportEquipment
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa")]
        [StringLength(200, ErrorMessage = "Nazwa musi mieć maksymalnie 200 znaków")]
        public string Name { get; set; }
        [Display(Name = "Liczba wszystkich sztuk")]

        [Range(1, 1000, ErrorMessage = "Liczba sztuk musi być pomiędzy 1 a 1000")]
        public int Units { get; set; }
        public ICollection<Renting> Rentings { get; set; } = new List<Renting>();
        [Display(Name = "Liczba dostępnycgh sztuk")]
        public int AvaibleUnits()
        {
            int currentRents = Rentings.Where(x => x.End.Date >= DateTime.UtcNow.Date).Count();

            return Units - currentRents;
        }

    }
}

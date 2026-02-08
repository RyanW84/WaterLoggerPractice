using System.ComponentModel.DataAnnotations;

namespace WaterLogger.Ryanw84.Models
{
    public class DrinkingWaterModel
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Value for {0} must be a positive.")]
        public float Quantity { get; set; }

        [Required]
        public string? Measure { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace VelocityNET.Presentation.Hydrogen.WidgetGallery.Widgets.Models {

    public class NewWidgetModel {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Description { get; set; }

        public bool AreDimensionsKnown { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }

        [Range(1, 100)]
        public int? Height { get; set; }

        [Range(1, 100)]
        public int? Length { get; set; }
    }
}
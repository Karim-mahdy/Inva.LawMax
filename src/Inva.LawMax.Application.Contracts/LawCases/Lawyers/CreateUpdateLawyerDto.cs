using System.ComponentModel.DataAnnotations;

namespace Inva.LawMax.LawCases.Lawyers
{
    public class CreateUpdateLawyerDto
    {
        [Required]
        [StringLength(128)]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        public string Position { get; set; }

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; }

        [StringLength(256)]
        public string Address { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Inva.LawMax.LawCases.Hearings
{
    public class CreateUpdateHearingDto
    {
        [Required]
        public DateTime Date { get; set; }

        [StringLength(256)]
        public string Decision { get; set; }

        [Required]
        public int CaseId { get; set; }
    }
}

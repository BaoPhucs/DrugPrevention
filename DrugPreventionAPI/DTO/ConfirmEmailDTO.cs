using System.ComponentModel.DataAnnotations;

namespace DrugPreventionAPI.DTO
{
    public class ConfirmEmailDTO
    {
        [Required]
        public string OobCode { get; set; }
    }
}

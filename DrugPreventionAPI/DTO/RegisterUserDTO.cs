using System.ComponentModel.DataAnnotations;

namespace DrugPreventionAPI.DTO
{
    public class RegisterUserDTO
    {

        public string Name { get; set; }

        [Required]
        public DateOnly Dob { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Lấy ngày hôm nay dưới dạng DateOnly
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            // Tính tuổi
            var age = today.Year - Dob.Year
                      - (Dob > today.AddYears(-(today.Year - Dob.Year)) ? 1 : 0);

            if (age < 13 || age > 120)
            {
                yield return new ValidationResult(
                    "Tuổi phải từ 13 đến 120.",
                    new[] { nameof(Dob) }
                );
            }
        }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string AgeGroup { get; set; }

    }
}

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

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string Phone { get; set; }

        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string Password { get; set; }

        public string AgeGroup { get; set; }

    }
}

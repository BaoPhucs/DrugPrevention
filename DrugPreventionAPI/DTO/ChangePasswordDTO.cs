namespace DrugPreventionAPI.DTO
{
    public class ChangePasswordDTO
    {
        public string Email { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        // Optional: You can add validation attributes if needed
        // [Required]
        // [EmailAddress]
        // public string Email { get; set; }
        // [Required]
        // public string OldPassword { get; set; }
        // [Required]
        // public string NewPassword { get; set; }
        // [Required]
        // [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        // public string ConfirmNewPassword { get; set; }
    }
}

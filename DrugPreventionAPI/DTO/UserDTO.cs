namespace DrugPreventionAPI.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateOnly? Dob { get; set; }

        public string? Phone { get; set; }

        public string Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public string? AgeGroup { get; set; }

        public string? ProfileData { get; set; }

        public bool? EmailVerified { get; set; }

        public int NoShowTotal { get; set; }

        public DateTime? CreatedDate { get; set; }

    }
}

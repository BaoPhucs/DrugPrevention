namespace DrugPreventionAPI.DTO
{
    public class RegisterUserDTO
    {

        public string Name { get; set; }

        public DateOnly Dob { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

    }
}

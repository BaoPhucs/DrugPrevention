namespace DrugPreventionAPI.DTO
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string QuestionText { get; set; } = null!;
        public string Level { get; set; } = null!;
        public List<OptionDTO> Options { get; set; } = new();
    }
}

namespace DrugPreventionAPI.DTO
{
    public class CreateActivityCommentDTO
    {
        public int ActivityId { get; set; } 
        public string Content { get; set; } = null!;
    }
}

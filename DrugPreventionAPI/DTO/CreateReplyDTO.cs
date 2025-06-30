namespace DrugPreventionAPI.DTO
{
    public class CreateReplyDTO
    {
        public int ParentCommentId { get; set; }
        public string Content { get; set; }
    }
}

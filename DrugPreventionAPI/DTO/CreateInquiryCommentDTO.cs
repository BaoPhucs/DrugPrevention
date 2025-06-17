namespace DrugPreventionAPI.DTO
{
    public class CreateInquiryCommentDTO
    {
        public int InquiryID { get; set; }
        public int CommentByID { get; set; }
        public string CommentType { get; set; }
        public string CommentText { get; set; }
        public string? AttachmentURL { get; set; }
        public string? FileName { get; set; }
        public string? AttachmentType { get; set; }
    }
}

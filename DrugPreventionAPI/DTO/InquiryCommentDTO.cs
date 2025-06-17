namespace DrugPreventionAPI.DTO
{
    public class InquiryCommentDTO
    {
        public int ID { get; set; }
        public int InquiryID { get; set; }
        public int CommentByID { get; set; }
        public string CommentType { get; set; }
        public string CommentText { get; set; }
        public string? AttachmentURL { get; set; }
        public string? FileName { get; set; }
        public string? AttachmentType { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserDTO? CommentBy { get; set; }
    }
}

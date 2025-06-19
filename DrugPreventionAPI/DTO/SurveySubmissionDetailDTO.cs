namespace DrugPreventionAPI.DTO
{
    public class SurveySubmissionDetailDTO : SurveySubmissionReadDTO
    {
        public List<SurveyAnswerDTO> Answers { get; set; } = new List<SurveyAnswerDTO>();
    }
}

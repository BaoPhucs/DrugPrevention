using AutoMapper;
using DrugPreventionAPI.Data;
using DrugPreventionAPI.DTO;
using DrugPreventionAPI.Models;

namespace DrugPreventionAPI.Interfaces
{
    public interface ICommentRepo
    {
        //interface of CommentRepo



         Task<Comment> AddAsync(CreateCommentDTO dto);
         Task<Comment> UpdateAsync(int id, UpdateCommentDTO dto);
        Task DeleteAsync(int id);
        Task<Comment?> GetByIdAsync(int id);
        Task<IEnumerable<Comment>> GetRepliesAsync(int parentId);
        Task<IEnumerable<Comment>> GetAllByPostIdAsync(int blogPostId);
        Task<IEnumerable<Comment>> GetAllByActivityIdAsync(int activityId);
        Task<IEnumerable<Comment>> GetAllAsync();
        Task<Comment> AddReplyAsync(CreateReplyDTO dto, int memberId);

    }
}

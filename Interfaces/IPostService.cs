using APISocMed.Models.DTOs;
using APISocMed.Models;

namespace APISocMed.Interfaces
{
    public interface IPostService
    {
       Task<IEnumerable<Post>> GetAllPostsAsync();
       Task<(bool IsSuccess, string Message, Post CreatedPost)> CreatePostAsync(PostDTO postDto, int userId);
       Task<IEnumerable<Post>> GetFeedAsync(int userId);
       Task DeletePostAsync(int postId, int userId);
    }
}

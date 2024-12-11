using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Interfaces
{
    public interface IPostRepository
    {
        Task<(bool isSuccess, string message, Post? createdPost)> CreatePost(PostDTO content, int userId);

        Task<List<Post>> GetAllPosts();

    }
}

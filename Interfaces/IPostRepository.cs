using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Interfaces
{
    public interface IPostRepository
    {
        Task<(bool isSuccess, string message, Post? createdPost)> CreatePost(PostDTO content, int userId);

        Task<List<Post>> GetAllPosts();

        Task<List<Post>> GetPostsByUserIdAsync(int userId);

        Task<List<Post>> GetPostsFromFollowedUsers(int userId);

        Task<Post?> GetPostByIdAsync(int id);

        Task<(bool isSuccess, string message)> DeletePostAsync(int postId, int userId);

    }
}

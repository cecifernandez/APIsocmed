using APISocMed.Interfaces;
using APISocMed.Models.DTOs;
using APISocMed.Models;

namespace APISocMed.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _postRepository.GetAllPosts();
        }

        public async Task<(bool IsSuccess, string Message, Post CreatedPost)> CreatePostAsync(PostDTO postDto, int userId)
        {
            return await _postRepository.CreatePost(postDto, userId);
        }

        public async Task<IEnumerable<Post>> GetFeedAsync(int userId)
        {
            return await _postRepository.GetPostsFromFollowedUsers(userId);
        }

        public async Task DeletePostAsync(int postId, int userId)
        {
            await _postRepository.DeletePostAsync(postId, userId);
        }
    }
}

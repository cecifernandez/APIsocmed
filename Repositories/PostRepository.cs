using APISocMed.Data;
using APISocMed.Interfaces;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace APISocMed.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        public PostRepository(SocMedBdContext socMedBdContext)
        {
            _socMedBdContext = socMedBdContext;
        }

        public async Task<(bool isSuccess, string message, Post? createdPost)> CreatePost(PostDTO postDto, int userId)
        {
            if (string.IsNullOrEmpty(postDto.Content))
                return (false, "Content cannot be empty.", null);

            var post = new Post
            {
                Content = postDto.Content,
                UserId = userId,
            };

            await _socMedBdContext.Posts.AddAsync(post);
            var result = await _socMedBdContext.SaveChangesAsync();

            if (result > 0)
                return (true, "Post created successfully.", post);

            return (false, "Post creation failed.", null);
        }

        public async Task<List<Post>> GetAllPosts()
        {
            return await _socMedBdContext.Posts.ToListAsync();
        }
    }
}

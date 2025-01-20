using APISocMed.Data;
using APISocMed.Interfaces;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace APISocMed.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        private readonly IMapper _mapper;
        public PostRepository(SocMedBdContext socMedBdContext, IMapper mapper)
        {
            _socMedBdContext = socMedBdContext;
            _mapper = mapper;
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

        public async Task<List<Post>> GetPostsByUserIdAsync(int userId)
        {
            return await _socMedBdContext.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public Task<List<Post>> GetPostsFromFollowedUsers(int userId)
        {
            return _socMedBdContext.Posts
                .Where(p => _socMedBdContext.Followers.Any(f => f.FollowerId == userId && f.FollowedId == p.UserId))
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<Post?> GetPostByIdAsync(int id)
        {
            return await _socMedBdContext.Posts
                .FirstOrDefaultAsync(u => u.PostId == id);
        }

        public async Task<(bool isSuccess, string message)> DeletePostAsync(int postId, int userId)
        {
            var post = await _socMedBdContext.Posts.FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == userId);

            if (post == null)
                return (false, "Post not found or you are not authorized to delete this post.");

            _socMedBdContext.Posts.Remove(post);
            var result = await _socMedBdContext.SaveChangesAsync();

            if (result > 0)
                return (true, "Post deleted successfully.");

            return (false, "Failed to delete post.");
        }
    }
}

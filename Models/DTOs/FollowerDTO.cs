namespace APISocMed.Models.DTOs
{
    public class FollowerDTO
    {
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }

        public User FollowerUser { get; set; }
        public User FollowedUser { get; set; }
    }
}

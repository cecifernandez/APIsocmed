using System.ComponentModel.DataAnnotations;

namespace APISocMed.Models
{
    public class Follower
    {
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }

        public User FollowerUser { get; set; }
        public User FollowedUser { get; set; }
    }
}

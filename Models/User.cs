using System;
using System.Collections.Generic;

namespace APISocMed.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string UserEmail { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Follower> Followers { get; set; } 
    public ICollection<Follower> FollowedBy { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

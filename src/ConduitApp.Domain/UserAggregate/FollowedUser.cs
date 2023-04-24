namespace ConduitApp.Domain.UserAggregate;

public class FollowedUser : Entity
{
    public FollowedUser(
        int userId,
        int followedUserId)
    {
        UserId = userId;
        FollowedUserId = followedUserId;
    }

    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public int FollowedUserId { get; set; }

    public User FollowerUser { get; set; } = null!;
}

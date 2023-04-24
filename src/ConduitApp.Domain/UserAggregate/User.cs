namespace ConduitApp.Domain.UserAggregate;

public class User : Entity, IAggregateRoot
{
    public User()
    {
    }

    public User(
        string userName,
        string email,
        string bio,
        byte[] hash,
        byte[] salt)
    {
        UserName = userName;
        Email = email;
        Bio = bio;
        Hash = hash;
        Salt = salt;
    }

    public string UserName { get; }

    public string Email { get; set; } = default!;

    public string Bio { get; set; } = string.Empty;

    public string Image { get; set; } = string.Empty;

    public byte[] Hash { get; set; } = Array.Empty<byte>();

    public byte[] Salt { get; set; } = Array.Empty<byte>();

    public ICollection<FollowedUser> Followers { get; set; } = new List<FollowedUser>();

    public ICollection<FollowedUser> FollowedUsers { get; set; } = new List<FollowedUser>();
}

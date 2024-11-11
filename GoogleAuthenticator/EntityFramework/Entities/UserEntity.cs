namespace GoogleAuthenticator.EntityFramework.Entities;

public sealed record UserEntity
{
    public Guid Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
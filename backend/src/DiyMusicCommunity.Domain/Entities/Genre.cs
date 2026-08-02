namespace DiyMusicCommunity.Domain.Entities;

public sealed class Genre : Entity
{
    public string Name { get; private set; }

    public Genre(Guid id, string name) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Genre name cannot be empty.", nameof(name));
        }

        Name = name;
    }
}

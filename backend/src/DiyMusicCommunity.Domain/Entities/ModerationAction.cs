namespace DiyMusicCommunity.Domain.Entities;

public sealed class ModerationAction : Entity
{
    public Guid ModeratorId { get; private set; }
    public string ActionType { get; private set; }
    public Guid TargetId { get; private set; }
    public string Reason { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public ModerationAction(
        Guid id,
        Guid moderatorId,
        string actionType,
        Guid targetId,
        string reason,
        DateTime createdAt)
        : base(id)
    {
        if (moderatorId == Guid.Empty)
        {
            throw new ArgumentException("ModeratorId cannot be empty.", nameof(moderatorId));
        }
        if (string.IsNullOrWhiteSpace(actionType))
        {
            throw new ArgumentException("ActionType cannot be empty.", nameof(actionType));
        }
        if (targetId == Guid.Empty)
        {
            throw new ArgumentException("TargetId cannot be empty.", nameof(targetId));
        }
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason cannot be empty.", nameof(reason));
        }

        ModeratorId = moderatorId;
        ActionType = actionType;
        TargetId = targetId;
        Reason = reason;
        CreatedAt = createdAt;
    }
}

namespace DiyMusicCommunity.Domain.Exceptions;

public sealed class InvalidProposalTransitionException : DomainException
{
    public InvalidProposalTransitionException(string message) : base(message) { }
}

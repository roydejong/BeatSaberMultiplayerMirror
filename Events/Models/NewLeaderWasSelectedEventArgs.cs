namespace MultiplayerMirror.Events.Models
{
    public class NewLeaderWasSelectedEventArgs
    {
        public readonly MultiplayerGameplayAnimator MultiplayerGameplayAnimator;
        public readonly string UserId;

        public NewLeaderWasSelectedEventArgs(MultiplayerGameplayAnimator multiplayerGameplayAnimator, string userId)
        {
            MultiplayerGameplayAnimator = multiplayerGameplayAnimator;
            UserId = userId;
        }

        public override string ToString()
        {
            return $"NewLeaderWasSelectedEventArgs (UserId={UserId})";
        }
    }
}
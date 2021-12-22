﻿namespace MultiplayerMirror.Events.Models
{
    public class FirstPlayerDidChangeEventArgs
    {
        public readonly MultiplayerLeadPlayerProvider LeadPlayerProvider;
        public readonly MultiplayerScoreProvider.RankedPlayer? FirstPlayer;

        public FirstPlayerDidChangeEventArgs(MultiplayerLeadPlayerProvider leadPlayerProvider,
            MultiplayerScoreProvider.RankedPlayer? firstPlayer)
        {
            LeadPlayerProvider = leadPlayerProvider;
            FirstPlayer = firstPlayer;
        }

        public override string ToString()
        {
            return FirstPlayer is null
                ? "FirstPlayerDidChangeEventArgs (firstPlayer=NULL)"
                : $"FirstPlayerDidChangeEventArgs (userId={FirstPlayer.userId}, userName={FirstPlayer.userName})";
        }
    }
}
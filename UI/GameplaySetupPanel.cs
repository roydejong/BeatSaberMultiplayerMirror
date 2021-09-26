using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;

namespace MultiplayerMirror.UI
{
    public class GameplaySetupPanel : NotifiableSingleton<GameplaySetupPanel>
    {
        #region Values
        [UIValue("EnableLobbyMirror")]
        public bool EnableLobbyMirror
        {
            get => Plugin.Config!.EnableLobbyMirror;
            set
            {
                Plugin.Config!.EnableLobbyMirror = value;
                NotifyPropertyChanged();
            }
        } 
        
        [UIValue("EnableSelfHologram")]
        public bool EnableSelfHologram
        {
            get => Plugin.Config!.EnableSelfHologram;
            set
            {
                Plugin.Config!.EnableSelfHologram = value;
                NotifyPropertyChanged();
            }
        } 
        
        [UIValue("ForceSelfHologram")]
        public bool ForceSelfHologram
        {
            get => Plugin.Config!.ForceSelfHologram;
            set
            {
                Plugin.Config!.ForceSelfHologram = value;
                NotifyPropertyChanged();
            }
        } 
        #endregion

        #region Actions
        [UIAction("SetEnableLobbyMirror")]
        public void SetEnableLobbyMirror(bool value)
        {
            EnableLobbyMirror = value;   
        }

        [UIAction("SetEnableSelfHologram")]
        public void SetEnableSelfHologram(bool value)
        {
            if (value)
            {
                EnableSelfHologram = true;    
            }
            else
            {
                EnableSelfHologram = false;
                ForceSelfHologram = false;   
            }
        }

        [UIAction("SetForceSelfHologram")]
        public void SetForceSelfHologram(bool value)
        {
            if (value)
            {
                EnableSelfHologram = true;
                ForceSelfHologram = true;   
            }
            else
            {
                ForceSelfHologram = false;   
            }
        }
        #endregion
    }
}
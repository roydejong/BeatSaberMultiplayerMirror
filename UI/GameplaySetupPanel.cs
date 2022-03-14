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
                Plugin.Config.TriggerChangeEvent(this);
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
                Plugin.Config.TriggerChangeEvent(this);
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
                Plugin.Config.TriggerChangeEvent(this);
            }
        } 
        
        [UIValue("EnableDuelHologram")]
        public bool EnableDuelHologram
        {
            get => Plugin.Config!.EnableDuelHologram;
            set
            {
                Plugin.Config!.EnableDuelHologram = value;
                NotifyPropertyChanged();
                Plugin.Config.TriggerChangeEvent(this);
            }
        } 
        
        [UIValue("InvertMirror")]
        public bool InvertMirror
        {
            get => Plugin.Config!.InvertMirror;
            set
            {
                Plugin.Config!.InvertMirror = value;
                NotifyPropertyChanged();
                Plugin.Config.TriggerChangeEvent(this);
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

        [UIAction("SetEnableDuelHologram")]
        public void SetEnableDuelHologram(bool value)
        {
            if (value)
            {
                EnableSelfHologram = true;
                EnableDuelHologram = true;   
            }
            else
            {
                EnableDuelHologram = false;   
            }
        }

        [UIAction("SetInvertMirror")]
        public void SetInvertMirror(bool value)
        {
            InvertMirror = value;
        }
        #endregion
    }
}
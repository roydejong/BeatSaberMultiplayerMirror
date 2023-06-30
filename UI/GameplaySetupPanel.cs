using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Util;

namespace MultiplayerMirror.UI
{
    public class GameplaySetupPanel : NotifiableSingleton<GameplaySetupPanel>
    {
        #region Bindings - Values

        [UIValue("EnableLobbyMirror")]
        public bool EnableLobbyMirror
        {
            get => Plugin.Config!.EnableLobbyMirror;
            set
            {
                Plugin.Config!.EnableLobbyMirror = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

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
                NotifyInteractablePropertiesChanged();

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
                NotifyInteractablePropertiesChanged();

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
                NotifyInteractablePropertiesChanged();

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
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        #endregion

        #region Bindings - Interactable

        [UIValue("EnableLobbyMirrorInteractable")]
        public bool EnableLobbyMirrorInteractable => true;

        [UIValue("EnableSelfHologramInteractable")]
        public bool EnableSelfHologramInteractable => true;

        [UIValue("ForceSelfHologramInteractable")]
        public bool ForceSelfHologramInteractable => EnableSelfHologram;

        [UIValue("EnableDuelHologramInteractable")]
        public bool EnableDuelHologramInteractable => EnableSelfHologram;

        [UIValue("InvertMirrorInteractable")]
        public bool InvertMirrorInteractable => EnableLobbyMirror || EnableSelfHologram;

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
            EnableSelfHologram = value;
        }

        [UIAction("SetForceSelfHologram")]
        public void SetForceSelfHologram(bool value)
        {
            ForceSelfHologram = value;
        }

        [UIAction("SetEnableDuelHologram")]
        public void SetEnableDuelHologram(bool value)
        {
            EnableDuelHologram = value;
        }

        [UIAction("SetInvertMirror")]
        public void SetInvertMirror(bool value)
        {
            InvertMirror = value;
        }

        #endregion

        #region Utils

        private void NotifyInteractablePropertiesChanged()
        {
            NotifyPropertyChanged("EnableLobbyMirrorInteractable");
            NotifyPropertyChanged("EnableSelfHologramInteractable");
            NotifyPropertyChanged("ForceSelfHologramInteractable");
            NotifyPropertyChanged("EnableDuelHologramInteractable");
            NotifyPropertyChanged("InvertMirrorInteractable");
        }

        #endregion
    }
}
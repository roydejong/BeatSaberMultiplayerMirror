using System;
using System.Diagnostics.CodeAnalysis;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.GameplaySetup;
using JetBrains.Annotations;
using Zenject;

namespace MultiplayerMirror.UI
{
    [UsedImplicitly]
    public class GameplaySetupPanel : NotifiableBase, IInitializable, IDisposable
    {
        private const string TabName = "Multiplayer Mirror";
        private const string BsmlResource = "MultiplayerMirror.UI.BSML.GameplaySetupPanel.bsml";

        [Inject] private readonly GameplaySetup _gameplaySetup = null!;

        public void Initialize()
        {
            _gameplaySetup.AddTab(
                name: TabName,
                resource: BsmlResource,
                host: this,
                menuType: MenuType.Online
            );
        }

        public void Dispose()
        {
            _gameplaySetup.RemoveTab(TabName);
        }

        #region Bindings - Values

        [UIValue("EnableLobbyMirror")]
        [UsedImplicitly]
        public bool EnableLobbyMirror
        {
            get => Plugin.Config.EnableLobbyMirror;
            set
            {
                Plugin.Config.EnableLobbyMirror = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        [UIValue("EnableSelfHologram")]
        [UsedImplicitly]
        public bool EnableSelfHologram
        {
            get => Plugin.Config.EnableSelfHologram;
            set
            {
                Plugin.Config.EnableSelfHologram = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        [UIValue("ForceSelfHologram")]
        [UsedImplicitly]
        public bool ForceSelfHologram
        {
            get => Plugin.Config.ForceSelfHologram;
            set
            {
                Plugin.Config.ForceSelfHologram = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        [UIValue("EnableDuelHologram")]
        [UsedImplicitly]
        public bool EnableDuelHologram
        {
            get => Plugin.Config.EnableDuelHologram;
            set
            {
                Plugin.Config.EnableDuelHologram = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        [UIValue("InvertMirror")]
        [UsedImplicitly]
        public bool InvertMirror
        {
            get => Plugin.Config.InvertMirror;
            set
            {
                Plugin.Config.InvertMirror = value;

                NotifyPropertyChanged();
                NotifyInteractablePropertiesChanged();

                Plugin.Config.TriggerChangeEvent(this);
            }
        }

        #endregion

        #region Bindings - Interactable

        [UIValue("EnableLobbyMirrorInteractable")]
        [UsedImplicitly]
        public bool EnableLobbyMirrorInteractable => true;

        [UIValue("EnableSelfHologramInteractable")]
        [UsedImplicitly]
        public bool EnableSelfHologramInteractable => true;

        [UIValue("ForceSelfHologramInteractable")]
        [UsedImplicitly]
        public bool ForceSelfHologramInteractable => EnableSelfHologram;

        [UIValue("EnableDuelHologramInteractable")]
        [UsedImplicitly]
        public bool EnableDuelHologramInteractable => EnableSelfHologram;

        [UIValue("InvertMirrorInteractable")]
        [UsedImplicitly]
        public bool InvertMirrorInteractable => EnableLobbyMirror || EnableSelfHologram;

        #endregion

        #region Actions

        [UIAction("SetEnableLobbyMirror")]
        [UsedImplicitly]
        public void SetEnableLobbyMirror(bool value)
        {
            EnableLobbyMirror = value;
        }

        [UIAction("SetEnableSelfHologram")]
        [UsedImplicitly]
        public void SetEnableSelfHologram(bool value)
        {
            EnableSelfHologram = value;
        }

        [UIAction("SetForceSelfHologram")]
        [UsedImplicitly]
        public void SetForceSelfHologram(bool value)
        {
            ForceSelfHologram = value;
        }

        [UIAction("SetEnableDuelHologram")]
        [UsedImplicitly]
        public void SetEnableDuelHologram(bool value)
        {
            EnableDuelHologram = value;
        }

        [UIAction("SetInvertMirror")]
        [UsedImplicitly]
        public void SetInvertMirror(bool value)
        {
            InvertMirror = value;
        }

        #endregion

        #region Utils

        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
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
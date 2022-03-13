using SiraUtil.Logging;
using Zenject;

namespace MultiplayerMirror.Core
{
    public class HologramMirror : IInitializable
    {
        [Inject] private SiraLog _log = null!;
        [Inject] private PluginConfig _config = null!;
        
        public void Initialize()
        {
            _log.Info("HOLOGRAM MIRROR INIT");
        }
    }
}
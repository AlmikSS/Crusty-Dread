using Core.Audio;
using Core.EventSystem;
using Core.Input;
using Core.TicksSystem;
using Tools.DevConsole;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.EntryPoints
{
    public class GamePlayBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSystem _tickSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private AudioSystem _audioSystem;
        
        private EventBus _eventBus;
        
        private void Awake()
        {
            _tickSystem.Construct();
            _inputSystem.Construct();
            _audioSystem.Construct();
            _eventBus = new EventBus();
            
            CommandsRegistry.RegisterAllCommands();
            _tickSystem.StartTicks();
        }

        private void OnDestroy()
        {
            _eventBus.Dispose();
        }

        [Command("enable_debug_mode", "Enable global debug hud with input and player input, jump")]
        private void EnableDebugMode(bool enable)
        {
            var en = enable ? "true" : "false";
            CommandExecutor.Execute("toggle_debug_hud " + en);
            CommandExecutor.Execute("show_input_info " + en);
            CommandExecutor.Execute("show_player_info " + en);
            CommandExecutor.Execute("set_jump_enable " + en);
        }
    }
}
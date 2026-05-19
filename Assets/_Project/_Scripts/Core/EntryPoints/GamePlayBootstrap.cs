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
        
        private void Awake()
        {
            _tickSystem.Construct();
            _inputSystem.Construct();
            
            CommandsRegistry.RegisterAllCommands();
            
            _tickSystem.StartTicks();
        }

        [Command("restart", "Reload active scene")]
        private void Restart()
        {
            Debug.Log("Restart");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
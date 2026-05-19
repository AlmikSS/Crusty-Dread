using Core.Input;
using Core.TicksSystem;
using Tools.DevConsole;
using UnityEngine;

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
    }
}
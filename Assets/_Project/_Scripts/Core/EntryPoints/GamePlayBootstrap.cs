using Core.Input;
using Core.TicksSystem;
using UnityEngine;

namespace Core.EntryPoints
{
    public class GamePlayBootstrap : MonoBehaviour
    {
        [SerializeField] private TickSystem _tickSystem;
        [SerializeField] private InputSystem _inputSystem;
        
        private void Start()
        {
            _tickSystem.Construct();
            _inputSystem.Construct();
            
            _tickSystem.StartTicks();
        }
    }
}
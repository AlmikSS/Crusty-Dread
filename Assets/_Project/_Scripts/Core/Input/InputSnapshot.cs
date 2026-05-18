using UnityEngine;

namespace Core.Input
{
    public struct InputSnapshot
    {
        public readonly Vector2 MoveInput;
        public readonly Vector2 LookInput;
        public readonly bool UseInput;

        public InputSnapshot(Vector2 moveInput, Vector2 lookInput, bool useInput)
        {
            MoveInput = moveInput;
            LookInput = lookInput;
            UseInput = useInput;
        }
    }
}
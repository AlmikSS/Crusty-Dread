using UnityEngine;

namespace Core.Input
{
    public struct InputSnapshot
    {
        public readonly InputContext Context;
        public readonly Vector2 MoveInput;
        public readonly Vector2 LookInput;
        public readonly bool UseInput;
        public readonly bool OpenConsole;

        public InputSnapshot(InputContext context, Vector2 moveInput, Vector2 lookInput, bool useInput, bool openConsole)
        {
            Context = context;
            MoveInput = moveInput;
            LookInput = lookInput;
            UseInput = useInput;
            OpenConsole = openConsole;
        }
    }
}
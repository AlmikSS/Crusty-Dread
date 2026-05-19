using System;

namespace Tools.DevConsole
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        
        public CommandAttribute(string name, string description)
        {
            Name = name.ToLower();
            Description = description;
        }
    }
}
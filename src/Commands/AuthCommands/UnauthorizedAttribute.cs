using System;

namespace FocusMark.App.Cli.Commands.AuthCommands
{
    /// <summary>
    /// Denotes that a Command can be used without authorization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class UnauthorizedAttribute : Attribute
    {
    }
}

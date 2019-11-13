using System;

namespace PlantUml.Net.InputModes
{
    public interface IInputResult : IDisposable
    {
        string Input { get; }
        string Argument { get; }
    }
}

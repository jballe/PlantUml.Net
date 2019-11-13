using System;
using System.IO;

namespace PlantUml.Net.InputModes
{
    internal class InputFactory
    {
        readonly InputMode _mode;
        private readonly string _workingDir;

        internal InputFactory(InputMode mode, string workingDir = null)
        {
            _mode = mode;
            _workingDir = workingDir;
        }

        internal IInputResult Create(string code)
        {
            switch (_mode)
            {
                case InputMode.Pipe: return new PipeInputResult(code);
                case InputMode.TempFile: return new FileInputResult(Path.GetTempFileName(), code);
                case InputMode.WorkingDirFile: return new FileInputResult(Path.Combine(_workingDir, Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".puml"), code);
                default:
                    throw new InvalidOperationException("Unknown InputMode " + _mode);
            }
        }
    }
}

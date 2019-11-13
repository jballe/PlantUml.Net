namespace PlantUml.Net.InputModes
{
    internal class PipeInputResult : IInputResult
    {
        public PipeInputResult(string code)
        {
            Input = code;
            Argument = "-pipe";
        }

        public void Dispose()
        {
        }

        public string Input { get; }
        public string Argument { get; }
    }
}

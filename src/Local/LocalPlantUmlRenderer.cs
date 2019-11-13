using System;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using PlantUml.Net.Remote;
using static System.Text.Encoding;

namespace PlantUml.Net.Local
{
    internal class LocalPlantUmlRenderer : IPlantUmlRenderer
    {
        private readonly JarRunner jarRunner;
        private readonly LocalCommandProvider commandProvider;
        private readonly RenderUrlCalculator renderUrlCalculator;
        private readonly InputFactory inputFactory;

        public LocalPlantUmlRenderer(JarRunner jarRunner, LocalCommandProvider commandProvider,
            RenderUrlCalculator renderUrlCalculator, InputFactory inputFactory)
        {
            this.jarRunner = jarRunner;
            this.commandProvider = commandProvider;
            this.renderUrlCalculator = renderUrlCalculator;
            this.inputFactory = inputFactory;
        }

        public byte[] Render(string code, OutputFormat outputFormat)
        {
            using (var input = inputFactory.Create(code))
            {
                string command = commandProvider.GetCommand(outputFormat);
                var processResult = jarRunner.RunJarWithInput(input.Input, command, input.Argument);

                if (processResult.ExitCode != 0)
                {
                    string message = UTF8.GetString(processResult.Error);
                    throw new RenderingException(code, message);
                }

                return processResult.Output;
            }
        }

        public Uri RenderAsUri(string code, OutputFormat outputFormat)
        {
            string renderUri = renderUrlCalculator.GetRenderUrl(code, outputFormat);
            return new Uri(renderUri);
        }
    }
}

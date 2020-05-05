using System;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using static System.Text.Encoding;

namespace PlantUml.Net.Local
{
    internal class LocalPlantUmlRenderer : IPlantUmlRenderer
    {
        private readonly JarRunner jarRunner;
        private readonly LocalCommandProvider commandProvider;
        private readonly InputFactory inputFactory;

        public LocalPlantUmlRenderer(JarRunner jarRunner, LocalCommandProvider commandProvider, InputFactory inputFactory)
        {
            this.jarRunner = jarRunner;
            this.commandProvider = commandProvider;
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
            var bytes = Render(code, outputFormat);
            var base64 = System.Convert.ToBase64String(bytes);
            var url = $"data:image/{GetMimeType(outputFormat)};base64,{base64}";
            return new Uri(url);
        }

        private static string GetMimeType(OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Png:
                case OutputFormat.Svg:
                    return $"image/{format.ToString().ToLowerInvariant()}";

                case OutputFormat.Eps:
                    return "application/postscript";
                case OutputFormat.Pdf:
                    return "application/pdf";
                case OutputFormat.Vdx:
                    return "application/vnd.visio";
                case OutputFormat.Xmi:
                    return "application/xml";
                case OutputFormat.Scxml:
                    return "application/scxml+xml";
                case OutputFormat.Html:
                    return "text/html";
                case OutputFormat.Ascii:
                case OutputFormat.Ascii_Unicode:
                case OutputFormat.LaTeX:
                    return "text/plain";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}

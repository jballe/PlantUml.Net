using System;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using PlantUml.Net.Remote;

namespace PlantUml.Net.LocalEncode
{
    internal class LocalEncodePlantUmlRenderer : IPlantUmlRenderer
    {
        private readonly JarRunner _jarRunner;
        private readonly UrlFormatMap _urlFormatMap;
        private readonly InputFactory _inputFactory;
        private readonly IPlantUmlRenderer _remoteRenderer;

        internal LocalEncodePlantUmlRenderer(JarRunner jarRunner, UrlFormatMap urlFormatMap, InputFactory inputFactory, IPlantUmlRenderer remoteRenderer)
        {
            _jarRunner = jarRunner;
            _urlFormatMap = urlFormatMap;
            _inputFactory = inputFactory;
            _remoteRenderer = remoteRenderer;
        }

        public byte[] Render(string code, OutputFormat outputFormat)
        {
            var allCode = Encode(code);
            return _remoteRenderer.Render(allCode, outputFormat);
        }

        public Uri RenderAsUri(string code, OutputFormat outputFormat)
        {
            var component = Encode(code);
            var url = _urlFormatMap.GetRenderUrl(component, outputFormat);
            return new Uri(url);
        }

        private string Encode(string code)
        {
            using (var input = _inputFactory.Create(code))
            {
                var processResult = _jarRunner.RunJarWithInput(input.Input, "-computeurl", input.Argument);
                if (processResult.ExitCode != 0)
                {
                    var message = System.Text.Encoding.UTF8.GetString(processResult.Error);
                    Console.WriteLine($"WARN {message}");
                    throw new RenderingException(code, message);
                }

                var result = System.Text.Encoding.UTF8.GetString(processResult.Output);
                return result;
            }
        }
    }
}

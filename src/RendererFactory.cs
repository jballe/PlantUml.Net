using System;
using PlantUml.Net.InputModes;
using PlantUml.Net.Java;
using PlantUml.Net.Local;
using PlantUml.Net.LocalEncode;
using PlantUml.Net.Remote;

namespace PlantUml.Net
{
    public class RendererFactory
    {
        public IPlantUmlRenderer CreateRenderer()
        {
            return CreateRenderer(new PlantUmlSettings());
        }

        public IPlantUmlRenderer CreateRenderer(PlantUmlSettings settings, string workingDir = null)
        {
            return CreateRenderer(settings, workingDir, settings.RenderingMode);
        }

        private IPlantUmlRenderer CreateRenderer(PlantUmlSettings settings, string workingDir, RenderingMode mode)
        {
            UrlFormatMap urlFormatMap = new UrlFormatMap(settings.RemoteUrl);
            RenderUrlCalculator renderUrlCalculator = new RenderUrlCalculator(urlFormatMap);

            switch (mode)
            {
                case RenderingMode.Remote:

                    return new RemotePlantUmlRenderer(renderUrlCalculator);

                case RenderingMode.Local:

                    JarRunner jarRunner = CreateJarRunner(settings);
                    LocalCommandProvider commandProvider = new LocalCommandProvider(settings.LocalGraphvizDotPath);
                    return new LocalPlantUmlRenderer(jarRunner, commandProvider, new InputFactory(settings.InputMode, workingDir));

                case RenderingMode.LocalEncode:

                    var remote = CreateRenderer(settings, workingDir, RenderingMode.Remote);
                    return new LocalEncodePlantUmlRenderer(CreateJarRunner(settings), urlFormatMap, new InputFactory(settings.InputMode, workingDir), remote);

                default:
                    throw new ArgumentException("invalid rendering mode", nameof(settings.RenderingMode));
            }
        }

        private static JarRunner CreateJarRunner(PlantUmlSettings settings)
        {
            IJavaLocator javaLocator = CreateJavaLocator(settings);
            string installationPath = javaLocator.GetJavaInstallationPath();
            return new JarRunner(installationPath, settings.LocalPlantUmlPath);
        }

        private static IJavaLocator CreateJavaLocator(PlantUmlSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.JavaPath))
            {
                return new EnvironmentJavaLocator();
            }
            else
            {
                return new ExplicitJavaLocator(settings.JavaPath);
            }
        }
    }
}

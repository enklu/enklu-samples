using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Enklu.Data;
using Enklu.Samples.Network;
using Mamba.Experience;
using Microsoft.Azure.Kinect.Sensor;
using Newtonsoft.Json;
using Serilog;

namespace Enklu.Samples
{
    public class KinectController : IDisposable
    {
        private Device _kinect;

        public KinectController()
        {

        }

        public void Start()
        {
            // forward logs
            Logger.LogMessage += Logger_OnLog;

            // open device
            _kinect = Device.Open();
            _kinect.StartCameras(new DeviceConfiguration
            {
                DepthMode = DepthMode.NFOV_2x2Binned,
                ColorResolution = ColorResolution.R1080p
            });
        }

        private void Logger_OnLog(LogMessage message)
        {
            switch (message.LogLevel)
            {
                case LogLevel.Warning:
                {
                    Log.Warning(message.FormatedMessage);
                    break;
                }
                case LogLevel.Error:
                {
                    Log.Error(message.FormatedMessage);
                    break;
                }
                case LogLevel.Critical:
                {
                    Log.Fatal(message.FormatedMessage);
                    break;
                }
                default:
                {
                    Log.Debug(message.FormatedMessage);
                    return;
                }
            }
        }

        private void ReleaseUnmanagedResources()
        {
            if (null != _kinect)
            {
                _kinect.Dispose();
                _kinect = null;
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~KinectController()
        {
            ReleaseUnmanagedResources();
        }
    }

    /// <summary>
    /// Entry point.
    /// </summary>
    internal class AzureKinectSample
    {
        /// <summary>
        /// Path to optional config.
        /// </summary>
        private const string APP_CONFIG_PATH = "app-config.json";

        /// <summary>
        /// Main.
        /// </summary>
        private static void Main()
        {
            // logging
            var log = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .MinimumLevel.Information()
                .CreateLogger();
            Log.Logger = log;
            Log.Information("Logging initialized.");
            
            var config = Configuration();

            Log.Information("Configuration: {0}", config);
            Run(config).Wait();
        }

        /// <summary>
        /// Starts the application.
        /// </summary>
        /// <param name="config">Options to run with.</param>
        /// <returns></returns>
        private static async Task Run(AzureKinectSampleConfiguration config)
        {
            using (var experience = new ExperienceController(new ExperienceControllerConfig
            {
                AppId = config.ExperienceId,
                TrellisToken = config.Token,
                TrellisUrl = config.TrellisUrl
            }))
            {
                ElementData elements;

                // load experience first
                try
                {
                    elements = await experience.Initialize();
                }
                catch (Exception exception)
                {
                    Log.Error($"Could not initialize experience: '{exception}'.");

                    return;
                }

                // connect to Mycelium next
                using (var network = new MyceliumController(new MyceliumControllerConfiguration
                {
                    Ip = config.MyceliumIp,
                    Port = config.MyceliumPort,
                    Token = config.Token
                }))
                {
                    network.Start();

                    // startup kinect
                    using (var kinect = new KinectController())
                    {
                        kinect.Start();

                        Console.ReadLine();
                    }
                }
            }

            Log.Information("Shutting down.");
        }

        /// <summary>
        /// Creates an AppActorConfiguration.
        /// </summary>
        /// <returns></returns>
        private static AzureKinectSampleConfiguration Configuration()
        {
            // construct the application config
            var config = new AzureKinectSampleConfiguration();

            // override defaults with app-config.json
            if (File.Exists(APP_CONFIG_PATH))
            {
                var src = File.ReadAllText(APP_CONFIG_PATH);
                config.Override(JsonConvert.DeserializeObject<AzureKinectSampleConfiguration>(src));
            }

            // override with environment variables
            SetFromEnvironment("EXPERIENCE_ID", ref config.ExperienceId, a => a);
            SetFromEnvironment("TRELLIS_URL", ref config.TrellisUrl, a => a);
            SetFromEnvironment("TRELLIS_TOKEN", ref config.Token, a => a);
            SetFromEnvironment("MYCELIUM_IP", ref config.MyceliumIp, a => a);
            SetFromEnvironment("MYCELIUM_PORT", ref config.MyceliumPort, int.Parse);
            
            return config;
        }

        /// <summary>
        /// Sets a value from an environment variable.
        /// </summary>
        /// <typeparam name="T">The type of prop to set.</typeparam>
        /// <param name="name">The name of the environment variable.</param>
        /// <param name="prop">A reference to the field.<param>
        /// <param name="converter">A function that converts from string to the required type.</param>
        private static void SetFromEnvironment<T>(string name, ref T prop, Func<string, T> converter)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
            {
                prop = converter(value);
            }
        }
    }
}
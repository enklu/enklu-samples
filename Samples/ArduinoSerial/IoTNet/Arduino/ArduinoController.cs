using System;
using System.Threading.Tasks;
using Serilog;

namespace ArKit
{
    public class ArduinoController : IStreamMessageListener
    {
        private readonly IStreamReader _reader;
        private bool _isAlive;

        public ArduinoController()
        {
            _reader = new DelimiterMessageReader(this);
        }

        public void OnMessage(string message)
        {
            Log.Information($"Message: {message}");

            // TODO: process
        }

        public void Stop()
        {
            _isAlive = false;
        }

        public async Task Start()
        {
            _isAlive = true;

            while (_isAlive)
            {
                try
                {
                    using (var stream = new SerialStream(_reader))
                    {
                        Console.Read();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"There was a serial stream error: '{ex}'.");

                    // wait for a second before trying to reconnect
                    await Task.Delay(1000);
                }
            }
        }
    }
}

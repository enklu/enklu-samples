using System;
using Serilog;

namespace ArKit
{
    public class DelimiterMessageReader : IStreamReader
    {
        private readonly IStreamMessageListener _listener;
        private string _buffer;

        public DelimiterMessageReader(IStreamMessageListener listener)
        {
            _listener = listener;
        }

        public void OnRead(string contents)
        {
            var next = contents;
            var index = next.IndexOf('\n');
            while (index >= 0)
            {
                _buffer += next.Substring(0, index + 1);

                try
                {
                    _listener.OnMessage(_buffer);
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not handle message: '{ex}'.");
                }

                _buffer = string.Empty;

                // restart
                next = next.Substring(index + 1);
                index = next.IndexOf('\n');
            }

            _buffer += next;
        }
    }
}

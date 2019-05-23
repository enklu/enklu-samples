using System;
using Enklu.Mycerializer.Netty;

namespace Enklu.IoTNet.Network
{
    /// <summary>
    /// Tracks network metrics.
    /// </summary>
    public class PassthroughNetworkMetricsProvider : INetworkMetricsProvider, IDisposable
    {
        /// <inheritdoc />
        public void TrackMessageSize(int size)
        {
            // 
        }

        /// <inheritdoc />
        public IDisposable TrackEncodeTime()
        {
            return this;
        }

        /// <inheritdoc />
        public IDisposable TrackDecodeTime()
        {
            return this;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // 
        }
    }
}
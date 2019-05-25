using System;
using System.IO.Ports;

namespace ArKit
{
    public class SerialStream : IDisposable
    {
        private readonly IStreamReader _reader;
        private readonly SerialPort _port;

        public SerialStream(
            IStreamReader reader,
            string name = "COM4",
            int baudRate = 9600)
        {
            _reader = reader;
            _port = new SerialPort(name)
            {
                BaudRate = baudRate,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.None
            };

            _port.DataReceived += Port_DataReceived;
            _port.Open();
        }

        public void Dispose()
        {
            _port.Close();
            _port.Dispose();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            _reader.OnRead(_port.ReadExisting());
        }
    }
}

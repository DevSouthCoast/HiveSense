using System.IO.Ports;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

namespace Gadgeteer.Modules.IngenuityMicro
{
    /// <summary>
    /// A RfPipe module for Microsoft .NET Gadgeteer
    /// </summary>
    public class RfPipe : GTM.Module
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public delegate void RfPipeReceivedHandler(string val);

        /// <summary>
        /// 
        /// </summary>
        public event RfPipeReceivedHandler DataReceived;

        private string[] _dataIn;

        private readonly SimpleSerial _rfSerial;
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public RfPipe(int socketNumber)
        {
            Socket socket = Socket.GetSocket(socketNumber, true, this, null);
            socket.EnsureTypeIsSupported(new char[] { 'U' }, null);

            _rfSerial = new SimpleSerial(socket.SerialPortName, 9600, Parity.None, 8, StopBits.One);
            _rfSerial.DataReceived += new SerialDataReceivedEventHandler(RfSerialDataReceived);
            _rfSerial.Open();
        }
        /// <summary>
        /// Send data via the Rf Pipe
        /// </summary>
        /// <param name="data">Data to send</param>
        public void SendData(string data)
        {
            _rfSerial.WriteLine(data);
        }
        void RfSerialDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _dataIn = _rfSerial.Deserialize();
            for (int index = 0; index < _dataIn.Length; index++)
            {
                DataReceived(_dataIn[index]);
            }
        }
    }
}
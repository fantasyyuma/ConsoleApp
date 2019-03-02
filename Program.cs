using System;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        private static readonly TcpClient connection = new TcpClient();
        static void Main(string[] args)
        {
            try
            {
                connection.Connect("localhost", 5555);
                NetworkStream stream = connection.GetStream();
                SendCommand(stream, "power status discharging");
                string command = string.Format("power capacity {0}", 50);
                SendCommand(stream, command);
                stream.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("The following error has occured: {0}", ex.Message);
            }
        }

        private static void ReadDataToConsole(NetworkStream stream)
        {
            var responseBytes = new byte[connection.ReceiveBufferSize];
            stream.Read(responseBytes, 0, connection.ReceiveBufferSize);
            string responseText = Encoding.ASCII.GetString(responseBytes).Trim(new[] { ' ', ' ', 'n', 'r' });
            if (!string.IsNullOrEmpty(responseText))
                Console.WriteLine("Response: '{0}'.", responseText);
        }

        private static void SendCommand(NetworkStream stream, string command)
        {
            Console.WriteLine("Sending command '{0}'.", command);
            byte[] commandBytes = Encoding.ASCII.GetBytes(command + "r");
            Buffer.BlockCopy(command.ToCharArray(), 0, commandBytes, 0, commandBytes.Length);
            stream.Write(commandBytes, 0, commandBytes.Length);
            ReadDataToConsole(stream);
        }
    }
}

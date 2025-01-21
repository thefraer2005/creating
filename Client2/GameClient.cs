using Common;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Client2
{
    public class GameClient
    {
        private Socket clientSocket;
        public event Action<byte[]> OnGameStarted;
        public event Action<byte[]> UpdateGame;
        public event Action<byte[]> onVictory;
        public event Action<byte[]> endRound;
        public event Action<string> UnoAction;

       
        public void Connect(string ipAddress)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(IPAddress.Parse(ipAddress), 12345);

            Thread receiveThread = new Thread(ReceiveMessages);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        private void ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[3024];
                    int bytesRead = clientSocket.Receive(buffer);
                   
                  

               
           

                    if (bytesRead == 0)
                    {
                        MessageBox.Show("Получен пустой пакет (0 байт). Завершение работы.");
                        break;
                    }

                    var packet = Packet.FromBytes(buffer.Take(bytesRead).ToArray());

                  
                    ProcessMessage(packet);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получеия: {ex.Message}");
                    break;
                }
            }
        }

       



        private void ProcessMessage(Packet packet)
        {

            switch (packet.Command)
            {

                case Protocol.StartGame:
                
                    OnGameStarted?.Invoke(packet.Data);
                    break;
                case Protocol.UpdateGameState:
                    UpdateGame?.Invoke(packet.Data);
                    break;
                case Protocol.UNO:
                    UnoAction?.Invoke(packet.Command);
                    break; 
                case Protocol.Round:

                    endRound?.Invoke(packet.Data);
                    break;
                case Protocol.Victory:

                    onVictory?.Invoke(packet.Data);
                    break;





                default:
                    Console.WriteLine("Неизвестная команда: " + packet.Command);
                    break;
            }
        }



        public void SendMessage(string command, byte[] data = null)
        {
            var packet = new Packet
            {
                Command = command,
                Data = data
            };

            byte[] packetBytes = packet.ToBytes();
            clientSocket.Send(packetBytes);
        }

        public void Disconnect()
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}

using Common;

using System.Net;
using System.Net.Sockets;



namespace Client3
{
    using static Package11207Helper;
    public class GameClient
    {
        private Socket clientSocket;
        public event Action<byte[]> OnGameStarted;

        public event Action<byte[]> UpdateGame;
        public event Action<byte[]> onVictory;
        public event Action<byte[]> endRound;
        public event Action<byte[]> Error;
        public event Action<string> UnoAction;


        public async Task Connect(string ipAddress)
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                await clientSocket.ConnectAsync(IPAddress.Parse(ipAddress), 5000);
                await ReceiveMessages();
            }
            catch (SocketException ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Общая ошибка: {ex.Message}");
            }
        }
        private void DisconnectFromServer()
        {
            try
            {
                if (clientSocket.Connected)
                {
                   
                    clientSocket.Shutdown(SocketShutdown.Both); 
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Ошибка при отключении: {ex.Message}");
            }
            finally
            {
               
                clientSocket.Close(); 
            }

        }

        private async Task ReceiveMessages()
        {
            while (clientSocket.Connected)
            {
                try
                {
                    await GetResponse(clientSocket);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получеия: {ex.Message}");
                    break;
                }
            }
        }

        public async Task GetResponse(Socket socket)
        {
            var buffer = new byte[MaxPacketSize];
            var responseContent = new List<byte>();
            UnoCommand command;
            int contentLength;
            do
            {
                contentLength = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);
                command = GetCommand(buffer[Command]);
                responseContent.AddRange(GetContent(buffer, contentLength));

            } while (!IsFull(buffer[Fullness]));
            switch (command)
            {

                case UnoCommand.START:

                    OnGameStarted?.Invoke(responseContent.ToArray());

                    break;
                case UnoCommand.UPDATE_FIELD:
                    UpdateGame?.Invoke(responseContent.ToArray());
                    break;
                case UnoCommand.UNO:
                    UnoAction?.Invoke(command.ToString());
                    break;
                case UnoCommand.NEW_ROUND:

                    endRound?.Invoke(responseContent.ToArray());
                    break;
                case UnoCommand.ERROR_START:

                    Error?.Invoke(responseContent.ToArray());
                    break;
                case UnoCommand.VICTORY:

                    onVictory?.Invoke(responseContent.ToArray());
                   
                    break;
            }
            responseContent.Clear();



        }




        public async Task SendMessage(UnoCommand command, byte[] data = null)
        {
            var packages = GetPackagesByMessage(data, command, QueryType.Request);
            bool allSent = false;

            while (!allSent)
            {
                allSent = true; 

                foreach (var package in packages)
                {
                    try
                    {
                       
                        if (clientSocket.Poll(1000, SelectMode.SelectWrite) && clientSocket.Connected)
                        {
                            clientSocket.SendAsync(package, SocketFlags.None);

                        }
                        else
                        {
                            allSent = false; 
                            break; 
                        }
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show($"Ошибка сокета: {ex.Message}");
                      
                        allSent = false;
                        break; 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}");
                        allSent = false; 
                        break; 
                    }
                }
            }
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

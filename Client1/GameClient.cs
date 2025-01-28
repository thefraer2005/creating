using Common;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Client1
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


        private async Task ReceiveMessages()
        {
            while (true)
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
                allSent = true; // Предполагаем, что все пакеты будут отправлены

                foreach (var package in packages)
                {
                    try
                    {
                        if (clientSocket.Connected)
                        {
                            await clientSocket.SendAsync(package, SocketFlags.None);
                        }
                        else
                        {
                            // Соединение разорвано
                            MessageBox.Show("Соединение разорвано. Попытка переподключения...");
                            await Reconnect();

                            // Устанавливаем флаг, чтобы повторить отправку всех пакетов
                            allSent = false;
                            break; // Выход из цикла foreach и повторная попытка
                        }
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show($"Ошибка при отправке сообщения: {ex.Message}");
                        // Дополнительная обработка ошибки
                        allSent = false; // Устанавливаем флаг для повторной попытки
                        break; // Выход из цикла foreach
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла ошибка: {ex.Message}");
                        // Дополнительная обработка ошибки
                        allSent = false; // Устанавливаем флаг для повторной попытки
                        break; // Выход из цикла foreach
                    }
                }
            }
        }

        private async Task Reconnect()
        {
            // Логика для повторного подключения
            try
            {
                // Закрываем старый сокет, если он существует
                if (clientSocket != null)
                {
                    clientSocket.Close();
                }

                // Подключаемся к серверу (укажите правильный адрес)
                // Замените на фактический IP-адрес сервера
                await Connect("127.0.0.1");

                MessageBox.Show("Подключение восстановлено.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось восстановить соединение: {ex.Message}");
                // Дополнительная обработка ошибки
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


using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UnoServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new GameServer(); // Замените на ваш класс сервера
            await server.Start();

            // Ожидание ввода от пользователя
            Console.WriteLine("Нажмите любую клавишу для выхода...");
         
        }
    }
}

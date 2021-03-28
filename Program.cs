using Bot_Telegram;
using System;
using Telegram.Bot;

namespace TelegramBotExamples
{
   

    class Program
    {
        //llave del bot
        private static readonly TelegramBotClient Bot = new TelegramBotClient("1609292836:AAFzCw4wgYUESoCZeX5LXpsgpCMuqJzb8rA");

        static void Main(string[] args)
        {
            SearchComplete guardar = new SearchComplete();

            var option = new ButtonsProducts();

            Robot robot = new Robot(Bot);

            //Método que se ejecuta cuando se recibe un mensaje
            Bot.OnMessage += robot.BotonMessageReceived;

            //Método que se ejecuta cuando se recibe un callbackQuery
            Bot.OnCallbackQuery += robot.BotonCallbackQueryReceived;

            //Método que se ejecuta cuando se recibe un callbackQuery de Tipo
            Bot.OnCallbackQuery += robot.BotonCallbackQueryReceivedType;

            //Método que se ejecuta cuando se recibe un callbackQuery de Marcas
            Bot.OnCallbackQuery += robot.BotonCallbackQueryReceivedBrands;

            //Método que se ejecuta cuando se recibe un callbackQuery de Portafolios
            Bot.OnCallbackQuery += robot.BotonCallbackQueryReceivedBriefcase;

            //Método que se ejecuta cuando se recibe un error
            Bot.OnReceiveError += robot.BotonReceiveError;

            //Inicia el bot
            Bot.StartReceiving();
            Console.WriteLine("Levantamiento del Servicio");
            Console.ReadLine();
            Bot.StopReceiving();
        }
    }
}

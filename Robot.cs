using Bot_Telegram.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot_Telegram
{
    class Robot
    {
        public Robot(TelegramBotClient Bot)
        {
            this.Bot = Bot;
        }

        public TelegramBotClient Bot { get; set; }

        private ButtonsProducts option = new ButtonsProducts();

        private List<SearchComplete> list = new List<SearchComplete>();

        //Respuesta de Texto
        public async void BotonMessageReceived(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                string messageconsult = e.Message.Text.ToLower();

                var message = e.Message;

                var item = list.Where(a => a.Id == message.Chat.Id).FirstOrDefault();

                string[] keys = new string[] { "hola", "buenas", "buenos", "/start", "gracias" };

                string keyResult = keys.FirstOrDefault<string>(s => messageconsult.Contains(s));

                switch (keyResult)
                {
                    case "hola":
                        await Saludos(message);
                        break;
                    case "buenas":
                        await Saludos(message);
                        break;
                    case "buenos":
                        await Saludos(message);
                        break;
                    case "/start":
                        await Saludos(message);
                        break;
                    case "gracias":
                        var options = new InlineKeyboardMarkup(new[]
                        {
                            new []
                            {
                                InlineKeyboardButton.WithCallbackData(
                                    text:"Buena",
                                    callbackData: "buena"),
                                InlineKeyboardButton.WithCallbackData(
                                    text: "Mala",
                                    callbackData: "mala"),
                            }
                        });
                        await Bot.SendTextMessageAsync(
                            //message.Chat.Id,
                            message.Chat.Id,
                            text: "Espero que tus dudas se hayan resuelto, ¿Cómo te pareció nuestra charla ? 😉 .",
                            replyMarkup: options);
                        break;
                    default:
                        if (messageconsult.Contains("-") & !string.IsNullOrEmpty(this.option.serialFGN))
                        {
                            await GetSerialFGN(messageconsult, message);
                            this.option.serialFGN = "";
                        }
                        else if (!string.IsNullOrEmpty(this.option.serialMinTic) & (new Regex(@"^[a-zA-Z0-9]*\d*$").Match(messageconsult).Success))
                        {
                            await GetEquimentInformationAsync(messageconsult, message);
                            this.option.serialMinTic = "";
                        }
                        else if (!string.IsNullOrEmpty(item.Portafolio) & !string.IsNullOrEmpty(this.option.productos) & (new Regex(@"^[0-9]*\d*$").Match(messageconsult).Success))
                        {
                            await ConsultProductQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio, double.Parse(messageconsult));
                            this.option.productos = "";
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(
                                //message.Chat.Id,
                                message.Chat.Id,
                                text: "🤓 Lo siento, no tengo una respuesta a lo que escribes. ",
                                replyMarkup: new ReplyKeyboardRemove());
                        }
                        break;
                }
            }
        }

        //Respuesta de Botones Principales
        public async void BotonCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            var item = list.Where(a => a.Id == callbackQuery.Message.Chat.Id).FirstOrDefault();

            var text = "";

            switch (callbackQuery.Data)
            {
                case "serialFGN":
                    this.option.serialFGN = callbackQuery.Data;
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Por favor diligencie el serial y placa con guion al medio, sin espacios  Ejemplo: serial-placa"
                        );
                    break;

                case "serialMinTic":
                    this.option.serialMinTic = callbackQuery.Data;
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Por favor diligencie el serial sin espacios"
                        );
                    break;
                case "productos":
                    this.option.productos = callbackQuery.Data;
                    await Products(item.Id);
                    break;
                case "generacompra":
                    await Bot.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "En este momento estamos procesando tu solicitud 👨🏻‍💻");
                    break;
                case "cotizarotroproducto":
                    this.option.productos = callbackQuery.Data;
                    await Products(item.Id);
                    break;
                case "ningunadelasanteriores":
                    await Bot.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: "Fue un gusto atenderl@. ¡Hasta pronto! 👋");
                    break;
                case "buena":
                    await Bot.SendTextMessageAsync(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Fue un gusto atenderl@. ¡Hasta pronto! 👋");
                    break;
                case "mala":
                    text = "😥 Lamentamos tu inconformidad, por favor elija nuevamente las opciones para ayudarte: ";
                    await OptionsReturn(callbackQuery.Message.Chat.Id, text);
                    break;
                case "finalizar":
                    text = "Fue un gusto atenderl@.\n "+
                    "Elige tu opción:";
                    await OptionsReturn(callbackQuery.Message.Chat.Id, text);
                    break;

            }
        }

        //Respuestas de Botones tipos
        public async void BotonCallbackQueryReceivedType(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            var item = list.Where(a => a.Id == callbackQuery.Message.Chat.Id).FirstOrDefault();

            switch (callbackQuery.Data)
            {
                case "104":
                    item.TipoDeEquipo = callbackQuery.Data;
                    await Laptop(item.Id);
                    break;
                case "100":
                    item.TipoDeEquipo = callbackQuery.Data;
                    await PcDesk(item.Id);
                    break;
            }
        }

        //Respuestas de Botones Marcas
        public async void BotonCallbackQueryReceivedBrands(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            var item = list.Where(a => a.Id == callbackQuery.Message.Chat.Id).FirstOrDefault();

            switch (callbackQuery.Data)
            {
                case "003":
                    item.Marca = callbackQuery.Data;
                    await Lenovo(item.Id, item.TipoDeEquipo, item.Marca);
                    break;
                case "HEWLETT PACKARD":
                    item.Marca = callbackQuery.Data;
                    await Hp(item.Id, item.TipoDeEquipo, item.Marca);
                    break;
                case "EPSON":
                    item.Marca = callbackQuery.Data;
                    await Epson(item.Id, item.TipoDeEquipo, item.Marca);
                    break;
                case "DELL":
                    item.Marca = callbackQuery.Data;
                    await Dell(item.Id, item.TipoDeEquipo, item.Marca);
                    break;
            }
        }

        //Respuestas de Botones Portafolios
        public async void BotonCallbackQueryReceivedBriefcase(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            var item = list.Where(a => a.Id == callbackQuery.Message.Chat.Id).FirstOrDefault();

            switch (callbackQuery.Data)
            {
                case "01962BS":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "20AMS10L00":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "LK573LA#ABM":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "M5G51LA#ABM":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "1045MLBP1":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "002SA34329":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "YFKRX-1036309":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "VKMR6":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "10BD00KBLS":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "10NH0004LS":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "4VR03LT-ENS":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "8SL09EC":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "1020199960045":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
                case "72177595-DESEN":
                    item.Portafolio = callbackQuery.Data;
                    await ShippingQuantity(item.Id, item.TipoDeEquipo, item.Marca, item.Portafolio);
                    break;
            }
        }

        //Saludos con opciones
        private async Task Saludos(Telegram.Bot.Types.Message message)
        {
            //Botones de opciones
            var options = new InlineKeyboardMarkup(new[]
{
                new []
                {
                    InlineKeyboardButton.WithCallbackData(
                        text:"Consulta Serial FNG",
                        callbackData: "serialFGN"),
                    InlineKeyboardButton.WithCallbackData(
                        text: "Consulta Serial MinTic",
                        callbackData: "serialMinTic"),
                },
                new []
                {
                    InlineKeyboardButton.WithCallbackData(
                        text:"Consulta Nuestros Productos",
                        callbackData: "productos"),
                }
            });

            string usage =
                 $"Hola 👋 {message.Chat.FirstName} \n\n" +
                 "Soy Guía Bot 🤖 de Colsof.\n\n" +
                 "No soy humano, pero siempre busco entenderte.\n\n" +
                 "Estoy para ti las 24 horas ⏰.\n\n" +
                 "¿En qué puedo ayudarte hoy ?\n\n" +
                 "Elige tu opción: \n\n";

            await Bot.SendTextMessageAsync(
                message.Chat.Id,
                text: usage,
                replyMarkup: options);

            list.Add(new SearchComplete() { Id = message.Chat.Id });
        }

        private async Task OptionsReturn(long id, string text)
        {
            var options = new InlineKeyboardMarkup(new[]
            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"Consulta Serial FNG",
                                callbackData: "serialFGN"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "Consulta Serial MinTic",
                                callbackData: "serialMinTic"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"Consulta Nuestros Productos",
                                callbackData: "productos"),
                        }
            });
            await Bot.SendTextMessageAsync(
                chatId: id,
                text: text,
                replyMarkup: options);
        }

        //Busqueda de serial FGN
        private async Task GetSerialFGN(string messageconsult, Telegram.Bot.Types.Message message)
        {
            Services a = new Services();
            a.serial = messageconsult;
            var b = a.serial;
            bool c = a.SearchSeriales(b);

            if (c)
            {
                var options = new InlineKeyboardMarkup(new[]
{
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text:"Consultar otro serial",
                            callbackData: "serialFGN"),
                        InlineKeyboardButton.WithCallbackData(
                            text:"Finalizar",
                            callbackData: "finalizar"),
                    }
                });

                await Bot.SendTextMessageAsync(
                    message.Chat.Id,
                    text: "Este serial si existe en la BD 🤩",
                    replyMarkup: options
                    );
            }
            else
            {
                await Bot.SendTextMessageAsync(
                    message.Chat.Id,
                    text: "Uy! 😅 No hemos encontrado respuesta a lo que solicitaste, por favor verifica que se encuentra bien escrito📝"
                    );
            }
        }

        //Busqueda de serial MinTic
        private async Task GetEquimentInformationAsync(string messageconsult, Telegram.Bot.Types.Message message)
        {
            Services a = new Services();
            a.serial = messageconsult;
            var b = a.serial;
            EquipmentInformation c = a.GetInformationPartNumber(b);

            if (c != null)
            {
                int[] d = { c.PartNumber.Length, c.Brand.Length, c.Serial.Length, c.InternalNumber.Length, c.Type.Length };

                int textoLargo = d.Max();

                string tabla =
                $" ════════════════{ ObtenerRelleno(textoLargo, string.Empty, "═")}{Environment.NewLine}" +
                $"║ PartNumber     ║{c.PartNumber}{ObtenerRelleno(textoLargo, c.PartNumber, " ")}║{Environment.NewLine}" +
                $"║ Brand          ║{c.Brand }{ ObtenerRelleno(textoLargo, c.Brand, " ")}║{Environment.NewLine}" +
                $"║ Serial         ║{c.Serial }{ ObtenerRelleno(textoLargo, c.Serial, " ")}║{Environment.NewLine}" +
                $"║ InternalNumber ║{c.InternalNumber}{ ObtenerRelleno(textoLargo, c.InternalNumber, " ")}║{Environment.NewLine}" +
                $"║ Type           ║{c.Type }{ ObtenerRelleno(textoLargo, c.Type, " ")}║{Environment.NewLine}" +
                $" ════════════════{ ObtenerRelleno(textoLargo, string.Empty, "═")}{Environment.NewLine}";

                var options = new InlineKeyboardMarkup(new[]
{
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text:"Consultar otro serial",
                            callbackData: "serialMinTic"),
                        InlineKeyboardButton.WithCallbackData(
                            text:"Finalizar",
                            callbackData: "finalizar"),
                    }
                });

                await Bot.SendTextMessageAsync(
                    message.Chat.Id,
                    text: $"```{tabla}```",
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: options
                    );
            }
            else
            {
                await Bot.SendTextMessageAsync(
                    message.Chat.Id,
                    text: "Uy! 😅 No hemos encontrado respuesta a lo que solicitaste, por favor verifica que se encuentra bien escrito📝"
                    );
            }

        }

        //Botonera de Productos
        private async Task Products(long id)
        {
            var options = new InlineKeyboardMarkup(new[]
            {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"Portatiles💻",
                                callbackData: "104"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "Pc Escritorio🖥",
                                callbackData: "100"),
                        }
            });

            await Bot.SendTextMessageAsync(
            chatId: id,
            text: "Selecciona la categoria que deseas consultar 👀:\n\n",
            replyMarkup: options);
        }

        //Botonera de Marcas
        private async Task Brand(long id)
        {
            var options = new InlineKeyboardMarkup(new[]
{
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"LENOVO",
                                callbackData: "003"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "HP",
                                callbackData: "HEWLETT PACKARD"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"EPSON",
                                callbackData: "EPSON"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "DELL",
                                callbackData: "DELL"),
                        }
                    });

            await Bot.SendTextMessageAsync(
            chatId: id,
            text: "Selecciona la Marca 📌:\n\n",
            replyMarkup: options);

        }

        //Botoneras de Portafolios
        private async Task Briefcase(long id, string tipodeequipo, string marca)
        {
            await Bot.SendTextMessageAsync(
            chatId: id,
            text: "Un momento, estamos consultado los productos disponibles para esta Marca.\n\n");
            await LaptopBrands(id, tipodeequipo, marca);
            await DeskBrands(id, tipodeequipo, marca);
        }

        //Envio de catidades de Portafolio
        private async Task ShippingQuantity(long id, string tipodeequipo, string marca, string portafolio)
        {
            Services a = new Services();
            a.serial = portafolio;
            var b = a.serial;
            double c = a.ProductQuantity(b);

            if (c == 0)
            {
                await Bot.SendTextMessageAsync(
                chatId: id,
                text: "En este momento no contamos con existencias de esta referencia.");
                await Briefcase(id, tipodeequipo, marca);
            }
            else
            {
                await Bot.SendTextMessageAsync(
                chatId: id,
                text: "¿Cuantas cantidades de este producto quieres cotizar?");
            }
        }

        //Verificacion de cantidades en BD con multiplicacion sobre estas
        private async Task ConsultProductQuantity(long id, string tipodeequipo, string marca, string portafolio, double cantidad)
        {
            Services a = new Services();
            a.serial = portafolio;
            var b = a.serial;
            double c = a.ProductQuantity(b);

            if (c > cantidad)
            {
                string vu = string.Format("{0:C2}", a.PriceProduct(b));

                string vt = string.Format("{0:C2}", (a.PriceProduct(b) * cantidad));

                int[] e = { cantidad.ToString().Length, vu.Length, vt.Length };

                int textoLargo = e.Max();

                string tabla =
                $" ═════════════════{ ObtenerRelleno(textoLargo, string.Empty, "═")}{Environment.NewLine}" +
                $"║ Cantidad       ║ {cantidad }{ObtenerRelleno(textoLargo, cantidad.ToString(), " ")}║{Environment.NewLine}" +
                $"║ Vlr Unitario   ║ {vu }{ ObtenerRelleno(textoLargo, vu, " ")}║{Environment.NewLine}" +
                $"║ Vlr Total      ║ {vt }{ ObtenerRelleno(textoLargo, vt, " ")}║{Environment.NewLine}" +
                $" ═════════════════{ ObtenerRelleno(textoLargo, string.Empty, "═")}{Environment.NewLine}";

                var options = new InlineKeyboardMarkup(new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text:"Generar compra",
                            callbackData: "generacompra"),

                        InlineKeyboardButton.WithCallbackData(
                            text:"¿Cotizar otro producto?",
                            callbackData: "cotizarotroproducto"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(
                            text:"Ninguna de las anteriores",
                            callbackData: "ningunadelasanteriores"),
                    }
                });

                await Bot.SendTextMessageAsync(
                   chatId: id,
                   text: $"```{tabla}```",
                   parseMode: ParseMode.MarkdownV2,
                   replyMarkup: options
                   );
            }
            else
            {

                var options = new InlineKeyboardMarkup(new[]
                {
                new []
                {
                    InlineKeyboardButton.WithCallbackData(
                        text:"Consulta Nuestros Productos",
                        callbackData: "productos"),
                }
                });

                await Bot.SendTextMessageAsync(
                chatId: id,
                text: "En el momento no contamos con esa Cantidad, por favor consultar otro producto",
                replyMarkup: options);
            }
        }


        #region "Type Products"

        private async Task Laptop(long id)
        {
            await Brand(id);
        }

        private async Task PcDesk(long id)
        {
            await Brand(id);
        }

        #endregion

        #region "Brands"

        private async Task Lenovo(long id, string tipodeequipo, string marca)
        {
            await Briefcase(id, tipodeequipo, marca);
        }

        private async Task Hp(long id, string tipodeequipo, string marca)
        {
            await Briefcase(id, tipodeequipo, marca);
        }

        private async Task Epson(long id, string tipodeequipo, string marca)
        {
            await Briefcase(id, tipodeequipo, marca);
        }

        private async Task Dell(long id, string tipodeequipo, string marca)
        {
            await Briefcase(id, tipodeequipo, marca);
        }

        #endregion

        #region "Briefcase"

        const string referencia = "Selecciona la referencia de tu interes\n\n";
        private async Task LaptopBrands(long id, string tipodeequipo, string marca)
        {
            switch (tipodeequipo + "-" + marca)
            {
                case "104-003":
                    var options1 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"LENOVO THINKPAD 13Pulgadas",
                                callbackData: "01962BS"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "LENOVO X240 I7 THINKPAD X240",
                                callbackData: "20AMS10L00"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options1);

                    break;

                case "104-HEWLETT PACKARD":
                    var options2 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"PAVILION DV5-2230LA",
                                callbackData: "LK573LA#ABM"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "ZBOOK 15U ",
                                callbackData: "M5G51LA#ABM"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options2);

                    break;

                case "104-EPSON":
                    var options3 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"PALM TUNGSTEN E 2",
                                callbackData: "1045MLBP1"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "EPSON ACTIONNOTE",
                                callbackData: "002SA34329"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options3);

                    break;

                case "104-DELL":
                    var options4 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"LATITUDE 5480",
                                callbackData: "YFKRX-1036309"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "LATITUDE 7390",
                                callbackData: "VKMR6"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options4);

                    break;
            }
        }

        private async Task DeskBrands(long id, string tipodeequipo, string marca)
        {
            switch (tipodeequipo + "-" + marca)
            {
                case "100-003":
                    var options1 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"THINKCENTRE E73Z",
                                callbackData: "10BD00KBLS"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "V510Z ALL-IN-ONE 23",
                                callbackData: "10NH0004LS"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options1);

                    break;
                case "100-HEWLETT PACKARD":
                    var options2 = new InlineKeyboardMarkup(new[]
                    {
                    new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"PRODESK 400 G4",
                                callbackData: "4VR03LT-ENS"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "EliteDesk 800G5",
                                callbackData: "8SL09EC"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options2);

                    break;

                case "100-EPSON":
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: "En el momento no contamos con productos disponibles\n\n");

                    break;

                case "100-DELL":
                    var options4 = new InlineKeyboardMarkup(new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData(
                                text:"OPTIPLEX 3040",
                                callbackData: "1020199960045"),
                            InlineKeyboardButton.WithCallbackData(
                                text: "OPTIPLEX 7050 MICRO FORM FACTOR",
                                callbackData: "72177595-DESEN"),
                        },
                    });
                    await Bot.SendTextMessageAsync(
                    chatId: id,
                    text: referencia,
                    replyMarkup: options4);

                    break;
            }
        }

        #endregion

        private static string ObtenerRelleno(int type, string textoBase, string caracter)
        {
            var faltante = type - textoBase.Length;

            return new string(Convert.ToChar(caracter), faltante + 1);
        }

        public void BotonReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
            receiveErrorEventArgs.ApiRequestException.ErrorCode,
            receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}

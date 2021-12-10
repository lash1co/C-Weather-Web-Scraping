using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Timers;

namespace WebScrapingConsoleApp
{
    class Program
    {
        public static bool banderaTemperatura = false;
        public static bool banderaUvi = false;
        public static bool banderaCalidad = false;
        public static bool banderaHumedad = false;
        static string temperatura;
        static string uvi;
        static string calidadAire;
        static string estadoClima;
        static string humedad;
        static void Main(string[] args)
        {
            //Se crea un timer para hacer una tarea repetitiva y se ejecuta la tarea de scraping
            Timer timer = new Timer(300000);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            consultarDatosWeb();
            //Ciclo Infinito para que la app no se detenga o cierre a menos de que sea por intervención externa
            while (true) 
            { 
            
            }
        }
        //Función del timer que se ejecuta cada 5 minutos
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            consultarDatosWeb();
        }

        //Función que hace Scraping de una pagina web en concreto, en este caso una pagina web de clima
        private static void consultarDatosWeb() 
        {
            HtmlWeb oWeb = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc = oWeb.Load("https://weather.com/es-CO/tiempo/hoy/l/c503ea15a6530e5108edae16b80456a49736653dee9afe4aee27fad3bb9b4f0e");
            foreach (var nodo in doc.DocumentNode.SelectNodes("//span[@data-testid]")) // Busca un nodo que tenga span y posea un atributo en particular
            {
                if (nodo.GetAttributeValue("data-testid").Contains("TemperatureValue") && !banderaTemperatura) // Busca un valor en particular para extraer el dato que se busca
                {
                    temperatura = nodo.InnerText;
                    banderaTemperatura = true; // Se coloca una bandera para que no siga buscando este mismo dato mas adelante
                }
                if (nodo.GetAttributeValue("data-testid").Contains("UVIndexValue") && !banderaUvi)
                {
                    uvi = nodo.InnerText;
                    banderaUvi = true;
                }
                if (nodo.GetAttributeValue("data-testid").Contains("PercentageValue") && !banderaHumedad)
                {
                    humedad = nodo.InnerText;
                    banderaHumedad = true;
                }
            }
            foreach (var nodo2 in doc.DocumentNode.SelectNodes("//div[@data-testid]"))
            {
                if (nodo2.GetAttributeValue("data-testid").Contains("AirQualityCard") && !banderaCalidad)
                {
                    foreach (var subnodo in nodo2.SelectNodes("//text[@data-testid]"))
                    {
                        if (subnodo.GetAttributeValue("data-testid").Contains("DonutChartValue"))
                        {
                            calidadAire = subnodo.InnerText;
                        }
                    }
                    foreach (var subnodo in nodo2.SelectNodes("//span[@data-testid]"))
                    {
                        if (subnodo.GetAttributeValue("data-testid").Contains("AirQualityCategory"))
                        {
                            calidadAire = calidadAire + " => " + subnodo.InnerText + ".";
                        }
                    }
                    foreach (var subnodo in nodo2.SelectNodes("//p[@data-testid]"))
                    {
                        if (subnodo.GetAttributeValue("data-testid").Contains("AirQualitySeverity"))
                        {
                            calidadAire = calidadAire + " " + subnodo.InnerText;
                        }
                    }
                    banderaCalidad = true;
                }
                if (nodo2.GetAttributeValue("data-testid").Contains("wxPhrase"))
                {
                    estadoClima = nodo2.InnerText;
                }
            }
            Console.WriteLine("Hora actual: "+DateTime.Now.ToString());
            Console.WriteLine("En la ciudad de Pereira tenemos:");
            Console.WriteLine("Temperatura: " + temperatura);
            Console.WriteLine("Uvi: " + uvi);
            Console.WriteLine("Humedad: " + humedad);
            Console.WriteLine("Calidad de Aire: " + calidadAire);
            Console.WriteLine("Clima: " + estadoClima);
        }
    }
}

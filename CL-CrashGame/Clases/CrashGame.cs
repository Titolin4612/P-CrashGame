using System;

namespace CL_CrashGame.Clases
{
    public class CrashGame
    {
        // Atributos
        private double multiplicador; // Pa llevar la cuenta de cuánto sube esa mierda si sabe
        private double puntoColision; // Donde el astronauta se muere ese hp
        private double apuestaJugador; // Plata que el mka mete pal chance
        private double multiplicadorJugadorCobrado; // Cuánto se corono la gonorrea

        // Atributos de estado
        private bool juegoActivo; // Pa saber si la mkada esta funcionando
        private bool retiroJugador; 

        // Random
        private Random generadorAleatorio = new Random(); 

        private const double margenCasa = 2.0; // Es como el porcentaje de ganancia de la casa si o q

        public void IniciarJuego()
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("=== ¡Bienvenido al Casino CesiArmy! ===");
            Console.WriteLine("===========================================");
            Console.WriteLine();
            Console.WriteLine("El astronauta va a subir y el multiplicador también.");
            Console.WriteLine("Presiona 'C' pa COBRAR antes de que se estrelle ese hp.");
            Console.WriteLine();
            Console.WriteLine("===========================================");

            while (true)
            {
                IniciarNuevaRonda(); 
                IniciarRonda(); 
                MostrarResultadosRonda(); // Mostrar si el mka ganó o se jodió

                Console.Write("\n¿Jugar otra ronda o miedo? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s")
                {
                    break; // Si el mka le saca el culo, se acaba la mkada
                }
            }
            Console.WriteLine("\nGracias por jugar, abrite pa la puta mierda!");
        }

        private void IniciarNuevaRonda()
        {
            Console.WriteLine("\n--- Nueva Ronda ---");
            apuestaJugador = ObtenerApuestaJugador(); 
            multiplicador = 1.0;
            juegoActivo = true; // La mkada tá prendida
            retiroJugador = false; // El mka no ha cobrado 
            multiplicadorJugadorCobrado = 0; // Nada en el bolsillo

            double valorAleatorio = generadorAleatorio.NextDouble(); 

            // Punto de colisión = (100 - MargenPorcentual) / (100 * (1 - valorAleatorio))
            // asegurando que el retorno por jugador esté por debajo del 100% pq si no perdemos plata si sabe.
            puntoColision = (100.0 - margenCasa) / (100.0 * (1.0 - valorAleatorio));

            puntoColision = Math.Floor(puntoColision * 100.0) / 100.0; // Redondear a 2 decimales pa que sea mas chimba

            // Asegurar que el punto de colisión sea como mínimo 1.00x.
            // como esa gonorrea de formula puede dar menor a 0 en ciertos casos entonces
            // se ajusta a 1.00x, pa que sea una colisión de una.
            if (puntoColision < 1.0)
            {
                puntoColision = 1.00;
            }
        }

        private double ObtenerApuestaJugador()
        {
            double montoApuesta;
            while (true)
            {
                Console.Write($"Ingresa tu apuesta: ");
                string entrada = Console.ReadLine();
                if (double.TryParse(entrada, out montoApuesta) && montoApuesta > 0)
                {
                    return montoApuesta; 
                }
                Console.WriteLine("Apuesta inválida. Debe ser un número positivo, no mkadas!");
            }
        }

        private void IniciarRonda()
        {
            Console.WriteLine($"\n¡El astronauta comienza a subir! Apuesta: {apuestaJugador:C}");
            Console.WriteLine("Presiona 'C' en cualquier momento para COBRAR.");

            while (juegoActivo)
            {
                // 1. Pillar si el multiplicador ya está o pasó el punto de choque
                if (multiplicador >= puntoColision)
                {
                    juegoActivo = false; // Se acabó la mkada
                    multiplicador = puntoColision;
                    Console.Write($"\rMultiplicador: {multiplicador:F2}x - ");
                    Console.ForegroundColor = ConsoleColor.Red; // Colorcito bien bellaco pa fail
                    Console.WriteLine("¡SE CHOCÓ! Muy salado.");
                    Console.ResetColor();
                    break;
                }

                // 2. Subir el multiplicador
                multiplicador += 0.01; // Va subiendo de a poquitos
                multiplicador = Math.Round(multiplicador, 2); // Redondear a 2 decimales pa que sea chimba

                // 3. Mostrar el multiplicador actual. Esa mierda de \r es pa sobreescribir la línea
                Console.Write($"\rMultiplicador: {multiplicador:F2}x ");

                // 4. Mirar si don chimbo apreto la 'C'
                if (Console.KeyAvailable)
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.C)
                    {
                        IntentarCobrar(); // El mka quiere la plata
                        if (retiroJugador)
                        {
                            juegoActivo = false;
                            Console.Write($"\rMultiplicador en el momento: {multiplicadorJugadorCobrado:F2}x - ");
                            Console.ForegroundColor = ConsoleColor.Green; // Colorcito de victoria merlano
                            Console.WriteLine("¡GANASTE HPTA!");
                            Console.ResetColor();
                            break;
                        }
                    }
                }

                // 5. Un momentico pa controlar la velocidad, 100ms pa que suba 0.10x por segundo
                System.Threading.Thread.Sleep(100);
            }
        }

        private void IntentarCobrar()
        {
            if (juegoActivo && !retiroJugador)
            {
                multiplicadorJugadorCobrado = multiplicador; // Guardar el multiplicador cuando el mka cobra
                retiroJugador = true; // Marcar que el mka ya cobró
            }
        }

        private void MostrarResultadosRonda()
        {
            if (retiroJugador) // Si el mka cobró
            {
                double ganancia = apuestaJugador * multiplicadorJugadorCobrado;
                Console.ForegroundColor = ConsoleColor.Green; // Colorcito bien victorinostylo
                Console.WriteLine($"\n¡Felicidades! Cobraste con éxito a {multiplicadorJugadorCobrado:F2}x.");
                Console.WriteLine($"Tu apuesta fue de: {apuestaJugador:C}");
                Console.WriteLine($"Ganaste: {ganancia:C}");
                Console.ResetColor();
            }
            else // Si el astronauta se hizo mierda
            {
                Console.ForegroundColor = ConsoleColor.Red; // Color bien gonorrea de fallo
                Console.WriteLine($"\nEl astronauta se murio a {puntoColision:F2}x.");
                Console.WriteLine($"Perdiste toda la carechimba apuesta de: {apuestaJugador:C}");
                Console.ResetColor();
            }
        }
    }
}
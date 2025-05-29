using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL_CrashGame.Clases
{
    public class CrashGame
    {
        // Atributos
        private double multiplicador;
        private double puntoColision; 
        private double apuestaJugador;
        private double multiplicadorJugadorCobrado;

        // Atributos de estado
        private bool juegoActivo;
        private bool retiroJugador; 

        // Random
        private Random generadorAleatorio = new Random(); 

        private const double margenCasa = 2.0; // Es como el porcentaje de ganancia de la casa si o q

        private CancellationTokenSource tokenCancelacionRonda; // Esto es pa poder cancelar chimbadas de la ronda

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
                MostrarResultadosRonda(); 

                Console.Write("\n¿Jugar otra ronda o miedo? (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s")
                {
                    break; 
                }
            }
            Console.WriteLine("\nGracias por jugar. ¡Vuelve pronto al espacio!");
        }

        private void IniciarNuevaRonda()
        {
            Console.WriteLine("\n--- Nueva Ronda ---");
            apuestaJugador = ObtenerApuestaJugador(); 
            multiplicador = 1.0; 
            juegoActivo = true; 
            retiroJugador = false; 
            multiplicadorJugadorCobrado = 0;
            tokenCancelacionRonda = new CancellationTokenSource();

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

                Console.WriteLine("Apuesta inválida. Debe ser un número positivo.");
                return 0;

            }
        }

        private void IniciarRonda()
        {
            Console.WriteLine($"\n¡El astronauta comienza a subir! Apuesta: {apuestaJugador:C}");
            Console.WriteLine("Presiona 'C' en cualquier momento para COBRAR.");

            // Esta mkada es como un listener pa escuchar cuando el mka le de a la C
            Task tareaEntrada = Task.Run(() => EscucharPaCobrar(tokenCancelacionRonda.Token), tokenCancelacionRonda.Token);

            while (juegoActivo)
            {
                // 1. Pillar si el multiplicador ya esta o paso el punto de choque.
                if (multiplicador >= puntoColision)
                {
                    juegoActivo = false; 

                    multiplicador = puntoColision;
                    Console.Write($"\rMultiplicador: {multiplicador:F2}x - ");
                    Console.ForegroundColor = ConsoleColor.Red; // Un colorcito bien bellaco. esto es solo pa consola
                    Console.WriteLine("¡COLISIÓN! Mala suerte esta vez.");
                    Console.ResetColor(); 
                    break; 
                }

                // 2. Incrementar el multiplicador.

                multiplicador += 0.01; // Va subiendo de a 0.01

                multiplicador = Math.Round(multiplicador, 2); // Redondear a 2 decimales pa que sea mas chimba

                // 3. Mostrar el multiplicador actual. Esa mkada de \r es pa sobreescribir la línea en la que esta.
                Console.Write($"\rMultiplicador: {multiplicador:F2}x ");

                // 4. Un controlcito pa controlar la velocidad.
                // 100ms por cada incremento de 0.01x eso es q el multiplicador sube 0.10x por segundo si o q.
                try
                {
                    Task.Delay(100, tokenCancelacionRonda.Token).Wait(); // Esperar o terminar si el mka cancela, pq si no daba error.
                }
                catch (OperationCanceledException)
                {

                    if (!juegoActivo) break; // Si la gonorrea cancela pues se sale
                }

                // 5. Verificar si el mka ya cobro
                if (retiroJugador)
                {
                    juegoActivo = false;

                    Console.Write($"\rMultiplicador en el momento del cobro: {multiplicadorJugadorCobrado:F2}x - ");
                    Console.ForegroundColor = ConsoleColor.Green; // Colorcito de victoria merlano 
                    Console.WriteLine("¡GANASTE HPTA!");
                    Console.ResetColor();
                    break; 
                }
            }

            // Esto es pa cancelar cualquier chimbada que quede pendiente despues del juego
            if (!tokenCancelacionRonda.IsCancellationRequested)
            {
                tokenCancelacionRonda.Cancel();
            }

            // Esperar un rose pa que la tarea frene pq si no daba error
            try
            {
                tareaEntrada.Wait(TimeSpan.FromMilliseconds(200));
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("Que hpta error", ae);
            }
        }

        private void EscucharPaCobrar(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && juegoActivo && !retiroJugador)
                {
                    if (Console.KeyAvailable) // Verificar si hay una tecla presionada sin bloquear.
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.C) 
                        {
                            IntentarCobrar();

                            break;
                        }
                    }

                    // Una pausita ahi breve pa chill, eso tambien manda disq OperationCanceledException si el token es cancelado.
                    token.WaitHandle.WaitOne(50);
                }
            }
            catch (OperationCanceledException oc)
            {
                Console.WriteLine("Se cancelo esa chimbada", oc);
            }
        }

        private void IntentarCobrar()
        {
            if (juegoActivo && !retiroJugador)
            {

                multiplicadorJugadorCobrado = multiplicador;
                retiroJugador = true; // Marcar que el jugador ha cobrado.

            }
        }

        private void MostrarResultadosRonda()
        {
            if (retiroJugador) // Si el mka gano
            {
                double ganancia = apuestaJugador * (double)multiplicadorJugadorCobrado;
                Console.ForegroundColor = ConsoleColor.Green; // Colorcito bien victorinostylo
                Console.WriteLine($"\n¡Felicidades! Cobraste con éxito a {multiplicadorJugadorCobrado:F2}x.");
                Console.WriteLine($"Tu apuesta fue de: {apuestaJugador:C}");
                Console.WriteLine($"Ganaste: {ganancia:C}");
                Console.ResetColor();
            }
            else // Si el astronauta se murio.
            {
                Console.ForegroundColor = ConsoleColor.Red; // Color bien gonorrea de fallo
                Console.WriteLine($"\nEl astronauta se murio a {puntoColision:F2}x.");
                Console.WriteLine($"Perdiste tu apuesta de: {apuestaJugador:C}");
                Console.ResetColor();
            }


        }

    }
}
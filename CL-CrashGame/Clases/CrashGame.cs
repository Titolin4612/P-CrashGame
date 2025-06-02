using System;
using System.ComponentModel.Design;

namespace CL_CrashGame.Clases
{
    public class CrashGame
    {
        // Atributos
        private double multiplicador;
        private double puntoColision; // Donde el astronauta se muere ese hp




        private List<ConsoleKey> l_teclas = new List<ConsoleKey>{

            ConsoleKey.Z,ConsoleKey.X,ConsoleKey.C,ConsoleKey.V,ConsoleKey.B

        };

        private List<Jugador> l_jugadores;

        // Atributos de estado
        private bool juegoActivo;




        // Random
        private Random generadorAleatorio = new Random();

        private const double margenCasa = 2.0; // Es como el porcentaje de ganancia de la casa si o q

        //idea de frontend, tanque al que se suben las personas , cada vez que se retira una sale del tanque
        //minero minando oro y multiplicando hasta que la dinamita explota

        public CrashGame()
        {
            l_jugadores = new List<Jugador>();
        }


        public void IniciarJuego()
        {


            Console.WriteLine("===========================================");
            Console.WriteLine("=== ¡Bienvenido a CrashTank! ===");
            Console.WriteLine("===========================================");
            Console.WriteLine();
            Console.WriteLine("El Tanque comenzara la avanzada y el multiplicador también.");
            Console.WriteLine("presione tu codigo de salida antes de que el tanque sea destruido");
            Console.WriteLine();
            Console.WriteLine("===========================================");

            while (true)
            {
                IniciarNuevaRonda();
                IniciarRonda();
                MostrarResultadosRonda();
                l_jugadores.Clear();



                Console.Write("\n¿Subete a otro Tanque (s/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "s")
                {
                    break;
                }
            }
            Console.WriteLine("\n Fin de la misión!");
        }

        private void IniciarNuevaRonda()
        {
            Console.WriteLine("\n--- Nueva Ronda ---");
            LlenarSala();
            multiplicador = 1.0;
            juegoActivo = true;




            double valorAleatorio = generadorAleatorio.NextDouble();


            puntoColision = (100.0 - margenCasa) / (100.0 * (1.0 - valorAleatorio));

            puntoColision = Math.Floor(puntoColision * 100.0) / 100.0;


            if (puntoColision < 1.0)
            {
                puntoColision = 1.00;
            }
            Console.WriteLine(puntoColision);
        }

        private double ObtenerApuestaJugador(Jugador jugadorNuevo)
        {
            double montoApuesta;
            while (true)
            {
                Console.Write($"Ingresa tu apuesta: ");


                string entrada = Console.ReadLine();
                if (double.TryParse(entrada, out montoApuesta) && montoApuesta > 0)
                {
                    jugadorNuevo.Apostar(montoApuesta);
                    return montoApuesta;
                }
                Console.WriteLine("Apuesta inválida. Debe ser un número positivo ");
            }
        }

        private void IniciarRonda()
        {
            Console.WriteLine($"\n¡El Tanque  comienza a Atacar!!!");
            Console.WriteLine("Presiona tu codigo  en cualquier momento para Retirarte.");

            while (juegoActivo)
            {
                // 1. Pillar si el multiplicador ya está o pasó el punto de choque
                if (multiplicador >= puntoColision)
                {
                    juegoActivo = false;
                    multiplicador = puntoColision;
                    Console.Write($"\rMultiplicador: {multiplicador:F2}x - ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("¡Tanque destruido! ");
                    Console.ResetColor();
                    foreach (Jugador soldado in l_jugadores)
                    {
                        if (soldado.EstaJugando)
                        {
                            soldado.HaGanado = false;
                        }
                    }
                    break;
                }

                // 2. Subir el multiplicador
                multiplicador += 0.01;
                multiplicador = Math.Round(multiplicador, 2);

                // 3. Mostrar el multiplicador actual. \r es para sobreescribir la línea
                Console.Write($"\rMultiplicador: {multiplicador:F2}x ");




                if (Console.KeyAvailable)
                {
                    ConsoleKey teclaPresionada = Console.ReadKey(true).Key;
                    var jugadorRetirado = l_jugadores.FirstOrDefault(j => j.TeclaRetiro == teclaPresionada);

                    if (jugadorRetirado != null)
                    {
                        IntentarCobrar(jugadorRetirado, multiplicador);

                    }
                }

                if (multiplicador < 3)
                {
                    System.Threading.Thread.Sleep(95);
                }
                else if (multiplicador >= 3 && multiplicador < 8)
                {
                    System.Threading.Thread.Sleep(70);
                }
                else
                {
                    System.Threading.Thread.Sleep(55);

                }

            }
        }

        private void IntentarCobrar(Jugador jugadorRetirado, double multiplicadorRetirada)
        {
            if (juegoActivo && jugadorRetirado.EstaJugando)
            {
                jugadorRetirado.Saldo += jugadorRetirado.ApuestaActual * multiplicadorRetirada;
                Console.WriteLine($"el soldado {jugadorRetirado.Usuario}  se ha retirado con exito en {multiplicadorRetirada}x  ------tiene un saldo de {jugadorRetirado.Saldo}");
                jugadorRetirado.EstaJugando = false;
                jugadorRetirado.ApuestaActual = 0;
                jugadorRetirado.HaGanado = true;
            }
        }



        private void LlenarSala()

        {

            while (true && l_jugadores.Count < 5)
            {

                if (l_jugadores.Count == 0)
                {

                    Console.WriteLine("Añade un soldado al juego ");
                }
                else
                {
                    Console.WriteLine("Entrara un soldado más? (s/n)");
                    if (Console.ReadLine()?.Trim().ToLower() != "s")
                    {
                        break;
                    }
                }
                Console.ForegroundColor = ConsoleColor.Blue;

                Console.WriteLine("cual es el nombre del soldado");
                string nombreSoldado = Console.ReadLine();
                Jugador soldado = new Jugador(nombreSoldado);
                l_jugadores.Add(soldado);

                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;

                Console.WriteLine("cuanto valor tiene el soldado?");
                ObtenerApuestaJugador(soldado);

                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;

                Console.WriteLine($"su codigo será la tecla {l_teclas[l_jugadores.Count - 1]} ");
                soldado.TeclaRetiro = l_teclas[l_jugadores.Count - 1];


                Console.ResetColor();
            }


        }


        private void MostrarResultadosRonda()
        {
            foreach (Jugador soldado in l_jugadores)
            {
                Console.WriteLine($"el soldado {soldado.Usuario} ha {(soldado.HaGanado ? "ganado" : "perdido")} y su saldo actual es de {soldado.Saldo}");
            }
        }

    }
}
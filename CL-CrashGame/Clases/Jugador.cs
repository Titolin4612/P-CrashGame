using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL_CrashGame.Clases
{
    class Jugador
    {
        private string usuario;
        private double saldo;
        private double apuestaActual;
        private bool estaJugando = false;
        private ConsoleKey teclaRetiro;
        private bool haGanado;


        public Jugador(string usuario)
        {
            this.usuario = usuario;
            saldo = 20000;
            apuestaActual = 0;
        }

        public string Usuario { get => usuario; }
        public double Saldo { get => saldo; set => saldo = value; }
        public double ApuestaActual { get => apuestaActual; set => apuestaActual = value; }
        public bool EstaJugando { get => estaJugando; set => estaJugando = value; }
        public ConsoleKey TeclaRetiro { get => teclaRetiro; set => teclaRetiro = value; }
        public bool HaGanado { get => haGanado; set => haGanado = value; }

        public void Apostar(double apuesta)
        {
            if (apuesta > 0)
                apuestaActual = apuesta;
            saldo -= apuesta;
            estaJugando = true;
        }
    }
}
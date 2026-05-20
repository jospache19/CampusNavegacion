using System;
using System.Collections.Generic;
using System.Linq;

namespace CampusNavegacion
{
    public class TablaHash
    {
        private Dictionary<string, int> visitas;

        public TablaHash()
        {
            visitas = new Dictionary<string, int>();
        }

        public void RegistrarVisita(string edificio)
        {
            if (visitas.ContainsKey(edificio))
            {
                visitas[edificio]++;
            }
            else
            {
                visitas[edificio] = 1;
            }
        }

        public int ObtenerConteo(string edificio)
        {
            if (visitas.ContainsKey(edificio))
            {
                return visitas[edificio];
            }
            return 0;
        }

        public void MostrarEstadisticas()
        {
            Console.WriteLine("\n=== ESTADÍSTICAS DE VISITAS ===");

            if (visitas.Count == 0)
            {
                Console.WriteLine("Aún no hay visitas registradas.");
                return;
            }

            var edificiosOrdenados = visitas.Keys.OrderByDescending(e => ObtenerConteo(e)).ToList();

            foreach (string edificio in edificiosOrdenados)
            {
                int cantidad = ObtenerConteo(edificio);
                Console.WriteLine($"{edificio,-30} {cantidad} visitas");
            }

            
            string topEdificio = EdificioMasVisitado();
            Console.WriteLine($"\nEdificio más visitado: {topEdificio} con {ObtenerConteo(topEdificio)} visitas");
        }

        public string EdificioMasVisitado()
        {
            if (visitas.Count == 0) return null;

            return visitas.OrderByDescending(x => x.Value).First().Key;
        }
    }
}
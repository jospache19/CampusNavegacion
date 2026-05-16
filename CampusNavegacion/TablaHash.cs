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

            var visitasOrdenadas = visitas.OrderByDescending(x => x.Value);

            foreach (var item in visitasOrdenadas)
            {
                string sufijo = item.Value == 1 ? "visita" : "visitas";
                Console.WriteLine($"{item.Key,-25} {item.Value} {sufijo}");
            }

            string masVisitado = EdificioMasVisitado();
            if (!string.IsNullOrEmpty(masVisitado))
            {
                Console.WriteLine($"\nEdificio más visitado: {masVisitado} con {visitas[masVisitado]} visitas");
            }
        }

        public string EdificioMasVisitado()
        {
            if (visitas.Count == 0) return null;

            return visitas.OrderByDescending(x => x.Value).First().Key;
        }
    }
}
using System;
using System.Collections.Generic;

namespace CampusNavegacion
{
    public class Grafo
    {
       
        private Dictionary<string, List<Tuple<string, int>>> listaAdyacencia;

        public Grafo()
        {
            listaAdyacencia = new Dictionary<string, List<Tuple<string, int>>>();
        }

        public void AgregarEdificio(string nombre)
        {
            if (!listaAdyacencia.ContainsKey(nombre))
            {
                listaAdyacencia[nombre] = new List<Tuple<string, int>>();
            }
        }

        public void AgregarCamino(string origen, string destino, int distancia)
        {
            AgregarEdificio(origen);
            AgregarEdificio(destino);

            // Grafo bidireccional
            listaAdyacencia[origen].Add(new Tuple<string, int>(destino, distancia));
            listaAdyacencia[destino].Add(new Tuple<string, int>(origen, distancia));
        }

        public void MostrarGrafo()
        {
            Console.WriteLine("=== MAPA DEL CAMPUS ===");
            foreach (var edificio in listaAdyacencia)
            {
                Console.Write($"{edificio.Key}:");
                foreach (var conexion in edificio.Value)
                {
                    Console.Write($" -> {conexion.Item1} [{conexion.Item2}m]");
                }
                Console.WriteLine();
            }
        }
        public void RecorridoBFS(string inicio)
        {
            if (!listaAdyacencia.ContainsKey(inicio))
            {
                Console.WriteLine($"El edificio {inicio} no existe en el grafo.");
                return;
            }

            Queue<string> cola = new Queue<string>();
            Dictionary<string, int> niveles = new Dictionary<string, int>();
            List<string> ordenVisita = new List<string>();

            cola.Enqueue(inicio);
            niveles[inicio] = 0;
            ordenVisita.Add(inicio);

            Console.WriteLine($"\n=== RECORRIDO BFS desde: {inicio} ===");

            while (cola.Count > 0)
            {
                string actual = cola.Dequeue();

                foreach (var conexion in listaAdyacencia[actual])
                {
                    string vecino = conexion.Item1;
                    if (!niveles.ContainsKey(vecino))
                    {
                        niveles[vecino] = niveles[actual] + 1;
                        cola.Enqueue(vecino);
                        ordenVisita.Add(vecino);
                    }
                }
            }

            // Agrupar e imprimir por niveles
            Dictionary<int, List<string>> edificiosPorNivel = new Dictionary<int, List<string>>();
            foreach (var kvp in niveles)
            {
                if (!edificiosPorNivel.ContainsKey(kvp.Value))
                {
                    edificiosPorNivel[kvp.Value] = new List<string>();
                }
                edificiosPorNivel[kvp.Value].Add(kvp.Key);
            }

            foreach (var nivel in edificiosPorNivel)
            {
                Console.WriteLine($"Nivel {nivel.Key}: {string.Join(" | ", nivel.Value)}");
            }

            Console.WriteLine($"Total edificios visitados: {ordenVisita.Count}");
        }
        public void RecorridoDFS(string inicio, string destino)
        {
            if (!listaAdyacencia.ContainsKey(inicio) || !listaAdyacencia.ContainsKey(destino))
            {
                Console.WriteLine("El origen o el destino no existen en el grafo.");
                return;
            }

            Console.WriteLine($"\n=== RECORRIDO DFS: {inicio} --> {destino} ===");

            Stack<string> pila = new Stack<string>();
            HashSet<string> visitados = new HashSet<string>();
            Dictionary<string, string> padres = new Dictionary<string, string>();

            pila.Push(inicio);
            bool encontrado = false;

            while (pila.Count > 0)
            {
                string actual = pila.Pop();

                if (!visitados.Contains(actual))
                {
                    Console.WriteLine($"Visitando: {actual}");
                    visitados.Add(actual);

                    if (actual == destino)
                    {
                        encontrado = true;
                        break;
                    }

                    List<Tuple<string, int>> vecinos = listaAdyacencia[actual];
                    for (int i = vecinos.Count - 1; i >= 0; i--)
                    {
                        string vecino = vecinos[i].Item1;
                        if (!visitados.Contains(vecino))
                        {
                            pila.Push(vecino);
                            if (!padres.ContainsKey(vecino))
                            {
                                padres[vecino] = actual;
                            }
                        }
                    }
                }
            }

            if (encontrado)
            {
                List<string> camino = new List<string>();
                string nodoCamino = destino;

                while (nodoCamino != inicio)
                {
                    camino.Add(nodoCamino);
                    nodoCamino = padres[nodoCamino];
                }
                camino.Add(inicio);
                camino.Reverse();

                Console.WriteLine($"✓ Camino encontrado: {string.Join(" -> ", camino)}");

                
                int distanciaTotal = 0;
                for (int i = 0; i < camino.Count - 1; i++)
                {
                    string nodo1 = camino[i];
                    string nodo2 = camino[i + 1];

                    foreach (var conexion in listaAdyacencia[nodo1])
                    {
                        if (conexion.Item1 == nodo2)
                        {
                            distanciaTotal += conexion.Item2;
                            break;
                        }
                    }
                }
                Console.WriteLine($"Distancia total del camino: {distanciaTotal} metros");
            }
            else
            {
                Console.WriteLine("Camino no encontrado.");
            }
        }
    }
}
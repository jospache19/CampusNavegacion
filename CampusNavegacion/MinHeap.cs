using System;
using System.Collections.Generic;

namespace CampusNavegacion
{
    public class MinHeap
    {
        private List<Tuple<string, int>> elementos;

        public MinHeap()
        {
            elementos = new List<Tuple<string, int>>();
        }

        public void Insertar(string edificio, int distancia)
        {
            elementos.Add(new Tuple<string, int>(edificio, distancia));
            Flotar(elementos.Count - 1);
        }

        public Tuple<string, int> ExtraerMinimo()
        {
            if (EstaVacio())
            {
                throw new InvalidOperationException("El heap está vacío.");
            }

            Tuple<string, int> minimo = elementos[0];
            // Reemplazar la raíz con el último elemento
            elementos[0] = elementos[elementos.Count - 1];
            elementos.RemoveAt(elementos.Count - 1);

            if (!EstaVacio())
            {
                Hundir(0); // Reacomodar el montículo
            }

            return minimo;
        }

        public bool EstaVacio()
        {
            return elementos.Count == 0;
        }

        public void MostrarRutasOrdenadas()
        {
            Console.WriteLine("\n=== RUTAS Ordenadas por distancia ===");

            if (EstaVacio())
            {
                Console.WriteLine("Heap vacío");
                return;
            }

            int posicion = 1;
            while (!EstaVacio())
            {
                var ruta = ExtraerMinimo();
                Console.WriteLine($"{posicion}° {ruta.Item1,-25} {ruta.Item2} metros");
                posicion++;
            }
            Console.WriteLine("Todas las rutas fueron procesadas.");
        }

        // --- Métodos auxiliares privados para mantener la propiedad del Min-Heap ---

        private void Flotar(int indice)
        {
            int indicePadre = (indice - 1) / 2;

            // Mientras no sea la raíz y el hijo sea menor que el padre
            while (indice > 0 && elementos[indice].Item2 < elementos[indicePadre].Item2)
            {
                Intercambiar(indice, indicePadre);
                indice = indicePadre;
                indicePadre = (indice - 1) / 2;
            }
        }

        private void Hundir(int indice)
        {
            int indiceHijoIzquierdo = 2 * indice + 1;
            int indiceHijoDerecho = 2 * indice + 2;
            int indiceMenor = indice;

            if (indiceHijoIzquierdo < elementos.Count && elementos[indiceHijoIzquierdo].Item2 < elementos[indiceMenor].Item2)
            {
                indiceMenor = indiceHijoIzquierdo;
            }

            if (indiceHijoDerecho < elementos.Count && elementos[indiceHijoDerecho].Item2 < elementos[indiceMenor].Item2)
            {
                indiceMenor = indiceHijoDerecho;
            }

            if (indiceMenor != indice)
            {
                Intercambiar(indice, indiceMenor);
                Hundir(indiceMenor);
            }
        }

        private void Intercambiar(int indiceA, int indiceB)
        {
            var temp = elementos[indiceA];
            elementos[indiceA] = elementos[indiceB];
            elementos[indiceB] = temp;
        }
    }
}
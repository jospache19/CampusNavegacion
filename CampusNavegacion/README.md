# Sistema de Navegación del Campus Universitario (SNCU)

Este repositorio contiene el proyecto desarrollado para el Lab de PED Grupo 04L
## Datos del Estudiante
* **Desarrollador:** Josué Emmanuel Pacheco Hernández
* **Carné:** PH250409
* **Carrera:** Ingeniería en Ciencias de la Computación
* **Universidad:** Universidad Don Bosco
* **Catedrático:** Ing. Rafael Torres

## Estructuras de Datos Utilizadas

Para hacer funcionar el mapa y sus algoritmos, se implementaron las siguientes estructuras desde cero:

* **Grafo (bidireccional):**Conecta los 7 edificios principales mediante caminos bidireccionales y almacena la distancia en metros de cada ruta física.
* **Búsqueda en Anchura (BFS):** recorre el campus por capas, calculando a cuántos saltos de distancia se encuentran los demás edificios respecto al origen.
* **Búsqueda en Profundidad (DFS):** busca y traza un camino específico entre un origen y un destino determinado.
* **Tabla Hash:** funciona como una bitácora estadística que registra y suma cada visita a los edificios durante los recorridos.
* **Min-Heap (Mínimo):** Estructura creada para almacenar rutas. Su objetivo es ordenar y extraer automáticamente los caminos de menor a mayor distancia.
lementado:

* **Video explicativo:** 
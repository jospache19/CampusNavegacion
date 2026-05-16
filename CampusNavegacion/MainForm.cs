using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CampusNavegacion
{
    public partial class MainForm : Form
    {
        // Estructuras lógicas
        private Grafo grafo;
        private TablaHash tablaVisitas;
        private MinHeap heapRutas;

        // Variables para renderizado visual
        private Dictionary<string, Point> coordenadas;
        private List<string> rutaActiva;
        private string origenActual;
        private string destinoActual;

        public MainForm()
        {
            InitializeComponent();

            // Activar DoubleBuffer para evitar parpadeos al redibujar
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, panelMapa, new object[] { true });

            InicializarDatos();
        }

        private void InicializarDatos()
        {
            grafo = new Grafo();
            tablaVisitas = new TablaHash();
            heapRutas = new MinHeap();
            rutaActiva = new List<string>();
            origenActual = "";
            destinoActual = "";

            // 1. Mapeo de coordenadas físicas en el Panel (ajusta X e Y según el tamaño de tu panel)
            coordenadas = new Dictionary<string, Point>
            {
                { "Biblioteca Central (A)", new Point(100, 250) },
                { "Cafeteria (B)", new Point(300, 250) },
                { "Laboratorio de Computo (C)", new Point(100, 100) },
                { "Rectoria (D)", new Point(500, 250) },
                { "Gimnasio (E)", new Point(300, 450) },
                { "Aulas Generales (F)", new Point(500, 100) },
                { "Estacionamiento (G)", new Point(700, 350) }
            };

            // 2. Llenar ComboBoxes
            foreach (var edificio in coordenadas.Keys)
            {
                cmbOrigen.Items.Add(edificio);
                cmbDestino.Items.Add(edificio);
            }
            if (cmbOrigen.Items.Count > 0) cmbOrigen.SelectedIndex = 0;
            if (cmbDestino.Items.Count > 0) cmbDestino.SelectedIndex = cmbDestino.Items.Count - 1;

            // 3. Cargar datos base del grafo
            grafo.AgregarCamino("Biblioteca Central (A)", "Cafeteria (B)", 120);
            grafo.AgregarCamino("Biblioteca Central (A)", "Laboratorio de Computo (C)", 200);
            grafo.AgregarCamino("Cafeteria (B)", "Rectoria (D)", 150);
            grafo.AgregarCamino("Cafeteria (B)", "Gimnasio (E)", 300);
            grafo.AgregarCamino("Laboratorio de Computo (C)", "Aulas Generales (F)", 100);
            grafo.AgregarCamino("Rectoria (D)", "Aulas Generales (F)", 80);
            grafo.AgregarCamino("Gimnasio (E)", "Estacionamiento (G)", 250);
            grafo.AgregarCamino("Aulas Generales (F)", "Estacionamiento (G)", 180);

            // Preparar el MinHeap (Tarea 5) internamente para cuando se presione el botón
            heapRutas.Insertar("Cafeteria (B)", 120);
            heapRutas.Insertar("Laboratorio de Computo (C)", 200);
            Console.SetOut(new TextBoxWriter(txtResultados));
        }

        private void panelMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Bordes suaves

            Pen penCaminoNormal = new Pen(Color.LightGray, 3);
            Pen penCaminoActivo = new Pen(Color.MediumSeaGreen, 5);
            Brush brushNodoNormal = Brushes.SteelBlue;
            Brush brushNodoOrigen = Brushes.Purple;
            Brush brushNodoDestino = Brushes.Crimson;
            Brush brushNodoVisitado = Brushes.DarkOrange;

            int radio = 25;

            // 1. DIBUJAR LÍNEAS (Caminos)
            // Aquí definimos manualmente las conexiones físicas para dibujarlas de una vez
            // 1. DIBUJAR LÍNEAS (Caminos y Distancias)
            // Agregamos la distancia al final de la tupla (Origen, Destino, Distancia)
            var aristasFisicas = new List<Tuple<string, string, int>>
            {
                Tuple.Create("Biblioteca Central (A)", "Cafeteria (B)", 120),
                Tuple.Create("Biblioteca Central (A)", "Laboratorio de Computo (C)", 200),
                Tuple.Create("Cafeteria (B)", "Rectoria (D)", 150),
                Tuple.Create("Cafeteria (B)", "Gimnasio (E)", 300),
                Tuple.Create("Laboratorio de Computo (C)", "Aulas Generales (F)", 100),
                Tuple.Create("Rectoria (D)", "Aulas Generales (F)", 80),
                Tuple.Create("Gimnasio (E)", "Estacionamiento (G)", 250),
                Tuple.Create("Aulas Generales (F)", "Estacionamiento (G)", 180)
            };

            foreach (var arista in aristasFisicas)
            {
                Point p1 = coordenadas[arista.Item1];
                Point p2 = coordenadas[arista.Item2];

                // Verificar si esta arista forma parte de la ruta activa (DFS)
                bool esRutaActiva = false;
                for (int i = 0; i < rutaActiva.Count - 1; i++)
                {
                    if ((rutaActiva[i] == arista.Item1 && rutaActiva[i + 1] == arista.Item2) ||
                        (rutaActiva[i] == arista.Item2 && rutaActiva[i + 1] == arista.Item1))
                    {
                        esRutaActiva = true;
                        break;
                    }
                }

                g.DrawLine(esRutaActiva ? penCaminoActivo : penCaminoNormal, p1.X, p1.Y, p2.X, p2.Y);

                
                // Calcular el punto medio de la línea
                int midX = (p1.X + p2.X) / 2;
                int midY = (p1.Y + p2.Y) / 2;

                // Dibujar un pequeño fondo blanco para que el texto sea legible
                string textoDistancia = $"{arista.Item3}m";
                SizeF tamanoTexto = g.MeasureString(textoDistancia, new Font("Arial", 9));
                g.FillRectangle(Brushes.White, midX - (tamanoTexto.Width / 2), midY - (tamanoTexto.Height / 2), tamanoTexto.Width, tamanoTexto.Height);

                // Dibujar el texto de la distancia
                g.DrawString(textoDistancia, new Font("Arial", 9), Brushes.DimGray, midX - (tamanoTexto.Width / 2), midY - (tamanoTexto.Height / 2));
            }

            // 2. DIBUJAR NODOS (Edificios)
            foreach (var nodo in coordenadas)
            {
                Point p = nodo.Value;
                Brush colorActual = brushNodoNormal;

                // Lógica de colores según el estado visual
                if (nodo.Key == origenActual) colorActual = brushNodoOrigen;
                else if (nodo.Key == destinoActual) colorActual = brushNodoDestino;
                else if (rutaActiva.Contains(nodo.Key)) colorActual = brushNodoVisitado;

                // Dibujar círculo centrado
                g.FillEllipse(colorActual, p.X - radio, p.Y - radio, radio * 2, radio * 2);

                // Extraer solo la letra (Ej: "A") para dibujarla dentro del nodo
                string letra = nodo.Key.Substring(nodo.Key.Length - 2, 1);
                g.DrawString(letra, new Font("Arial", 12, FontStyle.Bold), Brushes.White, p.X - 8, p.Y - 8);

                // Dibujar el nombre completo abajo del nodo
                g.DrawString(nodo.Key, new Font("Arial", 8), Brushes.Black, p.X - radio - 10, p.Y + radio + 5);
            }
        }

        private void btnMostrarGrafo_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();
            grafo.MostrarGrafo();
        }

        private void btnBFS_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();
            if (cmbOrigen.SelectedItem == null) return;

            origenActual = cmbOrigen.SelectedItem.ToString();
            destinoActual = "";
            rutaActiva.Clear();

            grafo.RecorridoBFS(origenActual);

            // Registramos visita del origen en la tabla hash para mantener estadísticas
            tablaVisitas.RegistrarVisita(origenActual);

            panelMapa.Invalidate(); // Repinta el mapa
        }

        private void btnDFS_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();
            if (cmbOrigen.SelectedItem == null || cmbDestino.SelectedItem == null) return;

            origenActual = cmbOrigen.SelectedItem.ToString();
            destinoActual = cmbDestino.SelectedItem.ToString();
            rutaActiva.Clear();

            grafo.RecorridoDFS(origenActual, destinoActual);

            // Leer la consola redirigida para extraer el camino exacto y pintarlo
            string texto = txtResultados.Text;
            if (texto.Contains("✓ Camino encontrado:"))
            {
                int index = texto.IndexOf("✓ Camino encontrado:") + "✓ Camino encontrado:".Length;
                string caminoStr = texto.Substring(index).Trim();
                string[] nodos = caminoStr.Split(new string[] { "->" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var n in nodos)
                {
                    rutaActiva.Add(n.Trim());
                }
            }

            // Registramos visitas en la tabla hash
            tablaVisitas.RegistrarVisita(origenActual);
            tablaVisitas.RegistrarVisita(destinoActual);

            panelMapa.Invalidate(); // Repinta el mapa con la ruta
        }

        private void btnTablaHash_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();
            tablaVisitas.MostrarEstadisticas();
        }

        private void btnMinHeap_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();

            // El Heap se vacía al extraer los elementos, se reinsertan para la demostración visual
            heapRutas = new MinHeap();
            heapRutas.Insertar("Cafeteria (B)", 120);
            heapRutas.Insertar("Laboratorio de Computo (C)", 200);

            heapRutas.MostrarRutasOrdenadas();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            txtResultados.Clear();
            origenActual = "";
            destinoActual = "";
            rutaActiva.Clear();

            // Reiniciar la tabla hash a cero
            tablaVisitas = new TablaHash();

            panelMapa.Invalidate();
        }
    }
    public class TextBoxWriter : System.IO.TextWriter
    {
        private RichTextBox rtb;
        public TextBoxWriter(RichTextBox richTextBox) { rtb = richTextBox; }
        public override void Write(char value) { rtb.AppendText(value.ToString()); }
        public override void Write(string value) { rtb.AppendText(value); }
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
    }
}

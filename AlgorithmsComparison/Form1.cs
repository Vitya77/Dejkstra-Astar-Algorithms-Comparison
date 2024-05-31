using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace AlgorithmsComparison
{
    public partial class Form1 : Form
    {
        private Graph.Graph currentGraph;
        private Thread dejkstraThread;
        private Thread aStarThread;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox_Paint;
            pictureBox2.Paint += PictureBox_Paint;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int numberOfVertices = int.Parse(textBox1.Text);
            int numberOfEdges = int.Parse(textBox2.Text);
            currentGraph = new Graph.Graph(numberOfVertices, numberOfEdges, pictureBox1.Height, pictureBox1.Width);
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            button2.Visible = true;
            label2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (currentGraph != null)
            {
                DrawGraph(e.Graphics, currentGraph);
            }
        }

        private void DrawGraph(Graphics graphics, Graph.Graph graph)
        {
            graphics.Clear(Color.White);
            int numberOfVertices = graph.vertices.Length;

            // Малювання ребер
            Pen edgePen = new Pen(Color.Black);
            for (int i = 0; i < numberOfVertices; i++)
            {
                for (int j = 0; j < numberOfVertices; j++)
                {
                    if (graph.adjacencyMatrix[i, j] > 0)
                    {
                        graphics.DrawLine(edgePen, graph.vertices[i], graph.vertices[j]);
                    }
                }
            }

            // Малювання вершин
            Brush vertexBrush = new SolidBrush(Color.Red);
            Font font = new Font("Arial", 9);
            Brush textBrush = new SolidBrush(Color.White);
            for (int i = 0; i < graph.vertices.Length; i++)
            {
                graphics.FillEllipse(vertexBrush, graph.vertices[i].X - 7, graph.vertices[i].Y - 7, 14, 14);

                // Малювання номера вершини
                string vertexNumber = i.ToString();
                SizeF stringSize = graphics.MeasureString(vertexNumber, font);
                graphics.DrawString(vertexNumber, font, textBrush, graph.vertices[i].X - stringSize.Width / 2, graph.vertices[i].Y - stringSize.Height / 2);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentGraph != null && int.TryParse(textBox4.Text, out int startVertex) && int.TryParse(textBox3.Text, out int endVertex))
            {
                if (dejkstraThread == null || !dejkstraThread.IsAlive)
                {
                    dejkstraThread = new Thread(() => RunDejkstra(startVertex, endVertex));
                    dejkstraThread.Start();
                }
                if (aStarThread == null || !aStarThread.IsAlive)
                {
                    aStarThread = new Thread(() => RunAStar(startVertex, endVertex));
                    aStarThread.Start();
                }
            }
        }
        private void RunAStar(int startVertex, int endVertex)
        {
            DrawGraph(pictureBox1.CreateGraphics(), currentGraph);
            try
            {
                currentGraph.AstarPathsChanged += AstarPathUpdate;
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                var paths = currentGraph.AStar(startVertex, endVertex);
                stopwatch.Stop();

                this.Invoke((MethodInvoker)delegate {
                    label4.Text = $"Час виконання алгоритму: {stopwatch.Elapsed}";
                    label4.Visible = true;
                });

                Pen pathPen = new Pen(Color.Green, 4);
                DrawPath(pictureBox1.CreateGraphics(), currentGraph, paths[endVertex], pathPen);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RunDejkstra(int startVertex, int endVertex)
        {
            DrawGraph(pictureBox2.CreateGraphics(), currentGraph);
            try
            {
                currentGraph.DejkstraPathsChanged += DejkstraPathUpdate;
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                var paths = currentGraph.Dejkstra(startVertex, endVertex);
                stopwatch.Stop();

                this.Invoke((MethodInvoker)delegate {
                    label3.Text = $"Час виконання алгоритму: {stopwatch.Elapsed}";
                    label3.Visible = true;
                });

                Pen pathPen = new Pen(Color.Green, 4);
                DrawPath(pictureBox2.CreateGraphics(), currentGraph, paths[endVertex], pathPen);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DejkstraPathUpdate(List<int> newPath)
        {
            Pen pathPen = new Pen(Color.Blue, 2);

            if (InvokeRequired)
            {
                this.Invoke(new Action(() => DrawPath(pictureBox2.CreateGraphics(), currentGraph, newPath, pathPen)));
            }
            else
            {
                DrawPath(pictureBox2.CreateGraphics(), currentGraph, newPath, pathPen);
            }
        }

        private void AstarPathUpdate(List<int> newPath)
        {
            Pen pathPen = new Pen(Color.Blue, 2);

            if (InvokeRequired)
            {
                this.Invoke(new Action(() => DrawPath(pictureBox1.CreateGraphics(), currentGraph, newPath, pathPen)));
            }
            else
            {
                DrawPath(pictureBox1.CreateGraphics(), currentGraph, newPath, pathPen);
            }
        }

        private void DrawPath(Graphics graphics, Graph.Graph graph, List<int> path, Pen pathPen)
        {
            if (path == null) return;

            
            
            for (int i = 0; i < path.Count - 1; i++)
            {
                int start = path[i];
                int end = path[i + 1];
                graphics.DrawLine(pathPen, graph.vertices[start], graph.vertices[end]);
            }
        }
    }
}

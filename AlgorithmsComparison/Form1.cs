using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlgorithmsComparison
{
    public partial class Form1 : Form
    {
        private Graph.Graph currentGraph;
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
    }
}

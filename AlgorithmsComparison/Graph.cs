using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graph
{ 
    public class Graph
    {
        public readonly double[,] adjacencyMatrix;
        public readonly Point[] vertices;

        public Graph(int numberOfVertices, int numberOfEdges, int height, int width)
        {
            if (numberOfEdges > numberOfVertices * (numberOfVertices - 1) / 2)
            {
                throw new ArgumentOutOfRangeException("Кількість ребер не може перевищувати максимальну n*(n-1)/2, де n - кількість вершин");
            }

            Point[] Vertices = new Point[numberOfVertices];

            Random random = new Random();

            for (int i = 0; i < numberOfVertices; i++)
            {
                Vertices[i] = new Point(random.Next(width), random.Next(height));
            }

            vertices = Vertices;

            double[,] AdjacencyMatrix = new double[numberOfVertices, numberOfVertices];

            for (int i = 0; i < numberOfEdges; i++)
            {
                int source = random.Next(numberOfVertices);
                int destination = -1;
                while (destination == -1 || destination == source)
                {
                    destination = random.Next(numberOfVertices);
                }

                if (AdjacencyMatrix[source, destination] == 0)
                {
                    AdjacencyMatrix[source, destination] = Math.Sqrt(Math.Pow(vertices[source].X - vertices[destination].X, 2) + Math.Pow(vertices[source].Y - vertices[destination].Y, 2));
                }
                else
                {
                    i--;
                }
            }

            adjacencyMatrix = AdjacencyMatrix;
        }
    }

}
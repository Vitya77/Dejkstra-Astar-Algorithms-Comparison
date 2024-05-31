using System;
using System.Collections.Generic;
using System.Drawing;

namespace Graph
{
    public class PriorityQueue<T>
    {
        private List<(T item, double priority)> elements = new List<(T, double)>();

        public int Count => elements.Count;

        public void Enqueue(T item, double priority)
        {
            elements.Add((item, priority));
        }

        public T Dequeue()
        {
            int bestIndex = 0;
            for (int i = 1; i < elements.Count; i++)
            {
                if (elements[i].priority < elements[bestIndex].priority)
                {
                    bestIndex = i;
                }
            }

            T bestItem = elements[bestIndex].item;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    public class Graph
    {
        public readonly double[,] adjacencyMatrix;
        public readonly Point[] vertices;
        public delegate void DejkstraPathsChangedHandler(List<int> newPath);
        public event DejkstraPathsChangedHandler DejkstraPathsChanged;

        public delegate void AstarPathsChangedHandler(List<int> newPath);
        public event AstarPathsChangedHandler AstarPathsChanged;

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
                    AdjacencyMatrix[destination, source] = AdjacencyMatrix[source, destination];
                }
                else
                {
                    i--;
                }
            }

            adjacencyMatrix = AdjacencyMatrix;
        }

        public Graph(int height, int width)
        {
            List<Point> Vertices = new List<Point>();

            int x = 5;
            int y = 5;
            int rows = 0;
            int cols = 0;
            while (y < height)
            {
                rows = 0;
                while (x + 20 < width)
                {
                    Vertices.Add(new Point(x, y));
                    x = x + 20;
                    rows++;
                }
                x = 5;
                y += 20;
                cols++;
            }

            vertices = Vertices.ToArray();

            double[,] AdjacencyMatrix = new double[cols*rows, cols*rows];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int sourceIndex = i * rows + j;

                    // Connect with right neighbor
                    if (j + 1 < rows)
                    {
                        int destinationIndex = i * rows + (j + 1);
                        double distance = Math.Sqrt(Math.Pow(vertices[sourceIndex].X - vertices[destinationIndex].X, 2) + Math.Pow(vertices[sourceIndex].Y - vertices[destinationIndex].Y, 2));
                        AdjacencyMatrix[sourceIndex, destinationIndex] = distance;
                        AdjacencyMatrix[destinationIndex, sourceIndex] = distance;
                    }

                    // Connect with bottom neighbor
                    if (i + 1 < cols)
                    {
                        int destinationIndex = (i + 1) * rows + j;
                        double distance = Math.Sqrt(Math.Pow(vertices[sourceIndex].X - vertices[destinationIndex].X, 2) + Math.Pow(vertices[sourceIndex].Y - vertices[destinationIndex].Y, 2));
                        AdjacencyMatrix[sourceIndex, destinationIndex] = distance;
                        AdjacencyMatrix[destinationIndex, sourceIndex] = distance;
                    }
                }
            }

            adjacencyMatrix = AdjacencyMatrix;
        }

        public List<List<int>> Dejkstra(int source, int destination)
        {
            int n = adjacencyMatrix.GetLength(0);
            List<List<int>> paths = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                paths.Add(new List<int>());
            }

            paths[source].Add(source);

            double[] distances = new double[n];
            bool[] visitedVertices = new bool[n];

            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;
                //Відмічаємо всі вершини, до яких немає доріг як пройдені, а інші як непройдені
                visitedVertices[i] = true;
                for (int j = 0; j < n; j++)
                {
                    if (adjacencyMatrix[i, j] != 0)
                    {
                        visitedVertices[i] = false;
                        break;
                    }
                }
            }

            distances[source] = 0;

            for (int i = 0; i < n - 1; i++)
            {
                double minDistance = double.MaxValue;
                int minIndex = -1;

                // Знайти найближчий непройдений вузол
                for (int j = 0; j < n; j++)
                {
                    if (!visitedVertices[j] && distances[j] < minDistance)
                    {
                        minDistance = distances[j];
                        minIndex = j;
                    }
                }

                if (minIndex != -1)
                {
                    visitedVertices[minIndex] = true;

                    // Оновити відстані до сусідніх вузлів
                    for (int k = 0; k < n; k++)
                    {
                        if (!visitedVertices[k] && adjacencyMatrix[minIndex, k] != 0 &&
                             distances[minIndex] + adjacencyMatrix[minIndex, k] < distances[k])
                        {
                            distances[k] = distances[minIndex] + adjacencyMatrix[minIndex, k];
                            paths[k] = new List<int>(paths[minIndex]);
                            paths[k].Add(k);

                            DejkstraPathsChanged?.Invoke(paths[k]);

                            if (k == destination)
                            {
                                return paths;
                            }
                        }
                    }
                }
            }

            return paths;
        }

        public List<List<int>> AStar(int source, int destination)
        {
            int n = adjacencyMatrix.GetLength(0);
            List<List<int>> paths = new List<List<int>>();
            for (int i = 0; i < n; i++)
            {
                paths.Add(new List<int>());
            }

            paths[source].Add(source);

            // Ініціалізуємо відстані та оцінки відстаней
            double[] distances = new double[n];
            double[] estimates = new double[n];
            for (int i = 0; i < n; i++)
            {
                distances[i] = double.MaxValue;
                estimates[i] = double.MaxValue;
            }

            distances[source] = 0;
            estimates[source] = Heuristic(source, destination); // Використовуємо евристику для оцінки відстані

            PriorityQueue<int> queue = new PriorityQueue<int>(); // Пріоритетна черга для вузлів

            queue.Enqueue(source, estimates[source]);

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                if (current == destination)
                {
                    return paths;
                }

                // Перебираємо сусідів поточного вузла
                for (int neighbor = 0; neighbor < n; neighbor++)
                {
                    if (adjacencyMatrix[current, neighbor] != 0)
                    {
                        double tentativeDistance = distances[current] + adjacencyMatrix[current, neighbor];
                        if (tentativeDistance < distances[neighbor])
                        {
                            distances[neighbor] = tentativeDistance;
                            estimates[neighbor] = tentativeDistance + Heuristic(neighbor, destination); // Оновлюємо оцінку відстані

                            paths[neighbor] = new List<int>(paths[current]);
                            paths[neighbor].Add(neighbor);

                            queue.Enqueue(neighbor, estimates[neighbor]);

                            AstarPathsChanged?.Invoke(paths[neighbor]);
                        }
                    }
                }
            }

            return paths;
        }

        private double Heuristic(int node, int destination)
        {
            Point nodePoint = vertices[node];
            Point destPoint = vertices[destination];

            // Евклідова відстань між двома точками
            double dx = nodePoint.X - destPoint.X;
            double dy = nodePoint.Y - destPoint.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }

}
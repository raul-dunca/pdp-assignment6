using System;
using System.Collections.Generic;

public class Graph
{
    private int nr_vertices;
    private int nr_edges;
    HashSet<(int, int)> edges;

    public Graph(int v,int e)
    {
        nr_vertices = v;
        nr_edges = e;
        edges = new HashSet<(int, int)>();
    }
    public int NumberOfVertices => nr_vertices;

    public int NumberOfEdges => nr_edges;

    public HashSet<(int, int)> Edges => edges;

    public bool EdgeExists(int x, int y)
    {
        return edges.Contains((x, y));
    }
    public void AddEdge(int x, int y)
    {
        if (x < 0 || x >= nr_vertices || y < 0 || y >= nr_vertices)
        {
            Console.WriteLine("Vertices out of range");
        }
        else
        {
            edges.Add((x, y));
        }
    }

    public void GenerateRandomGraph()
    {
        if (nr_edges > nr_vertices * (nr_vertices - 1))
        {
            Console.WriteLine("Number of edges exceeds the maximum possible for this graph");

        }
        else
        {

            Random rand = new Random();

            while (edges.Count < nr_edges)
            {
                int x = rand.Next(nr_vertices);
                int y = rand.Next(nr_vertices);

                if (!edges.Contains((x, y)) && x!=y)
                {
                    AddEdge(x, y);
                }
            }
        }
    }

    public void DisplayGraph()
    {
        Console.WriteLine("Directed Edges in the Graph:");
        foreach (var edge in edges)
        {
            Console.WriteLine($"{edge.Item1} {edge.Item2}");
        }
    }
}




public class HamiltonianCycle
{
    private Graph graph;
    int[] path;
    bool cycleFound;
    private Mutex mutex;

    public HamiltonianCycle(Graph g)
    {
        cycleFound = false;
        mutex= new Mutex();
        graph = g;
        path = new int[graph.NumberOfVertices];
        for (int i = 0; i < graph.NumberOfVertices; i++)
            path[i] = -1;
    }
    bool isSafe(int v,
                int[] path, int pos)
    {
        if (!graph.EdgeExists(path[pos-1],v))
        {
            return false;
        }
 
        for (int i = 0; i < pos; i++)
            if (path[i] == v)
                return false;

        return true;
    }

    void hamCycleUtil(int[] path, int pos,int depth)
    {
        if (pos == graph.NumberOfVertices)
        {
            if (graph.EdgeExists(path[pos - 1], path[0]))
            {
                mutex.WaitOne();
                cycleFound = true;
                printSolution(path);
                mutex.ReleaseMutex();
            }
            return;
        }

        if (depth < 3)
        {
            List<Task> tasks = new List<Task>();
            for (int v = 1; v < graph.NumberOfVertices; v++)
            {
                if (isSafe(v, path, pos))
                {
                    int vertex = v;
                    if (!cycleFound && isSafe(vertex, path, pos))
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            int[] newPath = (int[])path.Clone();
                            newPath[pos] = vertex;
                            hamCycleUtil(newPath, pos + 1, depth + 1);
                        }));
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
        else
        {
            for (int v = 1; v < graph.NumberOfVertices; v++)
            {
                if (isSafe(v, path, pos))
                {
                    int vertex = v;
                    if (!cycleFound && isSafe(vertex, path, pos))
                    {
                            int[] newPath = (int[])path.Clone();
                            newPath[pos] = vertex;
                            hamCycleUtil(newPath, pos + 1, depth + 1);
                        
                    }
                }
            }
        }

        
    }

    public void hamCycle()
    {
        
        path[0] = 0;
        hamCycleUtil(path, 1,0);

        if (!cycleFound)
        {
            Console.WriteLine("\nSolution does not exist");
        }

    }

    void printSolution(int[] path)
    {
        Console.WriteLine("Solution Exists: Following" +
                        " is one Hamiltonian Cycle");
        for (int i = 0; i < graph.NumberOfVertices; i++)
            Console.Write(path[i] + " ");

        Console.WriteLine(path[0]);
    }
}

public static class Program
{
    public static void Main()
    {
        int numVertices = 5; 
        int numEdges = 11; 

        Graph graph = new Graph(numVertices,numEdges);
        graph.GenerateRandomGraph();
        graph.DisplayGraph();

        HamiltonianCycle hamiltonian= new HamiltonianCycle(graph);
        
        hamiltonian.hamCycle();

    }
}

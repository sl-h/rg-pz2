using System;
using System.Collections.Generic;
using WpfApp1.Model;
using WpfApp1;
using System.Linq;
using System.Diagnostics;

public class Path
{
    public long lineEntityId;
    public List<EntitySpot> spots = new List<EntitySpot>();
}

public class BFSLineIterator
{
    BFSAlgorithm algorithm = new BFSAlgorithm();
    List<Path> paths = new List<Path>();

    public List<Path> FindPaths(Dictionary<long, EntitySpot> entities, List<List<EntitySpot>> matrix, List<LineEntity> lines)
    {
        int cnt = 0;
        foreach (var item in lines)
        {
            if (entities.TryGetValue(item.FirstEnd, out EntitySpot start) && entities.TryGetValue(item.SecondEnd, out EntitySpot end))
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    for (int j = 0; j < matrix[i].Count; j++)
                    {
                        matrix[i][j].IsVisited = false;
                        matrix[i][j].parent = null;
                    }
                }

                var path = algorithm.FindPath(start, end, matrix);
                path.lineEntityId = item.Id;
                paths.Add(path);

                if (paths.Count > 0)
                    if (paths.Last().spots.Count > 0)
                        cnt++;
                //break;
            }
        }
        Console.WriteLine(cnt);
        return paths;
    }
}




public class BFSAlgorithm
{

    Path GetPathFromNodeParents(EntitySpot v)
    {
        Path path = new Path();

        path.spots.Add(v);
        while (v.parent != null)
        {
            path.spots.Add(v.parent);
            v = v.parent;
        }
        return path;
    }

    public Path FindPath(EntitySpot start, EntitySpot end, List<List<EntitySpot>> matrix)
    {
        int ofst = 0;// MainWindow.pointOffset;

        Queue<EntitySpot> queue = new Queue<EntitySpot>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var v = queue.Dequeue();
            v.IsVisited = true;
            int startX = v.X / MainWindow.fieldSize;
            int startY = v.Y / MainWindow.fieldSize;

            if (v == end)
            {
                return GetPathFromNodeParents(v);
            }


            if (startX + ofst + 1 < 400)
            {
                var n = matrix[startX + ofst + 1][startY];
                if (n.IsOccupied == false && n.IsVisited == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }
            if (startX + ofst - 1 >= 0 && startX + ofst - 1 < 400)
            {
                var n = matrix[startX + ofst - 1][startY];
                if (n.IsOccupied == false && n.IsVisited == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }

            if (startY + ofst + 1 < 400)
            {
                var n = matrix[startX][startY + ofst + 1];

                if (n.IsOccupied == false && n.IsVisited == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }
            if (startY + ofst - 1 >= 0)
            {
                var n = matrix[startX][startY + ofst - 1];
                if (n.IsOccupied == false && n.IsVisited == false)
                {
                    n.IsVisited = true;
                    n.parent = v;
                    queue.Enqueue(n);
                }
            }
        }
        return new Path();
    }
}
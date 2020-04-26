using System;
using System.Collections.Generic;
using WpfApp1.Model;
using WpfApp1;
using System.Linq;
using System.Diagnostics;

public class Path
{
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
                paths.Add(algorithm.FindPath(start, end, matrix));

                //if (paths.Last().spots.Count > 0)
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
    public Path FindPath(EntitySpot start, EntitySpot end, List<List<EntitySpot>> matrix)
    {
        Path path = new Path();


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
                path.spots.Add(v);
                while (v.parent != null)
                {
                    path.spots.Add(v.parent);
                    v = v.parent;

                }
                return path;

            }


            if (startX + ofst + 1 < 400)
            {
                var n = matrix[startX + ofst + 1][startY];
                // if (n.Entities.Count == 0 || n == end)

                if (n.IsOccupied == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }
            if (startX + ofst - 1 >= 0 && startX + ofst - 1 < 400)
            {

                var n = matrix[startX + ofst - 1][startY];
                //  if (n.Entities.Count == 0 || n == end)
                if (n.IsOccupied == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }

            if (startY + ofst + 1 < 400)
            {
                var n = matrix[startX][startY + ofst + 1];
                // if (n.Entities.Count == 0 || n == end)

                if (n.IsOccupied == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = v;

                    queue.Enqueue(n);
                }
            }
            if (startY + ofst - 1 >= 0 && startY + ofst - 1 < 400)
            {

                var n = matrix[startX][startY + ofst - 1];
                //  if (n.Entities.Count == 0 || n == end)

                if (n.IsOccupied == false && n.IsVisited == false && n.IsExamined == false)
                {
                    n.IsVisited = true;
                    n.parent = v;
                    queue.Enqueue(n);
                }
            }
        }

        if (queue.Count > 0)
        {
            var s = queue.Dequeue();
            while (s.parent != null)
            {
                path.spots.Add(s.parent);
                s = s.parent;
            }
        }
        return path;
    }
}
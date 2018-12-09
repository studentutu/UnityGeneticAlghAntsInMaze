using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFSs
{

    // IntVector2 - custom struct (basically a copy of Vector2 with int's)
    // GetNeighbors - function to get the neighbors of the current cell
    public class IntVector2
    {
        public int x = 0;
        public int z = 0;
    }

    Dictionary<IntVector2, int> distanceChart = new Dictionary<IntVector2, int>();// this will record the distances from start to all positions on map
    Dictionary<IntVector2, IntVector2> pathChart = new Dictionary<IntVector2, IntVector2>(); // this will record the / a shortest path from start to all positions on map

    public void BFS(IntVector2 startPos)
    {
        IntVector2 currentPos = startPos;
        Queue<IntVector2> frontier = new Queue<IntVector2>();

        distanceChart.Clear();
        pathChart.Clear();

        frontier.Enqueue(currentPos);
        distanceChart.Add(currentPos, 0);
        pathChart.Add(currentPos, startPos);  // IntVector2.downLeft = marker for start position

        while (frontier.Count > 0) // have I not been everywhere
        {
            currentPos = frontier.Dequeue();// get position I am currently at
            foreach (IntVector2 nextPos in GetNeighbors(currentPos)) // get a list of all my neighbors
            {
                if (distanceChart.ContainsKey(nextPos) == false)  // if I have not been on the neighbor cell, process
                {
                    frontier.Enqueue(nextPos);
                    distanceChart.Add(nextPos, 1 + distanceChart[currentPos]);
                    pathChart.Add(nextPos, currentPos);

                    // here can go further logic
                    // i.e. stop once next enemy found
                    // or create a distance list to all enemy positions
                }
            }
        }
    }

    List<IntVector2> GetNeighbors(IntVector2 currentPos)
    {
        List<IntVector2> resultedList = new List<IntVector2>();
        return resultedList;
    }
}

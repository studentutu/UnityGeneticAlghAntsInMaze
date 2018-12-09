using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Direction
{
    Start,
    Right,
    Front,
    Left,
    Back,
};
//<summary>
//Class for representing concrete maze cell.
//</summary>
public class MazeCell
{
    public bool IsVisited = false;
    public bool WallRight = false;
    public bool WallFront = false;
    public bool WallLeft = false;
    public bool WallBack = false;
    public bool IsGoal = false;
    public int row = 0;
    public int column = 0;
    public int myWeight = 0;
    public List<MazeCell> neighbor = new List<MazeCell>();
	public NodeMono myMonoCell;
}

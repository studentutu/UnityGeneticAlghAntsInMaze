using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSomeFunction : MonoBehaviour
{

    [SerializeField] private MazeSpawner currentMazeSpawner;
    [SerializeField] private Graph graphToCalculate;

    // Update is called once per frame
    private MazeCell previous = null;
    void CalculateRowAndColm()
    {
        if (previous != null)
        {
            previous.myMonoCell.transform.rotation = Quaternion.identity;
        }
		previous = null;
        var newOne = transform.position;
		// currentMazeSpawner.ZfloatDistanceCellHeight is between
        int row = (int)( (newOne.z +currentMazeSpawner.ZfloatDistanceCellHeight/2 )  / (currentMazeSpawner.ZfloatDistanceCellHeight )); // heaight

        int colums = (int)( (newOne.x + currentMazeSpawner.XfloatDistanceCellWidth/2) / (currentMazeSpawner.XfloatDistanceCellWidth)); // widht

        var mazeCell = currentMazeSpawner.wholeMaze[row, colums];

        // var path = graphToCalculate.GetShortestPath(mazeCell.myMonoCell.MyNode, currentMazeSpawner.endGoal.MyNode);

        mazeCell.myMonoCell.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 90));
		previous = mazeCell;
        Debug.Log(" Position : " + transform.position + "  [" + row + "]" + "[" + colums + "]");
    }
    [SerializeField] private bool Calc = false;
    void Update()
    {
        if (Calc)
        {
            Calc = false;
            CalculateRowAndColm();
        }
    }
}

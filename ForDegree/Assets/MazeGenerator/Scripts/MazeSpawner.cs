using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour
{
    public enum MazeGenerationAlgorithm
    {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    [Tooltip("Generate Maze with Rows*Rows cells")]
    [SerializeField] public int Rows = 5;
    private int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject GoalPrefab = null;

    private BasicMazeGenerator mMazeGenerator = null;

    [Space]
    [SerializeField]
    private UnityEngine.UI.Button buttonGenerate;
    [HideInInspector] public bool isReady = false;
    [HideInInspector] public List<GameObject> allTargets = new List<GameObject>();

    [HideInInspector]
    public MazeCell[,] wholeMaze;
    [HideInInspector]
    public int[,] fromMazeTOInt;
    public int endGoalWith = 0;
    private IEnumerator waitAbit(UnityEngine.UI.Button button)
    {
        yield return null;
        button.interactable = true;

        float dist = 0;
        float tempDist = 0;
        GameObject target = null;
        for (int i = 0; i < allTargets.Count; i++)
        {
            tempDist = Vector3.Distance(Vector3.zero, allTargets[i].transform.position);
            if (dist <= tempDist)
            {
                dist = tempDist;
                target = allTargets[i];
            }
        }
        for (int i = 0; i < allTargets.Count; i++)
        {
            if (target != allTargets[i])
            {
                Destroy(allTargets[i]);
            }
        }
        allTargets.Clear();
        allTargets.Add(target);


        // BFS
        fromMazeTOInt = null;
        bool isItTheRightOne = false;
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                isItTheRightOne = true;
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                isItTheRightOne = true;
                break;
            case MazeGenerationAlgorithm.RandomTree:
                isItTheRightOne = true;
                break;
            case MazeGenerationAlgorithm.OldestTree:
                isItTheRightOne = true;
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                isItTheRightOne = false;
                break;
        }
        if (isItTheRightOne)
        {
            fromMazeTOInt = new int[Rows, Columns];
            string allInOne = "";
            endGoalWith = 0;
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    fromMazeTOInt[row, column] = wholeMaze[row, column].myWeight;
                    if (endGoalWith <= wholeMaze[row, column].myWeight)
                    {
                        endGoalWith = wholeMaze[row, column].myWeight;
                    }
                    allInOne += string.Format(" {0,2}", wholeMaze[row, column].myWeight);
                }
                allInOne += "\n";
            }

            Debug.Log(allInOne);
        }
        isReady = true;
    }


    public void GenerateMaze()
    {
        buttonGenerate.interactable = false;
        allTargets.Clear();
        DestroyAllChildren();
        // Debug.Log("Started! ");
        var timer = System.Diagnostics.Stopwatch.StartNew();

        if (!FullRandom)
        {
            // Random.seed = RandomSeed;
            Random.InitState(RandomSeed);
        }
        // Rows = Random.Range(10, 50);
        Columns = Rows;
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }
        mMazeGenerator.GenerateMaze();
        wholeMaze = mMazeGenerator.GetWholeMaze();

        ContainerFloor notCached;

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                float x = column * (CellWidth + (AddGaps ? .2f : 0)); // gaps between the walls and floor
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                tmp = Instantiate(Floor, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                notCached = tmp.GetComponent<ContainerFloor>();
                notCached.row = row;
                notCached.column = column;
                tmp.transform.parent = transform;
                if (cell.WallRight)
                {
                    tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 90, 0)) as GameObject;// right
                    tmp.transform.parent = transform;
                }
                if (cell.WallFront)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;// front
                    tmp.transform.parent = transform;
                }
                if (cell.WallLeft)
                {
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 0, z) + Wall.transform.position, Quaternion.Euler(0, 270, 0)) as GameObject;// left
                    tmp.transform.parent = transform;
                }
                if (cell.WallBack)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 0, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;// back
                    tmp.transform.parent = transform;
                }
                if (cell.IsGoal && GoalPrefab != null)
                {
                    tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                    if (tmp != null)
                    {
                        allTargets.Add(tmp);
                    }
                }
            }
        }
        if (Pillar != null)
        {
            for (int row = 0; row < Rows + 1; row++)
            {
                for (int column = 0; column < Columns + 1; column++)
                {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 0, z - CellHeight / 2), Quaternion.identity) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }

        timer.Stop();
        long elapsedMs = timer.ElapsedMilliseconds;
        Debug.Log(" All New! " + elapsedMs + "ms (1/1000 sec)");  // 4 ms

        StartCoroutine(waitAbit(buttonGenerate));
    }

    private void DestroyAllChildren()
    {
        if (gameObject.transform.childCount > 0)
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}

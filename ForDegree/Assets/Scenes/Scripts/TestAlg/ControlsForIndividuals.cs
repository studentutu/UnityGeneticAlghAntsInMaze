using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GeneticImplementation;


public enum DIRECTIONS
{
    RIGHT,
    FRONT,
    LEFT,
    BACK
}
public class ControlsForIndividuals : MonoBehaviour
{
    [Header(" Control Interface")]

    [SerializeField] private Rigidbody playerToControl;
    [SerializeField] private float ForceMagnitude = 2f;

    [Header("Genetic")]

    [SerializeField] int populationSize = 200;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int TopNBestElementsKeep = 5;
    private GeneticAlghorithm<byte> globalAlgor;
    [HideInInspector] public Transform target;
    // [SerializeField] float proximity = 3.5f;


    [Header("Other")]

    [SerializeField] Text numGenerationsText;


    #region Additional_Data
    private List<Rigidbody> allIndividuals = null;
    private float MostDistant = 0;
    private System.Random random;
    private bool initialized = false;
    [SerializeField] private float waSecondsFor = 0.2f;
    private Coroutine runningCoroutine;

    [SerializeField] private Button toTemporaralyDeactivate;
    private bool isitWithTheRightMaze = false;
    private int[,] allWeights = null;
    private int maxWeight = 0;
    #endregion


    public void StopCurrentGenetics()
    {

        toTemporaralyDeactivate.interactable = false;
        initialized = false;
        StartCoroutine(stopandClean());

    }
    public void StopButLeaveTheIndividuals()
    {
        toTemporaralyDeactivate.interactable = false;
        initialized = false;
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        initialized = false;
        globalAlgor = null;

        toTemporaralyDeactivate.interactable = true;
    }
    private IEnumerator stopandClean()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        initialized = false;
        if (allIndividuals != null)
        {
            for (int j = 0; j < allIndividuals.Count; j++)
            {
                Destroy(allIndividuals[j].gameObject);
            }
        }
        yield return null;
        globalAlgor = null;

        toTemporaralyDeactivate.interactable = true;
    }
    public void Initialize()
    {
        var maze = FindObjectOfType<MazeSpawner>();
        if (!maze.isReady)
        {
            return;
        }
        int dnaSize = maze.Rows;
        dnaSize *= dnaSize;
        random = new System.Random();
        allIndividuals = new List<Rigidbody>(populationSize);
        globalAlgor = new GeneticAlghorithm<byte>(populationSize, dnaSize, random, GetRandomDirection, FittnessFunction, TopNBestElementsKeep, mutationRate);

        // Set population and Target!
        StartCoroutine(GenerateIndividuals());
    }
    private IEnumerator GenerateIndividuals()
    {
        for (int i = 0; i < populationSize; i++)
        {
            allIndividuals.Add(Instantiate(playerToControl, Vector3.up, Quaternion.identity));
            if (i % 20 == 1)
            {
                yield return null;
            }
        }

        var mazeGen = FindObjectOfType<MazeSpawner>();
        while (!mazeGen.isReady)
        {
            yield return null;
        }
        yield return null;
        isitWithTheRightMaze = false;
        switch (mazeGen.Algorithm)
        {
            case MazeSpawner.MazeGenerationAlgorithm.PureRecursive:
                isitWithTheRightMaze = true;
                break;
            case MazeSpawner.MazeGenerationAlgorithm.RecursiveTree:
                isitWithTheRightMaze = true;
                break;
            case MazeSpawner.MazeGenerationAlgorithm.RandomTree:
                isitWithTheRightMaze = true;
                break;
            case MazeSpawner.MazeGenerationAlgorithm.OldestTree:
                isitWithTheRightMaze = true;
                break;
            case MazeSpawner.MazeGenerationAlgorithm.RecursiveDivision:
                isitWithTheRightMaze = false;
                break;
        }

        if (isitWithTheRightMaze)
        {
            int row = mazeGen.fromMazeTOInt.GetLength(0);
            int colms = mazeGen.fromMazeTOInt.GetLength(1);


            allWeights = new int[row, colms];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < colms; j++)
                {
                    allWeights[i, j] = mazeGen.fromMazeTOInt[i, j];
                }
            }
            maxWeight = mazeGen.endGoalWith;
        }
        List<GameObject> allTargets = mazeGen.allTargets;
        float dist = 0;
        float tempDist = 0;

        for (int i = 0; i < allTargets.Count; i++)
        {
            tempDist = Vector3.Distance(Vector3.zero, allTargets[i].transform.position);
            if (dist <= tempDist)
            {
                dist = tempDist;
                target = allTargets[i].transform;
            }
        }

        yield return null;

        MostDistant = dist;
        initialized = true;
    }

    // Update is called once per frame
    private void Update()
    {
        while (!initialized)
        {
            return;
        }
        for (int j = 0; j < globalAlgor.Population.Count; j++)
        {
            allIndividuals[j].velocity = Vector3.zero;
            allIndividuals[j].angularVelocity = Vector3.zero;
            allIndividuals[j].transform.position = Vector3.up;
        }
        globalAlgor.NewGeneration(0, false, true); // first is delayed

        initialized = false;
        runningCoroutine = StartCoroutine(waiterSecs());
        // when the wait function is over, start new Generation
        numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes;

        if (globalAlgor.BestFittnes == 1)
        {
            numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes + " End!";
            initialized = false;
            StopButLeaveTheIndividuals();
        }
    }

    private IEnumerator waiterSecs()
    {
        // Waiter for Population action to be over
        for (int i = 0; i < globalAlgor.dnaSize; i++)
        {
            yield return null;
            for (int j = 0; j < globalAlgor.Population.Count; j++)
            {
                UpdatePositionsForIndividuals(globalAlgor.Population[j].Genes[i], j);
            }
            yield return new WaitForSeconds(waSecondsFor);

        }
        globalAlgor.NewGeneration(); // Recalculated fittness ob end position
        initialized = true;
    }

    // Interface of Player Control from Individual
    public void UpdatePositionsForIndividuals(byte direct, int index)
    {
        DIRECTIONS newDirection = (DIRECTIONS)direct;
        // With Rigidbody
        // allIndividuals[index].velocity = Vector3.zero;
        // allIndividuals[index].angularVelocity = Vector3.zero;

        switch (newDirection)
        {
            case DIRECTIONS.RIGHT:
                // With Rigidbody
                allIndividuals[index].velocity = (Vector3.right * ForceMagnitude);
                break;
            case DIRECTIONS.FRONT:
                // With Rigidbody
                allIndividuals[index].velocity = (Vector3.forward * ForceMagnitude);
                break;
            case DIRECTIONS.LEFT:
                // With Rigidbody
                allIndividuals[index].velocity = (Vector3.left * ForceMagnitude);
                break;
            case DIRECTIONS.BACK:
                // With Rigidbody
                allIndividuals[index].velocity = (Vector3.back * ForceMagnitude);
                break;
        }

    }

    // Should return Value from 0 to 1 (best Score)
    public float FittnessFunction(int index)
    {

        // Calculate Distance
        float score = 0;
        // base distance

        if (isitWithTheRightMaze)
        {
            var newOne = allIndividuals[index].transform.GetChild(0).GetComponent<ContainerTrggerUnderneath>();
            if (newOne != null && newOne.ontheFloor != null)
            {
                score = allWeights[newOne.ontheFloor.row, newOne.ontheFloor.column];
                score = score / maxWeight;
                // 1 is the best
            }
        }
        else
        {
            // bad one
            score = Vector3.Distance(allIndividuals[index].transform.position, target.position);
            // normalize from the example - 0 to 1
            score /= MostDistant; // 1 is the farthest away

            score = 1 - score; // 1 is the nearest
        }


        // make the difference in genes Larger !
        score = (Mathf.Pow(2, score) - 1) / (2 - 1); // will normilize to [0-1]

        return score;
    }

    // getRandomGene
    private byte GetRandomDirection()
    {
        byte i = (byte)random.Next(4);
        return i;
    }

}

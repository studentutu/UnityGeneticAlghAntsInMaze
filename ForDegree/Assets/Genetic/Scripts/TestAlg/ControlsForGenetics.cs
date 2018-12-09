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
public class ControlsForGenetics : MonoBehaviour
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
    private float maxWeight = 0;
    #endregion
    [SerializeField] private MazeSpawner currentMazeSpawner = null;

    [SerializeField] private Graph graphToCalculate;
    [SerializeField] private Transform parentToAllIndiv;

    [SerializeField] private bool UseStagnation = false;
    [SerializeField] private int StartSizeOfGenes = 5;
    [SerializeField] private int endNumberOfGenerations = 500;

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
            allIndividuals.Clear();

        while (parentToAllIndiv.childCount > 0)
        {
            Destroy(parentToAllIndiv.GetChild(0));
        }
        yield return null;
        globalAlgor = null;

        toTemporaralyDeactivate.interactable = true;
    }
    public void Initialize()
    {
        if (!currentMazeSpawner.isReady)
        {
            return;
        }
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        if (allIndividuals != null)
            allIndividuals.Clear();

        while (parentToAllIndiv.childCount > 0)
        {
            Destroy(parentToAllIndiv.GetChild(0));
        }

        int dnaSize = currentMazeSpawner.Rows;
        if (UseStagnation)
        {
            dnaSize = StartSizeOfGenes;
        }
        else
        {
            dnaSize = currentMazeSpawner.Rows * currentMazeSpawner.Rows;
        }
        random = new System.Random();
        allIndividuals = new List<Rigidbody>(populationSize);
        globalAlgor = new GeneticAlghorithm<byte>(
                                    populationSize,
                                    dnaSize,
                                    random,
                                    GetRandomDirection,
                                    FittnessFunction,
                                    TopNBestElementsKeep,
                                    mutationRate,
                                    UseStagnation);

        maxWeight = currentMazeSpawner.Rows * currentMazeSpawner.Rows * currentMazeSpawner.diff;
        Debug.Log(" Max Weight = " + maxWeight);
        // Set population and Target!
        StartCoroutine(GenerateIndividuals());
    }
    private IEnumerator GenerateIndividuals()
    {

        for (int i = 0; i < populationSize; i++)
        {
            var someone = Instantiate(playerToControl, parentToAllIndiv);
            someone.transform.position = Vector3.up;
            someone.transform.rotation = Quaternion.identity;

            allIndividuals.Add(someone);
            if (i % 20 == 19)
            {
                yield return null;
            }
        }
        yield return null;
        isitWithTheRightMaze = true;
        ActivatedStagnation = false;
        counter = 0;
        // switch (mazeGen.Algorithm)
        // {
        //     case MazeSpawner.MazeGenerationAlgorithm.PureRecursive:
        //         isitWithTheRightMaze = true;
        //         break;
        //     case MazeSpawner.MazeGenerationAlgorithm.RecursiveTree:
        //         isitWithTheRightMaze = true;
        //         break;
        //     case MazeSpawner.MazeGenerationAlgorithm.RandomTree:
        //         isitWithTheRightMaze = true;
        //         break;
        //     case MazeSpawner.MazeGenerationAlgorithm.OldestTree:
        //         isitWithTheRightMaze = true;
        //         break;
        //     case MazeSpawner.MazeGenerationAlgorithm.RecursiveDivision:
        //         isitWithTheRightMaze = false;
        //         break;
        // }

        if (isitWithTheRightMaze)
        {
            MostDistant = 0;
        }
        else
        {

            List<GameObject> allTargets = currentMazeSpawner.allTargets;
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
            MostDistant = dist;
        }

        yield return null;

        initialized = true;
        StartCoroutine(StartUpdating());
    }

    // Update is called once per frame
    private IEnumerator StartUpdating()
    {
        if (!initialized)
        {
            yield break;
        }
        while (globalAlgor.BestFittnes != 1 || globalAlgor.Generation >= endNumberOfGenerations)
        {

            for (int j = 0; j < globalAlgor.Population.Count; j++)
            {
                allIndividuals[j].velocity = Vector3.zero;
                allIndividuals[j].angularVelocity = Vector3.zero;
                allIndividuals[j].transform.position = Vector3.up;
            }
            // globalAlgor.NewGeneration(0, false, true); // first is delayed
            // when the wait function is over, start new Generation
            numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes;

            runningCoroutine = StartCoroutine(waiterSecs());
            yield return runningCoroutine;
            if (globalAlgor.BestFittnes == 1)
            {
                numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes + " End!";
                initialized = false;
                StopButLeaveTheIndividuals();
                yield break;
            }

        }

    }
    private int counter = 0;
    private bool ActivatedStagnation = false;
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
        globalAlgor.NewGeneration(); // Recalculated fittness 

        if (UseStagnation)
        {
            counter++;
            globalAlgor.AddToStatgantion(globalAlgor.BestFittnes);
            if (counter > globalAlgor.MinimumForWatching)
            {
                ActivatedStagnation = true;
            }
            if (ActivatedStagnation && counter % (globalAlgor.MinimumForWatching + 1) == 0)
            {
                counter = 1;
                if (!globalAlgor.IsStagnationAtPeak())
                {
                    globalAlgor.addNewGenes();
                }
            }
        }
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

            var newOne = allIndividuals[index].transform.position;
            int row = (int)((newOne.z + currentMazeSpawner.ZfloatDistanceCellHeight / 2) / (currentMazeSpawner.ZfloatDistanceCellHeight)); // heaight

            int colums = (int)((newOne.x + currentMazeSpawner.XfloatDistanceCellWidth / 2) / (currentMazeSpawner.XfloatDistanceCellWidth)); // widht

            var mazeCell = currentMazeSpawner.wholeMaze[row, colums];

            var path = graphToCalculate.GetShortestPath(mazeCell.myMonoCell.MyNode, currentMazeSpawner.endGoal.MyNode);
            score = path.length;
            score = (maxWeight - score) / maxWeight; // make minimization problem into maximisation!
                                                     // 1 is the best

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

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
    [SerializeField] private float ForceMagnitude = 25f;

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
    [SerializeField] public MazeSpawner currentMazeSpawner = null;

    [SerializeField] private Graph graphToCalculate;
    [SerializeField] private Transform parentToAllIndiv;

    [SerializeField] private bool UseStagnation = false;
    [SerializeField] private int StartSizeOfGenes = 5;
    [SerializeField] private int endNumberOfGenerations = 500;

    // UI event
    public void StopCurrentGenetics()
    {

        toTemporaralyDeactivate.interactable = false;
        initialized = false;
        StartCoroutine(stopandClean());

    }
    public void StopButLeaveTheIndividuals()
    {
        // toTemporaralyDeactivate.interactable = false;
        initialized = false;

        // toTemporaralyDeactivate.interactable = true;
    }
    private IEnumerator stopandClean()
    {

        initialized = false;
        if (allIndividuals != null)
        {
            foreach (var item in allIndividuals)
            {
                item.velocity = Vector3.zero;
            }
            // if(runningCoroutine != null)
            //     StopCoroutine(runningCoroutine);
            yield return new WaitForSeconds(0.25f);

            while (runningCoroutine != null)
            {
                yield return null;
            }
            allIndividuals.Clear();
        }

        while (parentToAllIndiv.childCount > 0)
        {
            Destroy(parentToAllIndiv.GetChild(0).gameObject);
            yield return null;
        }
        globalAlgor = null;

        toTemporaralyDeactivate.interactable = true;
    }

    public static ControlsForGenetics Instance;
    public void Awake()
    {
        Instance = this;
    }

    // UI event
    public void Initialize()
    {
        if (!currentMazeSpawner.isReady)
        {
            return;
        }
        if (runningCoroutine != null)
            return;
        if (allIndividuals != null)
            allIndividuals.Clear();

        while (parentToAllIndiv.childCount > 0)
        {
            Destroy(parentToAllIndiv.GetChild(0).gameObject);
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
        // Debug.Log(" Max Weight = " + maxWeight);

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
        runningCoroutine = StartCoroutine(StartUpdating());
    }

    // Update is called once per frame
    private IEnumerator StartUpdating()
    {
        while (globalAlgor.BestFittnes != 1 || globalAlgor.Generation >= endNumberOfGenerations)
        {
            if (!initialized)
            {
                runningCoroutine = null;
                yield break;
            }
            for (int j = 0; j < globalAlgor.Population.Count; j++)
            {
                allIndividuals[j].velocity = Vector3.zero;
                allIndividuals[j].angularVelocity = Vector3.zero;
                allIndividuals[j].transform.position = Vector3.up;
            }
            // globalAlgor.NewGeneration(0, false, true); // first is delayed
            // when the wait function is over, start new Generation
            numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes;

            yield return StartCoroutine(waiterSecs());
            if (!initialized)
            {
                runningCoroutine = null;
                yield break;
            }
            if (globalAlgor.BestFittnes == 1)
            {
                numGenerationsText.text = "GENERATION: " + globalAlgor.Generation + " BEST FIT : " + globalAlgor.BestFittnes + " End!";
                initialized = false;
                runningCoroutine = null;
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
            if (!initialized)
            {
                runningCoroutine = null;
                yield break;
            }
            for (int j = 0; j < globalAlgor.Population.Count; j++)
            {
                UpdatePositionsForIndividuals(globalAlgor.Population[j].Genes[i], j);
            }
            float timeUp = waSecondsFor;
            while (timeUp > 0)
            {
                if (!initialized)
                {
                    runningCoroutine = null;
                    yield break;
                }
                timeUp -= Time.deltaTime;
                yield return null;
            }

        }
        if (!initialized)
        {
            runningCoroutine = null;
            yield break;
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

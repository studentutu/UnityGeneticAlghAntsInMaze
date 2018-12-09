
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using System;
using GeneticImplementation;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestText : MonoBehaviour
{

    [Header("Genetic Algorithm")]
    [SerializeField] string targetString = "To be, or not to be, that is the question.";
    [SerializeField] string validCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ,.|!#$%&/()=? ";
    [SerializeField] int populationSize = 200;
    [SerializeField] float mutationRate = 0.01f;
    [SerializeField] int TopNBestElementsKeep = 5;

    [Header("Other")]
    [SerializeField] int numCharsPerText = 15000;
    [SerializeField] Text targetText;
    [SerializeField] Text bestText;
    [SerializeField] Text bestFitnessText;
    [SerializeField] Text numGenerationsText;
    [SerializeField] Transform populationTextParent;
    [SerializeField] Text textPrefab;

    private GeneticAlghorithm<char> ga;
    private System.Random random;

    void Start()
    {
        targetText.text = targetString;

        if (string.IsNullOrEmpty(targetString))
        {
            Debug.LogError("Target string is null or empty");
            this.enabled = false;
        }

        random = new System.Random();
        ga = new GeneticAlghorithm<char>(populationSize, targetString.Length, random, GetRandomCharacter, FitnessFunction, TopNBestElementsKeep, mutationRate);

    }
    [Header("How Much To Store")]
    [SerializeField] private int storeEvery = 20;


    void Update()
    {

        ga.NewGeneration();
        if (ga.Generation % storeEvery == 0)
        {
            GeneticSaveData<char> newSave = new GeneticSaveData<char>();
            newSave.TakeFrom(ga);
            bool b = newSave.saveTo("save" + ga.Generation);
            Debug.Log( b? CharArrayToString(ga.Population[0].Genes) : "not saved");
        }

        UpdateText(ga.BestGenes, ga.BestFittnes, ga.Generation, ga.Population.Count, (j) => ga.Population[j].Genes);

        if (ga.BestFittnes == 1)
        {
            GeneticSaveData<char> newSave = new GeneticSaveData<char>();
            newSave.TakeFrom(ga);
            bool b = newSave.saveTo("save" + ga.Generation);            
            Debug.Log( b? CharArrayToString(ga.Population[0].Genes) : "not saved");
            this.enabled = false;
        }
    }

    // getRandomGene
    private char GetRandomCharacter()
    {
        int i = random.Next(validCharacters.Length);
        return validCharacters[i];
    }

    // Claculate Fitness
    private float FitnessFunction(int index)
    {
        float score = 0;
        DNA<char> dna = ga.Population[index];

        for (int i = 0; i < dna.Genes.Length; i++)
        {
            if (dna.Genes[i] == targetString[i])
            {
                score += 1;
            }
        }
        // normizle from the example - 0 to 1
        score /= targetString.Length;

        // make the difference in genes Larger !!!!!!!!!!!!!!!!!!!!!!!
        // score = Mathf.Pow(5,score); // range of 0 to 5, but we need [0,1]
        score = (Mathf.Pow(2, score) - 1) / (2 - 1); // will normilize to [0-1]

        return score;
    }



    private int numCharsPerTextObj;
    private List<Text> textList = new List<Text>();

    void Awake()
    {
        numCharsPerTextObj = numCharsPerText / validCharacters.Length;
        if (numCharsPerTextObj > populationSize) numCharsPerTextObj = populationSize;

        int numTextObjects = Mathf.CeilToInt((float)populationSize / numCharsPerTextObj);

        for (int i = 0; i < numTextObjects; i++)
        {
            textList.Add(Instantiate(textPrefab, populationTextParent));
        }
    }

    private void UpdateText(char[] bestGenes, float bestFitness, int generation, int populationSize, Func<int, char[]> getGenes)
    {
        bestText.text = CharArrayToString(bestGenes);
        bestFitnessText.text = bestFitness.ToString();

        numGenerationsText.text = generation.ToString();

        for (int i = 0; i < textList.Count; i++)
        {
            var sb = new StringBuilder();
            int endIndex = i == textList.Count - 1 ? populationSize : (i + 1) * numCharsPerTextObj;
            for (int j = i * numCharsPerTextObj; j < endIndex; j++)
            {
                foreach (var c in getGenes(j))
                {
                    sb.Append(c);
                }
                if (j < endIndex - 1) sb.AppendLine();
            }

            textList[i].text = sb.ToString();
        }
    }

    private string CharArrayToString(char[] charArray)
    {
        var sb = new StringBuilder();
        foreach (var c in charArray)
        {
            sb.Append(c);
        }

        return sb.ToString();
    }

#if UNITY_EDITOR
    // [MenuItem("Tools/SaveTo")]
    public static void ShowOutput()
    {
        // Debug.Log(Application.persistentDataPath);

        // GeneticSaveData<char> newSave = new GeneticSaveData<char>();
        // newSave.Generation = 1;
        // newSave.AllGenesFromPopulation = new List<char[]>();
        // newSave.AllGenesFromPopulation.Add(new char[] { 'r', 'b', 'c' });
        // newSave.AllGenesFromPopulation.Add(new char[] { 'p', 'o', 'm', 'e' });

        // string name = "save" + newSave.Generation;
        // bool isit = newSave.saveTo(name);
        // bool isit = newSave.readFrom(name);
    }
#endif
}

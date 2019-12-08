using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DEController : MonoBehaviour, IEvolutionaryController
{
    public int networkInputLayerSize = 9;
    public int networkHiddenLayerSize = 12;
    public int networkOutputLayerSize = 9;
    
    public int populationSize = 100;
    public float stopValue = 1.0f;
    public int maxIteration = 500;
    
    public GameObject simulatedEnvironment;
    private List<GameObject> simulationInstanceList;

    private List<Genome> populationList;
    private List<Genome> candidateList;
    private Genome bestGenome;
    private int currentIndex;
    
    private bool initialPopulationEvaluated;
    private bool running;
    private int totalIteration;

    private int testCase = 0;

    private EvolutionaryStatistics statistics;

    private readonly static System.Random randomGenerator = new System.Random();
    
    void Start()
    {
        this.simulationInstanceList = new List<GameObject>();
        this.populationList = new List<Genome>();

        for (int i = 0; i < populationSize; i++)
        {
            this.populationList.Add(new Genome(new MultiLayerPerceptron(i, networkInputLayerSize, networkHiddenLayerSize, networkOutputLayerSize)));
        }
        
        this.bestGenome = this.populationList[0];
        this.currentIndex = 0;
        this.initialPopulationEvaluated = false;
        this.running = true;
        this.totalIteration = 0;

        this.statistics = new EvolutionaryStatistics();
    }

    private float generateRandomFloatNumber(float minimum, float maximum)
    {
        return (float)(randomGenerator.NextDouble() * (maximum - minimum) + minimum);
    }

    private int generateRandomIntNumber(int minimum, int maximum)
    {
        return randomGenerator.Next(minimum, maximum);
    }

    private void updatePopulationList()
    {
        if (initialPopulationEvaluated)
        {
            for (int i = 0; i < populationList.Count; i++)
            {
                if (candidateList[i].Fitness <= populationList[i].Fitness)
                {
                    populationList[i] = candidateList[i];
                }
            }
        }
    }

    private void updateBestGenome()
    {
        if (initialPopulationEvaluated)
        {
            float sumPopulationFitness = 0.0f;

            for (int i = 0; i < populationList.Count; i++)
            {
                sumPopulationFitness += populationList[i].Fitness;

                if (populationList[i].Fitness <= bestGenome.Fitness)
                {
                    bestGenome = new Genome(populationList[i]);
                }
            }

            statistics.BestFitnessList.Add(bestGenome.Fitness);
            statistics.AvgFitnessList.Add(sumPopulationFitness / populationSize);
        }
    }

    private List<Genome> generateCandidateList()
    {
        List<Genome> candidateNewList = new List<Genome>();

        for (int i = 0; i < populationSize; i++)
        {
            float F = generateRandomFloatNumber(0.0f, 1.0f);
            float CR = generateRandomFloatNumber(0.0f, 1.0f);
            bool useBestStrategy = (generateRandomFloatNumber(0.0f, 1.0f) >= 0.5f);

            Genome candidateGenome = new Genome(populationList[i]);
            Genome genomeA, genomeB, genomeC;

            if (useBestStrategy)
            {
                int genomeIndexA, genomeIndexB;

                do
                {
                    genomeIndexA = generateRandomIntNumber(0, (populationSize - 1));
                }
                while (genomeIndexA == i);

                do
                {
                    genomeIndexB = generateRandomIntNumber(0, (populationSize - 1));
                }
                while ((genomeIndexB == i) || (genomeIndexB == genomeIndexA));

                genomeA = populationList[genomeIndexA];
                genomeB = populationList[genomeIndexB];
                genomeC = bestGenome;
            }
            else
            {
                int genomeIndexA, genomeIndexB, genomeIndexC;

                do
                {
                    genomeIndexA = generateRandomIntNumber(0, (populationSize - 1));
                }
                while (genomeIndexA == i);

                do
                {
                    genomeIndexB = generateRandomIntNumber(0, (populationSize - 1));
                }
                while ((genomeIndexB == i) || (genomeIndexB == genomeIndexA));

                do
                {
                    genomeIndexC = generateRandomIntNumber(0, (populationSize - 1));
                }
                while ((genomeIndexC == i) || (genomeIndexC == genomeIndexB) || (genomeIndexC == genomeIndexA));

                genomeA = populationList[genomeIndexA];
                genomeB = populationList[genomeIndexB];
                genomeC = populationList[genomeIndexC];
            }

            int randomWeightIndex = generateRandomIntNumber(0, (candidateGenome.Weights.Length - 1));

            for (int j = 0; j < candidateGenome.Weights.Length; j++)
            {
                if ((j == randomWeightIndex) || (generateRandomFloatNumber(0.0f, 1.0f) < CR))
                {
                    candidateGenome.Weights[j] = genomeC.Weights[j] + F * (genomeA.Weights[j] - genomeB.Weights[j]);
                }
            }

            candidateNewList.Add(candidateGenome);
        }

        return candidateNewList;
    }

    void Update()
    {
        if(!running)
        {
            if (Input.GetKey(KeyCode.Keypad0))
            {
                testCase = 0;
            }
            else if(Input.GetKey(KeyCode.Keypad1))
            {
                testCase = 1;
            }
            else if(Input.GetKey(KeyCode.Keypad2))
            {
                testCase = 2;
            }
            else if(Input.GetKey(KeyCode.Keypad3))
            {
                testCase = 3;
            }
        }

        //if (running && (Input.GetKey(KeyCode.P) || (bestGenome.Fitness <= stopValue)))
        if (running && (Input.GetKey(KeyCode.P) || (totalIteration == maxIteration)))
        {
            running = false;

            foreach (GameObject instance in simulationInstanceList)
            {
                Destroy(instance);
            }

            simulationInstanceList.Clear();

            candidateList.Clear();
            candidateList.Add(bestGenome);

            statistics.TotalIteration = totalIteration;
            statistics.TotalExecutionTime = Time.realtimeSinceStartup;
            statistics.save();
        }
        
        if (simulationInstanceList.Count == 0)
        {
            currentIndex = 0;
            
            if (running)
            {
                updatePopulationList();
                updateBestGenome();

                Debug.Log("Total iteration: " + totalIteration + " | Best ID: " + bestGenome.Id + " | Best fitness: " + bestGenome.Fitness);
                totalIteration++;

                if (initialPopulationEvaluated)
                {
                    candidateList = generateCandidateList();
                }
                else
                {
                    candidateList = populationList;
                    initialPopulationEvaluated = true;
                }

                int sqrtDimension = (int)Math.Sqrt(candidateList.Count);
                int xDimension = sqrtDimension;
                int zDimension = sqrtDimension;

                for (int i = 0; i < xDimension; i++)
                {
                    for (int j = 0; j < zDimension; j++)
                    {
                        GameObject instance = Instantiate(simulatedEnvironment);
                        instance.transform.parent = transform;
                        instance.transform.position = new Vector3((i * 40), 0, (j * 40));
                        simulationInstanceList.Add(instance);
                    }
                }
            }
            else
            {
                GameObject candidateInstance = Instantiate(simulatedEnvironment);
                candidateInstance.transform.parent = transform;
                simulationInstanceList.Add(candidateInstance);
            }
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool isRunning()
    {
        return running;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public int getInstanceIndex()
    {
        return currentIndex++;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public GameObject getInstanceByIndex(int instanceIndex)
    {
        return simulationInstanceList[instanceIndex];
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Genome getGenomeByIndex(int instanceIndex)
    {
        return candidateList[instanceIndex];
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setFitness(GameObject instance, int instanceIndex, float fitness)
    {
        Destroy(instance, 0.25f);
        simulationInstanceList.Remove(instance);

        candidateList[instanceIndex].Fitness = fitness;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Vector3 getPadPosition()
    {
        if(!running)
        {
            switch(testCase)
            {
                case 1:
                    return new Vector3(-10.0f, 0.025f, 0.0f);
                case 2:
                    return new Vector3(-10.0f, 0.025f, 10.0f);
                case 3:
                    return new Vector3(10.0f, 0.025f, 0.0f);
                default:
                    return new Vector3(0.0f, 0.025f, 0.0f);
            }
        }
        else
        {
            return new Vector3(0.0f, 0.025f, 0.0f);
        }
    }
}

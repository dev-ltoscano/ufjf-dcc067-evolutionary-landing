using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GAController : MonoBehaviour, IEvolutionaryController
{
    public int networkInputLayerSize = 9;
    public int networkHiddenLayerSize = 12;
    public int networkOutputLayerSize = 9;
    
    public int populationSize = 100;
    public int elitismCount = 5;
    public float crossoverRate = 0.9f;
    public float mutationRate = 0.1f;
    public float stopValue = 1.0f;

    public GameObject simulatedEnvironment;
    private List<GameObject> simulationInstanceList;

    private List<Genome> populationList;
    private Genome bestGenome;
    private int currentIndex;

    private bool initialPopulationEvaluated;
    private bool running;
    private int totalIteration;

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

    private void updateBestGenome()
    {
        float sumPopulationFitness = 0.0f;

        for (int i = 0; i < populationList.Count; i++)
        {
            sumPopulationFitness += populationList[i].Fitness;

            if (populationList[i].Fitness < bestGenome.Fitness)
            {
                bestGenome = new Genome(populationList[i]);
            }
        }

        statistics.BestFitnessList.Add(bestGenome.Fitness);
        statistics.AvgFitnessList.Add(sumPopulationFitness / populationSize);
    }

    private Genome genomeTournamentSelection(int tournamentSize)
    {
        List<Genome> tournamentList = new List<Genome>();
        int genomeIndex;

        while (tournamentList.Count < tournamentSize)
        {
            genomeIndex = randomGenerator.Next(populationList.Count);
            tournamentList.Add(populationList[genomeIndex]);
        }

        tournamentList = tournamentList.OrderBy(obj => obj.Fitness).ToList();

        return tournamentList[0];
    }

    private List<Genome> genomeCrossover(Genome genomeA, Genome genomeB, double crossoverRate)
    {
        if (generateRandomFloatNumber(0.0f, 1.0f) > crossoverRate)
        {
            return null;
        }

        List<Genome> childGenomeList = new List<Genome>();

        int genomeSize = genomeA.Weights.Length;
        double[] childWeightGenomeA = new double[genomeSize];
        double[] childWeightGenomeB = new double[genomeSize];

        int cutoffGenomeIndex = generateRandomIntNumber(0, (genomeSize - 1));
        int weightIndex = 0;

        // ChildA
        for (int i = 0; i < cutoffGenomeIndex; i++)
        {
            childWeightGenomeA[weightIndex++] = genomeA.Weights[i];
        }

        for (int i = cutoffGenomeIndex; i < genomeSize; i++)
        {
            childWeightGenomeA[weightIndex++] = genomeB.Weights[i];
        }

        weightIndex = 0;

        // ChildB
        for (int i = cutoffGenomeIndex; i < genomeSize; i++)
        {
            childWeightGenomeB[weightIndex++] = genomeA.Weights[i];
        }

        for (int i = 0; i < cutoffGenomeIndex; i++)
        {
            childWeightGenomeB[weightIndex++] = genomeB.Weights[i];
        }

        Genome childGenomeA = new Genome(new MultiLayerPerceptron(0, networkInputLayerSize, networkHiddenLayerSize, networkOutputLayerSize));
        childGenomeA.Weights = childWeightGenomeA;

        Genome childGenomeB = new Genome(new MultiLayerPerceptron(0, networkInputLayerSize, networkHiddenLayerSize, networkOutputLayerSize));
        childGenomeB.Weights = childWeightGenomeB;

        childGenomeList.Add(childGenomeA);
        childGenomeList.Add(childGenomeB);

        return childGenomeList;
    }

    private void genomeMutation(Genome genomeA, float mutateRate)
    {
        for (int i = 0; i < genomeA.Weights.Length; i++)
        {
            if (generateRandomFloatNumber(0.0f, 1.0f) > mutateRate)
            {
                continue;
            }

            genomeA.Weights[i] = generateRandomFloatNumber(-1.0f, 1.0f);
        }
    }

    private List<Genome> generateNewPopulation()
    {
        List<Genome> newPopulationList = new List<Genome>();

        // Elitism
        List<Genome> orderedPopulationList = populationList.OrderBy(obj => obj.Fitness).ToList();
        
        for (int i = 0; i < elitismCount; i++)
        {
            newPopulationList.Add(orderedPopulationList[i]);
        }

        while (newPopulationList.Count < populationSize)
        {
            // Crossover
            Genome genomeA = genomeTournamentSelection(3);
            Genome genomeB = genomeTournamentSelection(3);

            List<Genome> childGenomeList = genomeCrossover(genomeA, genomeB, crossoverRate);

            if(childGenomeList != null)
            {
                Genome childGenomeA = childGenomeList[0];
                Genome childGenomeB = childGenomeList[1];

                // Mutation
                genomeMutation(childGenomeA, mutationRate);
                genomeMutation(childGenomeB, mutationRate);

                if (newPopulationList.Count < populationSize)
                {
                    newPopulationList.Add(childGenomeA);
                }

                if (newPopulationList.Count < populationSize)
                {
                    newPopulationList.Add(childGenomeB);
                }
            }
            else
            {
                // Mutation
                genomeMutation(genomeA, (1.0f / genomeA.Weights.Length));
                genomeMutation(genomeB, (1.0f / genomeB.Weights.Length));

                if(newPopulationList.Count < populationSize)
                {
                    newPopulationList.Add(genomeA);
                }

                if (newPopulationList.Count < populationSize)
                {
                    newPopulationList.Add(genomeB);
                }
            }
        }

        for(int i = 0; i < populationSize; i++)
        {
            newPopulationList[i].Id = i;
        }

        return newPopulationList;
    }

    void Update()
    {
        if (running && (Input.GetKey(KeyCode.P) || (bestGenome.Fitness <= stopValue)))
        {
            running = false;

            foreach (GameObject instance in simulationInstanceList)
            {
                Destroy(instance);
            }

            simulationInstanceList.Clear();

            populationList.Clear();
            populationList.Add(bestGenome);

            statistics.TotalIteration = totalIteration;
            statistics.TotalExecutionTime = Time.realtimeSinceStartup;
            statistics.save();
        }

        if (simulationInstanceList.Count == 0)
        {
            currentIndex = 0;

            if (running)
            {
                if (initialPopulationEvaluated)
                {
                    updateBestGenome();
                    Debug.Log("Total iteration: " + totalIteration + " | Best ID: " + bestGenome.Id + " | Best fitness: " + bestGenome.Fitness);
                    totalIteration++;

                    populationList = generateNewPopulation();
                }
                else
                {
                    initialPopulationEvaluated = true;
                }

                int sqrtDimension = (int)Math.Sqrt(populationList.Count);
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
        return populationList[instanceIndex];
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setFitness(GameObject instance, int instanceIndex, float fitness)
    {
        Destroy(instance, 0.25f);
        simulationInstanceList.Remove(instance);

        populationList[instanceIndex].Fitness = fitness;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public Vector3 getPadPosition()
    {
        if (!running && Input.GetKey(KeyCode.Alpha1))
        {
            return new Vector3(-15.0f, 0.025f, 15.0f);
        }
        else
        {
            return new Vector3(0.0f, 0.025f, 0.0f);
        }
    }
}

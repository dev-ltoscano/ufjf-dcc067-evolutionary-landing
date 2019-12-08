using System;
using System.IO;
using System.Collections.Generic;

public class EvolutionaryStatistics
{
    private int totalIteration;
    private float totalExecutionTime;
    private List<float> bestFitnessList;
    private List<float> avgFitnessList;

    public int TotalIteration
    {
        set { this.totalIteration = value; }
    }

    public float TotalExecutionTime
    {
        set { this.totalExecutionTime = value; }
    }

    public List<float> BestFitnessList
    {
        get { return this.bestFitnessList; }
    }

    public List<float> AvgFitnessList
    {
        get { return this.avgFitnessList; }
    }

    public EvolutionaryStatistics()
    {
        this.bestFitnessList = new List<float>();
        this.avgFitnessList = new List<float>();
    }
    
    public void save()
    {
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        
        using (StreamWriter outputFile = new StreamWriter(filePath + @"\evolutionaryLanding.txt"))
        {
            outputFile.WriteLine("Total iteration: " + totalIteration);
            outputFile.WriteLine("Total execution time: " + totalExecutionTime);

            outputFile.WriteLine();
            outputFile.WriteLine("========== BEST FITNESS ==========");
            outputFile.WriteLine();

            for (int i = 0; i < bestFitnessList.Count; i++)
            {
                outputFile.WriteLine((i + 1).ToString() + "\t" + bestFitnessList[i]);
            }

            outputFile.WriteLine();
            outputFile.WriteLine("========== END BEST FITNESS ==========");

            outputFile.WriteLine();
            outputFile.WriteLine("========== AVG FITNESS ==========");
            outputFile.WriteLine();

            for (int i = 0; i < avgFitnessList.Count; i++)
            {
                outputFile.WriteLine((i + 1).ToString() + "\t" + avgFitnessList[i]);
            }

            outputFile.WriteLine();
            outputFile.WriteLine("========== END AVG FITNESS ==========");
        }
    }
}

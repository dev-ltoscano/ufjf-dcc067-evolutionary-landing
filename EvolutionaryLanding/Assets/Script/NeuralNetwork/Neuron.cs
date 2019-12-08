using System;

public class Neuron
{
    // Gerador de números pseudo-aleatórios comum a todos os neurônios
    private readonly static Random randomGenerator = new Random();

    // Identificador do neurônio
    private int id;

    // Vetor de pesos do neurônio
    private double[] weight;

    /// <summary>
    /// Get para o identificador do neurônio
    /// </summary>
    public int Id
    {
        get { return this.id; }
    }

    /// <summary>
    /// Get para a quantidade de pesos do neurônio
    /// </summary>
    public int WeightSize
    {
        get { return this.weight.Length; }
    }

    /// <summary>
    /// Get/Set para o vetor de pesos do neurônio
    /// </summary>
    public double[] Weights
    {
        get { return this.weight; }
        set { this.weight = value; }
    }

    /// <summary>
    /// Inicializa o neurônio
    /// </summary>
    /// <param name="id">Identificador do neurônio</param>
    /// <param name="inputSize">Tamanho da entrada do neurônio</param>
    public Neuron(int id, int inputSize)
    {
        this.id = id;
        this.weight = new double[inputSize];

        for (int i = 0; i < inputSize; i++)
        {
            this.weight[i] = generateRandomWeight(-1.0f, 1.0f);
        }
    }

    private double generateRandomWeight(double minimum, double maximum)
    {
        return (randomGenerator.NextDouble() * (maximum - minimum) + minimum);
    }

    /// <summary>
    /// Calcula a soma ponderada da entrada com os pesos do neurônio
    /// </summary>
    /// <param name="input">Entrada do neurônio</param>
    /// <returns>Soma ponderada da entrada com os pesos do neurônio</returns>
    private double calculateWeightedInputSum(double[] input)
    {
        double sumInput = 0.0f;

        for (int i = 0; i < weight.Length; i++)
        {
            sumInput += (input[i] * weight[i]);
        }

        return sumInput;
    }

    /// <summary>
    /// Calcula o valor da função de ativação para o neurônio
    /// </summary>
    /// <param name="value">Soma ponderada da entrada do neurônio</param>
    /// <returns>Valor da função de ativação</returns>
    private double calculateActivationFunctionValue(double value)
    {
        return Math.Tanh(value);
    }

    /// <summary>
    /// Faz a ativação do neurônio
    /// </summary>
    /// <param name="input">Entrada do neurônio</param>
    /// <returns>Valor da função de ativação do neurônio</returns>
    public double activate(double[] input)
    {
        return calculateActivationFunctionValue(calculateWeightedInputSum(input));
    }
}
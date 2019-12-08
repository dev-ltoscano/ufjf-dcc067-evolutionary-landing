public class Genome
{
    // Identificador do genoma
    private int id;

    // Tamanho da camada de entrada da rede neural do genoma
    private int inputLayerSize;

    // Tamanho da camada oculta da rede neural do genoma
    private int hiddenLayerSize;

    // Tamanho da camada de saída da rede neural do genoma
    private int outputLayerSize;

    // Vetor com todos os pesos da rede neural do genoma
    private double[] weight;

    // Valor de aptidão para o genoma
    private float fitness;

    /// <summary>
    /// Get para o identificador do genoma
    /// </summary>
    public int Id
    {
        get { return this.id; }
        set { this.id = value; }
    }

    /// <summary>
    /// Get/Set para os pesos da rede neural do genoma
    /// </summary>
    public double[] Weights
    {
        get { return this.weight; }
        set { this.weight = value; }
    }

    /// <summary>
    /// Get/Set para o valor de aptidão do genoma
    /// </summary>
    public float Fitness
    {
        get { return this.fitness; }
        set { this.fitness = value; }
    }

    public Genome(Genome genome)
    {
        this.id = genome.id;
        this.inputLayerSize = genome.inputLayerSize;
        this.hiddenLayerSize = genome.hiddenLayerSize;
        this.outputLayerSize = genome.outputLayerSize;
        this.weight = new double[genome.weight.Length];

        for(int i = 0; i < genome.weight.Length; i++)
        {
            this.weight[i] = genome.weight[i];
        }

        this.fitness = genome.fitness;
    }

    public Genome(MultiLayerPerceptron neuralNetwork)
    {
        this.id = neuralNetwork.Id;
        this.inputLayerSize = neuralNetwork.InputLayerSize;
        this.hiddenLayerSize = neuralNetwork.HiddenLayer1Size;
        this.outputLayerSize = neuralNetwork.OutputLayerSize;
        this.weight = new double[neuralNetwork.NeuronWeightCount];

        int weightIndex = 0;

        for (int i = 0; i < hiddenLayerSize; i++)
        {
            for (int j = 0; j < neuralNetwork.HiddenLayer1.NeuronList[i].WeightSize; j++)
            {
                this.weight[weightIndex++] = neuralNetwork.HiddenLayer1.NeuronList[i].Weights[j];
            }
        }

        for (int i = 0; i < hiddenLayerSize; i++)
        {
            for (int j = 0; j < neuralNetwork.HiddenLayer2.NeuronList[i].WeightSize; j++)
            {
                this.weight[weightIndex++] = neuralNetwork.HiddenLayer2.NeuronList[i].Weights[j];
            }
        }

        for (int i = 0; i < outputLayerSize; i++)
        {
            for (int j = 0; j < neuralNetwork.OutputLayer.NeuronList[i].WeightSize; j++)
            {
                this.weight[weightIndex++] = neuralNetwork.OutputLayer.NeuronList[i].Weights[j];
            }
        }

        this.fitness = float.MaxValue;
    }

    /// <summary>
    /// Constrói uma rede neural a partir do genoma
    /// </summary>
    /// <returns>Rede neural para o genoma</returns>
    public MultiLayerPerceptron buildNeuralNetwork()
    {
        MultiLayerPerceptron neuralNetwork = new MultiLayerPerceptron(id, inputLayerSize, hiddenLayerSize, outputLayerSize);

        int weightIndex = 0;

        for (int i = 0; i < neuralNetwork.HiddenLayer1Size; i++)
        {
            for (int j = 0; j < neuralNetwork.HiddenLayer1.NeuronList[i].WeightSize; j++)
            {
                neuralNetwork.HiddenLayer1.NeuronList[i].Weights[j] = this.weight[weightIndex++];
            }
        }

        for (int i = 0; i < neuralNetwork.HiddenLayer2Size; i++)
        {
            for (int j = 0; j < neuralNetwork.HiddenLayer2.NeuronList[i].WeightSize; j++)
            {
                neuralNetwork.HiddenLayer2.NeuronList[i].Weights[j] = this.weight[weightIndex++];
            }
        }

        for (int i = 0; i < neuralNetwork.OutputLayerSize; i++)
        {
            for (int j = 0; j < neuralNetwork.OutputLayer.NeuronList[i].WeightSize; j++)
            {
                neuralNetwork.OutputLayer.NeuronList[i].Weights[j] = this.weight[weightIndex++];
            }
        }

        return neuralNetwork;
    }
}

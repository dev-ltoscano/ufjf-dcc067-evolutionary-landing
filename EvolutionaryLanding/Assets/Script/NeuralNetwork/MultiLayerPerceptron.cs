public class MultiLayerPerceptron
{
    // Identificador da rede neural
    private int id;

    // Tamanho da camada de entrada
    private int inputLayerSize;

    // Primeira camada oculta da rede neural
    private NeuronLayer hiddenLayer1;

    // Segunda camada oculta da rede neural
    private NeuronLayer hiddenLayer2;

    // Camada de saída da rede neural
    private NeuronLayer outputLayer;

    /// <summary>
    /// Get para o identificador da rede neural
    /// </summary>
    public int Id
    {
        get { return this.id; }
    }

    /// <summary>
    /// Get para o tamanho da camada de entrada da rede neural
    /// </summary>
    public int InputLayerSize
    {
        get { return this.inputLayerSize; }
    }

    /// <summary>
    /// Get para a camada oculta da rede neural
    /// </summary>
    public NeuronLayer HiddenLayer1
    {
        get { return this.hiddenLayer1; }
    }

    public NeuronLayer HiddenLayer2
    {
        get { return this.hiddenLayer2; }
    }

    /// <summary>
    /// Get para o tamanho da camada oculta da rede neural
    /// </summary>
    public int HiddenLayer1Size
    {
        get { return this.hiddenLayer1.LayerSize; }
    }

    public int HiddenLayer2Size
    {
        get { return this.hiddenLayer2.LayerSize; }
    }

    /// <summary>
    /// Get para a camada de saída da rede neural
    /// </summary>
    public NeuronLayer OutputLayer
    {
        get { return this.outputLayer; }
    }

    /// <summary>
    /// Get para o tamanho da camada de saída da rede neural
    /// </summary>
    public int OutputLayerSize
    {
        get { return this.outputLayer.LayerSize; }
    }
    
    /// <summary>
    /// Get para a quantidade de neurônios da rede neural
    /// </summary>
    public int NeuronCount
    {
        get { return (hiddenLayer1.LayerSize + hiddenLayer2.LayerSize + outputLayer.LayerSize); }
    }

    /// <summary>
    /// Get para a quantidade de pesos da rede neural
    /// </summary>
    public int NeuronWeightCount
    {
        get
        {
            int count = 0;

            foreach (Neuron hiddenNeuron in hiddenLayer1.NeuronList)
            {
                count += hiddenNeuron.Weights.Length;
            }

            foreach (Neuron hiddenNeuron in hiddenLayer2.NeuronList)
            {
                count += hiddenNeuron.Weights.Length;
            }

            foreach (Neuron outputNeuron in outputLayer.NeuronList)
            {
                count += outputNeuron.Weights.Length;
            }

            return count;
        }
    }

    /// <summary>
    /// Inicializa a rede neural
    /// </summary>
    /// <param name="id">Identificador da rede neural</param>
    /// <param name="inputLayerSize">Tamanho da camada de entrada da rede neural</param>
    /// <param name="hiddenLayerSize">Tamanho da camada oculta da rede neural</param>
    /// <param name="outputLayerSize">Tamanho da camada de saída da rede neural</param>
    public MultiLayerPerceptron(int id, int inputLayerSize, int hiddenLayerSize, int outputLayerSize)
    {
        this.id = id;
        this.inputLayerSize = inputLayerSize;
        this.hiddenLayer1 = new NeuronLayer(1, hiddenLayerSize, inputLayerSize);
        this.hiddenLayer2 = new NeuronLayer(2, hiddenLayerSize, hiddenLayerSize);
        this.outputLayer = new NeuronLayer(3, outputLayerSize, hiddenLayerSize);
    }

    /// <summary>
    /// Faz a ativação da rede neural
    /// </summary>
    /// <param name="input">Entrada para a rede neural</param>
    /// <returns>Ativação da rede neural</returns>
    public double[] activate(double[] input)
    {
        input = hiddenLayer1.activate(input);
        input = hiddenLayer2.activate(input);
        return outputLayer.activate(input);
    }
}
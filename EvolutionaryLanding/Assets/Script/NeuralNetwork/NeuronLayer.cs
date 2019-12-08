using System.Collections.Generic;

public class NeuronLayer
{
    // Identificador da camada de neurônios
    private int id;

    // Lista com os neurônios da camada
    private List<Neuron> neuronList;

    /// <summary>
    /// Get para o identificador da camada de neurônios
    /// </summary>
    public int Id
    {
        get { return this.id; }
    }

    /// <summary>
    /// Lista de neurônios da camada
    /// </summary>
    public List<Neuron> NeuronList
    {
        get { return this.neuronList; }
    }

    /// <summary>
    /// Get para quantidade de neurônios da camada
    /// </summary>
    public int LayerSize
    {
        get { return this.neuronList.Count; }
    }

    /// <summary>
    /// Inicializa a camada de neurônios
    /// </summary>
    /// <param name="id">Indentificador da camada</param>
    /// <param name="neuronCount">Quantidade de neurônios da camada</param>
    /// <param name="neuronInputSize">Tamanho da entrada de cada neurônio da camada</param>
    public NeuronLayer(int id, int neuronCount, int neuronInputSize)
    {
        this.id = id;
        this.neuronList = new List<Neuron>();

        for (int i = 0; i < neuronCount; i++)
        {
            this.neuronList.Add(new Neuron(i, neuronInputSize));
        }
    }

    /// <summary>
    /// Faz a ativação dos neurônios da camada
    /// </summary>
    /// <param name="input">Entrada para cada neurônio da camada</param>
    /// <returns>Saída dos neurônios da camada</returns>
    public double[] activate(double[] input)
    {
        double[] output = new double[neuronList.Count];

        for (int i = 0; i < neuronList.Count; i++)
        {
            output[i] = neuronList[i].activate(input);
        }

        return output;
    }
}
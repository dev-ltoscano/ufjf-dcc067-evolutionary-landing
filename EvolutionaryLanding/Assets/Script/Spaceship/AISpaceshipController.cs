using System;
using System.Collections.Generic;
using UnityEngine;

public class AISpaceshipController : MonoBehaviour
{
    // Intensidade da força do motor principal da nave
    public float mainBurnerForce = 100000.0f;

    public float auxBurnerForce = 10000.0f;

    // Intensidade do torque aplicado em todos os eixos da nave
    public float torqueForce = 500.0f;

    // Quantidade de combustível da nave
    public float amountFuel = 500.0f;

    // Componente RigidBody da nave
    private Rigidbody spaceshipRigidBody;

    // Sistema de partículas para o motor principal da nave
    public ParticleSystem mainBurnerParticleSystem;

    // Sistema de partículas para os motores auxiliares da nave
    public ParticleSystem auxBurnerParticleSystem1;
    public ParticleSystem auxBurnerParticleSystem2;
    public ParticleSystem auxBurnerParticleSystem3;
    public ParticleSystem auxBurnerParticleSystem4;

    // Sistema de partículas para explosão da nave
    public ParticleSystem explosionParticleSystem;

    // Plataforma de pouso para a nave
    public GameObject targetObject;
    
    private IEvolutionaryController evolutionController;
    private GameObject simulationInstance;
    private int instanceIndex;
    
    private MultiLayerPerceptron neuralNetwork;

    private float maxTestTime = 30.0f;
    private float maxTestTimeAfterStop = 5.0f;
    private float currentTestTime = 0.0f;
    private bool running = true;

    private float thresholdValue = 0.3f;

    private double[] activateNeuralNetwork()
    {
        Vector3 targetVector = (targetObject.transform.position - transform.position);
        Vector3 velocityVector = spaceshipRigidBody.velocity;
        Vector3 angleVector = spaceshipRigidBody.rotation.eulerAngles;

        Vector3 targetVectorNormalized = targetVector.normalized;
        Vector3 velocityNormalized = velocityVector.normalized;
        Vector3 anglesNormalized = angleVector.normalized;

        double[] input = new double[9];

        input[0] = targetVectorNormalized.x;
        input[1] = targetVectorNormalized.y;
        input[2] = targetVectorNormalized.z;
        input[3] = velocityNormalized.x;
        input[4] = velocityNormalized.y;
        input[5] = velocityNormalized.z;
        input[6] = anglesNormalized.x;
        input[7] = anglesNormalized.y;
        input[8] = anglesNormalized.z;

        return neuralNetwork.activate(input);
    }
    
    void Start()
    {
        this.spaceshipRigidBody = this.GetComponent<Rigidbody>();

        this.evolutionController = this.transform.parent.parent.GetComponent<IEvolutionaryController>();
        this.instanceIndex = this.evolutionController.getInstanceIndex();
        this.simulationInstance = this.evolutionController.getInstanceByIndex(instanceIndex);
        this.neuralNetwork = this.evolutionController.getGenomeByIndex(instanceIndex).buildNeuralNetwork();

        if(evolutionController.isRunning())
        {
            Vector3 padPosition = this.evolutionController.getPadPosition();
            this.targetObject.transform.position = new Vector3(targetObject.transform.position.x + padPosition.x, targetObject.transform.position.y + padPosition.y, targetObject.transform.position.z + padPosition.z);
        }
        else
        {
            this.targetObject.transform.position = this.evolutionController.getPadPosition();
        }
    }
    
    void FixedUpdate()
    {
        if(!running)
        {
            if (currentTestTime >= maxTestTimeAfterStop)
            {
                evolutionController.setFitness(simulationInstance, instanceIndex, float.MaxValue);
            }
        }
        else
        {
            if (currentTestTime >= maxTestTime)
            {
                evolutionController.setFitness(simulationInstance, instanceIndex, float.MaxValue);
            }
            else
            {
                double[] output = activateNeuralNetwork();

                int mainBurnerFactor = (output[0] >= 0.0f ? 1 : 0);

                int aux1BurnerFactor = (output[1] >= 0.0f ? -1 : 0);
                int aux2BurnerFactor = (output[2] >= 0.0f ? 1 : 0);
                int aux3BurnerFactor = (output[3] >= 0.0f ? -1 : 0);
                int aux4BurnerFactor = (output[4] >= 0.0f ? 1 : 0);

                int torqueXFactor = 0;

                if (output[5] >= thresholdValue)
                {
                    torqueXFactor = 1;
                }
                else if (output[5] <= -thresholdValue)
                {
                    torqueXFactor = -1;
                }

                int torqueYFactor = 0;

                if (output[6] >= thresholdValue)
                {
                    torqueYFactor = 1;
                }
                else if (output[6] <= -thresholdValue)
                {
                    torqueYFactor = -1;
                }

                int torqueZFactor = 0;

                if (output[7] >= thresholdValue)
                {
                    torqueZFactor = 1;
                }
                else if (output[7] <= -thresholdValue)
                {
                    torqueZFactor = -1;
                }

                Vector3 mainBurnerVector = new Vector3(0.0f, mainBurnerFactor * mainBurnerForce, 0.0f);

                Vector3 aux1BurnerVector = new Vector3((aux1BurnerFactor * auxBurnerForce), 0.0f, 0.0f);
                Vector3 aux2BurnerVector = new Vector3((aux2BurnerFactor * auxBurnerForce), 0.0f, 0.0f);
                Vector3 aux3BurnerVector = new Vector3(0.0f, 0.0f, (aux3BurnerFactor * auxBurnerForce));
                Vector3 aux4BurnerVector = new Vector3(0.0f, 0.0f, (aux4BurnerFactor * auxBurnerForce));

                Vector3 torqueXVector = new Vector3((torqueXFactor * torqueForce), 0.0f, 0.0f);
                Vector3 torqueYVector = new Vector3(0.0f, (torqueYFactor * torqueForce), 0.0f);
                Vector3 torqueZVector = new Vector3(0.0f, 0.0f, (torqueZFactor * torqueForce));

                if (amountFuel > 0)
                {
                    if (mainBurnerFactor == 1)
                    {
                        spaceshipRigidBody.AddRelativeForce(mainBurnerVector);
                        amountFuel -= 0.1f;
                        mainBurnerParticleSystem.Play();
                    }

                    if (aux1BurnerFactor == -1)
                    {
                        spaceshipRigidBody.AddRelativeForce(aux1BurnerVector);
                        amountFuel -= 0.1f;
                        auxBurnerParticleSystem1.Play();
                    }

                    if (aux2BurnerFactor == 1)
                    {
                        spaceshipRigidBody.AddRelativeForce(aux2BurnerVector);
                        amountFuel -= 0.1f;
                        auxBurnerParticleSystem2.Play();
                    }

                    if (aux3BurnerFactor == -1)
                    {
                        spaceshipRigidBody.AddRelativeForce(aux3BurnerVector);
                        amountFuel -= 0.1f;
                        auxBurnerParticleSystem3.Play();
                    }

                    if (aux4BurnerFactor == 1)
                    {
                        spaceshipRigidBody.AddRelativeForce(aux4BurnerVector);
                        amountFuel -= 0.1f;
                        auxBurnerParticleSystem4.Play();
                    }
                }

                spaceshipRigidBody.AddRelativeTorque(torqueXVector);
                spaceshipRigidBody.AddRelativeTorque(torqueYVector);
                spaceshipRigidBody.AddRelativeTorque(torqueZVector);
            }
        }

        currentTestTime += Time.deltaTime;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (running)
        {
            running = false;
            currentTestTime = 0.0f;

            if (collision.relativeVelocity.magnitude > 8)
            {
                explosionParticleSystem.Play();
            }

            Vector3 targetVector = (targetObject.transform.position - transform.position);
            Vector3 velocityVector = spaceshipRigidBody.velocity;
            Vector3 angleVector = spaceshipRigidBody.rotation.eulerAngles;

            float targetMagnitude = targetVector.magnitude;
            float velocityMagnitude = velocityVector.magnitude;
            float fitness = (targetMagnitude + velocityMagnitude);

            if (((angleVector.x > 5) && (angleVector.x < 355))
                || ((angleVector.z > 5) && (angleVector.z < 355)))
            {
                fitness += angleVector.magnitude;
            }

            if(collision.relativeVelocity.magnitude > 8)
            {
                fitness += collision.relativeVelocity.magnitude;
            }

            if (!evolutionController.isRunning())
            {
                Debug.Log("Target Vector: " + targetVector
                + " | Velocity Vector: " + velocityVector
                + " | Angle Vector: " + angleVector
                + " | Target Magnitude: " + targetMagnitude
                + " | Velocity Magnitude: " + velocityMagnitude
                + " | Fitness: " + fitness
                + " | Collision Magnitude: " + collision.relativeVelocity.magnitude);
            }
            else
            {
                evolutionController.setFitness(simulationInstance, instanceIndex, fitness);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipController : MonoBehaviour
{
    // Intensidade da força do motor principal da nave
    public float burnerForce = 80000.0f;

    // Intensidade do torque aplicado em todos os eixos da nave
    public float torqueForce = 250.0f;

    // Quantidade de combustível da nave
    public float amountFuel = 500.0f;
    
    // Componente RigidBody da nave
    private Rigidbody spaceshipRigidBody;

    // Sistema de partículas para o motor principal da nave
    private ParticleSystem burnerParticleSystem;

    // Sistema de partículas para explosão da nave
    private ParticleSystem explosionParticleSystem;

    // Plataforma de pouso para a nave
    public GameObject targetObject;

    /// <summary>
    /// Obtém um componente a partir de um nome de uma lista de componentes
    /// </summary>
    /// <param name="childrenComponents">Lista de componentes</param>
    /// <param name="name">Nome do componente</param>
    /// <returns>Componente com o nome indicado, nulo caso contrário</returns>
    private Component getComponentByName(Component[] childrenComponents, string name)
    {
        foreach (Component component in childrenComponents)
        {
            if (component.name == name)
            {
                return component;
            }
        }

        return null;
    }
    
    void Start ()
    {
        // Inicializa o RigidBody da nave
        this.spaceshipRigidBody = GetComponent<Rigidbody>();

        // Inicializa os sistemas de partículas
        Component[] childrenParticleSystems = GetComponentsInChildren<ParticleSystem>();
        this.burnerParticleSystem = (ParticleSystem)getComponentByName(childrenParticleSystems, "BurnerParticleSystem");
        this.explosionParticleSystem = (ParticleSystem)getComponentByName(childrenParticleSystems, "ExplosionParticleSystem");
    }
	
	void FixedUpdate()
    {
        if (amountFuel > 0)
        {
            // Ativa o motor principal
            if (Input.GetKey(KeyCode.Space))
            {
                spaceshipRigidBody.AddRelativeForce(Vector3.up * burnerForce);
                burnerParticleSystem.Play();
                amountFuel -= 0.1f;
            }
            else
            {
                burnerParticleSystem.Stop();
            }
        }
        else
        {
            burnerParticleSystem.Stop();
        }

        // Torques aplicados em cada eixo
        if (Input.GetKey(KeyCode.W))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3((1 * torqueForce), 0, 0));
        }

        if (Input.GetKey(KeyCode.S))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3((-1 * torqueForce), 0, 0));
        }

        if (Input.GetKey(KeyCode.A))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3(0, 0, (1 * torqueForce)));
        }

        if (Input.GetKey(KeyCode.D))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3(0, 0, (-1 * torqueForce)));
        }

        if (Input.GetKey(KeyCode.Q))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3(0, (1 * torqueForce), 0));
        }

        if (Input.GetKey(KeyCode.E))
        {
            spaceshipRigidBody.AddRelativeTorque(new Vector3(0, (-1 * torqueForce), 0));
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Vector3 targetVector = (targetObject.transform.position - transform.position);
        Vector3 velocityVector = spaceshipRigidBody.velocity;
        Vector3 angleVector = spaceshipRigidBody.rotation.eulerAngles;

        float targetMagnitude = targetVector.magnitude;
        float velocityMagnitude = velocityVector.magnitude;
        float fitness = targetMagnitude + velocityMagnitude;

        if (((angleVector.x > 5) && (angleVector.x < 355))
            || ((angleVector.z > 5) && (angleVector.z < 355)))
        {
            fitness += angleVector.magnitude;
        }

        if (collision.relativeVelocity.magnitude > 10)
        {
            fitness += collision.relativeVelocity.magnitude;
        }

        Debug.Log("Target Vector: " + targetVector
            + " | Velocity Vector: " + velocityVector
            + " | Angle Vector: " + angleVector
            + " | Target Magnitude: " + targetMagnitude
            + " | Velocity Magnitude: " + velocityMagnitude
            + " | Fitness: " + fitness
            + " | Collision Magnitude: " + collision.relativeVelocity.magnitude);

        if (collision.relativeVelocity.magnitude > 10)
        {
            explosionParticleSystem.Play();
            Destroy(gameObject, explosionParticleSystem.main.duration);
        }
    }
}

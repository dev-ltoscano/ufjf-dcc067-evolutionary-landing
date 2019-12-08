using System;
using System.Collections.Generic;
using UnityEngine;

interface IEvolutionaryController
{
    bool isRunning();

    int getInstanceIndex();

    GameObject getInstanceByIndex(int instanceIndex);

    Genome getGenomeByIndex(int instanceIndex);

    void setFitness(GameObject instance, int instanceIndex, float fitness);

    Vector3 getPadPosition();
}

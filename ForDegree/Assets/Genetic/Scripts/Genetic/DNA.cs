
using System.Collections.Generic;
using System;
namespace GeneticImplementation
{
    public class DNA<T>
    {
        public T[] Genes { get; set; }
        public float Fittnes { get; private set; }
        public Random random;
        private Func<T> getRandomGene;
        private Func<int, float> fittnesFunction;

       
        /// <summary>
        /// Construct new Generic Instance with custom Fitness
        /// and generate random Genes based on getRandomGene
        /// </summary>
        public DNA(
			int size, 
			Random random, 
			Func<T> getRandomGene, 
			Func<int, float> fitnessFunc, // takes int, returns float
			bool initGenes = true)
        {
            Genes = new T[size];
            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fittnesFunction = fitnessFunc;
            // Initialize
            if (initGenes)
            {
                for (int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = getRandomGene();
                }
            }
        }

        public float CalculateFitness(int index)
        {
            Fittnes = fittnesFunction(index);
            return Fittnes;
        }

        public DNA<T> CrossOver(DNA<T> otherParent)
        {
            DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fittnesFunction, false);

            // Interchange the genes from both parents
            for (int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = random.NextDouble() < 0.5f ? this.Genes[i] : otherParent.Genes[i];
            }
            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    Genes[i] = getRandomGene();
                }
            }
        }
    }

}
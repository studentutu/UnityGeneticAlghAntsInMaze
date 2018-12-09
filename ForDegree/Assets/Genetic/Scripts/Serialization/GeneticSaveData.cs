
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace GeneticImplementation
{
    [System.Serializable]
    public class GeneticSaveData<T>
    {
        public List<T[]> AllGenesFromPopulation;
        public int Generation;

        public void TakeFrom(GeneticAlghorithm<T> original)
        {
            this.Generation = original.Generation;
            this.AllGenesFromPopulation = new List<T[]>(original.Population.Count);
            for (int i = 0; i < original.Population.Count; i++)
            {
                this.AllGenesFromPopulation.Add(new T[original.Population[i].Genes.Length]);
                System.Array.Copy(
                    original.Population[i].Genes,
                    this.AllGenesFromPopulation[i],
                    original.Population[i].Genes.Length
                );
            }
        }
        public bool saveTo(string name)
        {
            // 1 Check folder
            if (!Directory.Exists(Application.persistentDataPath + "/Previous"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Previous");
            }

            // 2 check if file needs to be overwritten
            if (File.Exists(Application.persistentDataPath + "/Previous/" + name + ".data"))
            {
                name += "_copy";
            }

            // 3 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/Previous/" + name + ".data");
            bf.Serialize(file, this);
            file.Close();

            // 4
            if (File.Exists(Application.persistentDataPath + "/Previous/" + name + ".data"))
            {
                if (name.Contains("_copy"))
                {
                    int index = name.LastIndexOf("_copy");
                    name = name.Substring(0, index);
                    Debug.Log(name);
                    File.Replace(
                        Application.persistentDataPath + "/Previous/" + name + "_copy.data",
                        Application.persistentDataPath + "/Previous/" + name + ".data",
                        Application.persistentDataPath + "/Previous/" + name + "_copy2.data"
                    );
                    if (!File.Exists(Application.persistentDataPath + "/Previous/" + name + "_copy.data"))
                    {
                        File.Delete(Application.persistentDataPath + "/Previous/" + name + "_copy2.data");
                    }
                }
                return true;
            }
            return false;
        }

        public bool readFrom(string name)
        {
            // 1 Check folder
            if (!Directory.Exists(Application.persistentDataPath + "/Previous"))
            {
                return false;
            }
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Previous/" + name + ".data", FileMode.Open);
            GeneticSaveData<T> save = (GeneticSaveData<T>)bf.Deserialize(file);
            file.Close();
            if (save.AllGenesFromPopulation.Count > 0)
            {
                this.Generation = save.Generation;
                this.AllGenesFromPopulation = save.AllGenesFromPopulation;
                return true;
            }
            return false;
        }

    }

}

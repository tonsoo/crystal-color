using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveManager
{
    public static void Save(iteract_object_data[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/estatisticas.pi";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static iteract_object_data[] Load()
    {
        string path = Application.dataPath+ "/estatisticas.pi";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            iteract_object_data[] data = formatter.Deserialize(stream) as iteract_object_data[];

            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("Arquivo de estatisticas não foi encontrado!");
            return null;
        }
    }
}

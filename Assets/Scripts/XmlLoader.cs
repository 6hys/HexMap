using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class XmlLoader : MonoBehaviour
{
    public T Load<T>(T list, string fileName)
    {
        TextAsset xmlFile = Resources.Load<TextAsset>(fileName);

        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        StringReader sr = new StringReader(xmlFile.ToString());
        list = (T)xmlSerializer.Deserialize(sr);
        sr.Close();

        return list;
    }
}

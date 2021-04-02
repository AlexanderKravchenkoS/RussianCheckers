using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class Board : MonoBehaviour
{
    [SerializeField] GameObject WhitePrefab;
    [SerializeField] GameObject BlackPrefab;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(3.5f, 0, 3.5f);
        for (int z = 0; z < 3; z++)
        {
            int sideOffset = z % 2 == 0 ? 0 : 1;
            for (int x = sideOffset; x < 8; x += 2) 
            {
                Instantiate(WhitePrefab, new Vector3(x, WhitePrefab.transform.position.y, z), WhitePrefab.transform.rotation);
                Instantiate(BlackPrefab, new Vector3(7-x, BlackPrefab.transform.position.y, 7-z), BlackPrefab.transform.rotation);
            }
        }

        var data = FindObjectsOfType<Checker>().Select(x => x.Data).ToArray();
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.dataPath, "Start.json");
        File.WriteAllText(path, json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenController : MonoBehaviour
{
    [Header("Templates")]
    public List<TerrainTemplateController> terrainTemplates;
    //public float terrainTemplateWidth;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffset;
    public float areaEndOffset;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplates;

    private const float debugLineHeight = 10.0f;

    private List<GameObject> spawnedTerrain;
    private float lastGeneratedPositionY;
    private float lastRemovedPositionY;

    // pool list
    private Dictionary<string, List<GameObject>> pool;

    // Start is called before the first frame update
    void Start()
    {
        // Inisialisasi pool
        pool = new Dictionary<string, List<GameObject>>();

        spawnedTerrain = new List<GameObject>();

        //lastGeneratedPositionY = GetHorizontalPositionStart();
        //lastRemovedPositionY = lastGeneratedPositionY - terrainTemplates.terrainTemplateWidth;

        // Set posisi awal dengan posisi X dari prefab pertama dalam earlyTerrainTemplate
        if (earlyTerrainTemplates.Count > 0)
        {
            lastGeneratedPositionY = earlyTerrainTemplates[0].transform.position.y; // Mengambil posisi Y prefab pertama
        }
        else
        {
            lastGeneratedPositionY = GetHorizontalPositionStart(); // Jika tidak ada earlyTerrainTemplate, gunakan posisi start yang sesuai
        }
            lastRemovedPositionY = lastGeneratedPositionY - terrainTemplates[numPrefab].terrainTemplateWidth;

            // Generate terrain sesuai dengan earlyTerrainTemplates
            //foreach (TerrainTemplateController terrainEarly in earlyTerrainTemplates)
            //{
            //    GenerateTerrain(lastGeneratedPositionY, terrainEarly);
            //    lastGeneratedPositionY += terrainEarly.terrainTemplateWidth;
            //}

            // Generate terrain secara terus-menerus hingga mencapai area akhir
            while (lastGeneratedPositionY < GetHorizontalPositionEnd())
            {
                GenerateTerrain(lastGeneratedPositionY);
                lastGeneratedPositionY += terrainTemplates[numPrefab].terrainTemplateWidth;
            }
                
    }

    // Update is called once per frame
    void Update()
    {

            while (lastGeneratedPositionY < GetHorizontalPositionEnd())
            {
                GenerateTerrain(lastGeneratedPositionY);
                lastGeneratedPositionY += terrainTemplates[numPrefab].terrainTemplateWidth; //Debug.Log("Terrain Width: "+ terrainTemplates[numPrefab].terrainTemplateWidth);
            }

            while (lastRemovedPositionY + terrainTemplates[numPrefab].terrainTemplateWidth < GetHorizontalPositionStart())
            {
                lastRemovedPositionY += terrainTemplates[numPrefab].terrainTemplateWidth;
                RemoveTerrain(lastRemovedPositionY);
            }
        
        //Invoke("RemoveTerrain", 5f);
    }

    private int numPrefab;
    private void GenerateTerrain(float posX, TerrainTemplateController forceterrain = null)
    {
        //GameObject newTerrain = Instantiate(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        numPrefab = Random.Range(0, terrainTemplates.Count);
        // Pilih prefab terrain secara acak dari daftar template
        GameObject selectedTerrainPrefab = terrainTemplates[numPrefab].gameObject;

        // Instantiate prefab baru
        GameObject newTerrain = Instantiate(selectedTerrainPrefab, transform);

        // Atur posisi X dan Z sesuai dengan posX
        newTerrain.transform.position = new Vector3(posX, 0f, selectedTerrainPrefab.transform.position.z);
        //newTerrain.transform.position = new Vector2(posX, 0f);

        spawnedTerrain.Add(newTerrain);
    }

    
    public void RemoveTerrain(float posX)
    {
        
        GameObject terrainToRemove = null;

        // find terrain at posX
        foreach (GameObject item in spawnedTerrain)
        {
            if (item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }

        // after found;
        if (terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            Destroy(terrainToRemove,70f);
        }
    }

    private void GetTerrainWidth()
    {
        foreach (TerrainTemplateController terrain in terrainTemplates)
        {
            float terrainWidth = terrain.terrainTemplateWidth;
            // Lakukan sesuatu dengan nilai terrainWidth
        }
    }
    private float GetHorizontalPositionStart()
    {
        //return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffset;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }

    // pool function
    private GameObject GenerateFromPool(GameObject item, Transform parent)
    {
        if (pool.ContainsKey(item.name))
        {
            // if item available in pool
            if (pool[item.name].Count > 0)
            {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else
        {
            // if item list not defined, create new one
            pool.Add(item.name, new List<GameObject>());
        }

        // create new one if no item available in pool
        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if (!pool.ContainsKey(item.name))
        {
            Debug.LogError("INVALID POOL ITEM!!");
        }

        pool[item.name].Add(item);
        item.SetActive(false);
    }

    // debug
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }
}
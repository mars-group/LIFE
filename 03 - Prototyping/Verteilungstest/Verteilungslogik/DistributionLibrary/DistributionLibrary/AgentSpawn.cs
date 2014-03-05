using UnityEngine;
using Random = UnityEngine.Random;

public class AgentSpawn : MonoBehaviour
{
    public int AgentCount;

    public GameObject AgentPrefab;

    public Vector3 Bounds;

	// Use this for initialization
	void Start () {
	    for (int i = 0; i < AgentCount; i++)
	    {
	        Instantiate(AgentPrefab,
                new Vector3(Random.Range(-Bounds.x/2, Bounds.x/2), 1, Random.Range(-Bounds.z/2, Bounds.z/2)),
                Quaternion.identity);
	    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class SinusTest : MonoBehaviour
{

    private Vector3 startPosition;

	// Use this for initialization
	void Start ()
	{
	    startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	    transform.Translate(startPosition.x, Mathf.Sin(Time.time) * 0.1f , startPosition.z);
	}
}

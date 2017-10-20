using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager INSTANCE;

    public float delay = 10f;
    public GameObject emergent;
    public GameObject multiLevel;

    Agent[] agents;

    public Agent[] Agents {
        get;
        set;
    }

	// Use this for initialization
	void Start () {
		if (INSTANCE != null) {
            this.enabled = false;
            return;
        }
        INSTANCE = this;
        Agents = FindObjectsOfType<Agent>();
        Debug.Log(Agents.Length);
        StartCoroutine(WaitAndSpawnFlocks());
	}

    IEnumerator WaitAndSpawnFlocks() {
        yield return new WaitForSeconds(delay);
        emergent.SetActive(true);
        yield return new WaitForSeconds(delay);
        multiLevel.SetActive(true);
    }
}

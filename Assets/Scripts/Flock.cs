using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public enum State {
        Static,
        Scalable,
        Emergent,
        MultiLevel
    }

    public bool liveUpdate = false;

    public State state = State.Static;

    public Agent leader;
    public Agent[] flock;

    [Header("Helpers")]
    public float separationDist = 2f;

    // Use this for initialization
    void Start () {
        Updateformation();
	}
	
	// Update is called once per frame
	void Update () {
		if (liveUpdate) {
            Updateformation();
        }
	}

    void SetState(State s) {
        state = s;
        Updateformation();
    }

    void Updateformation() {
        switch (state) {
            case State.Static:
                FillStaticSpots();
                break;
            case State.Scalable:
                BuildScalable();
                break;
            case State.Emergent:
                SetEmergentLeader();
                break;
            case State.MultiLevel:
                BuildMultiLevel();
                break;
        }
    }

    void FillStaticSpots() {
        int count = flock.Length;
        Vector2 radius = leader.GetForwardVector()*(count*separationDist/2*Mathf.PI);
        Vector2 center = (Vector2)leader.transform.position - radius;
        Quaternion rot = Quaternion.AngleAxis(360f / count, Vector3.forward);
        foreach (Agent a in flock) {
            if (a == leader) {
                continue;
            }
            radius = rot * radius;
            Debug.Log(radius);
            a.Formate(center + radius);
        }
    }

    void BuildScalable() {

    }

    void SetEmergentLeader() {

    }

    void BuildMultiLevel() {

    }
}

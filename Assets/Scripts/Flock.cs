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
    float flockSpeed = 1f;

    private void Awake() {
        flockSpeed = leader.move_speed;
    }

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
        float maxDist = 0f;
        foreach (Agent a in flock) {
            if (a == leader || a.curState != Agent.State.formation) {
                continue;
            }
            radius = rot * radius;
            float tmp = a.Formate(center + radius);
            if (tmp > maxDist) {
                maxDist = tmp;
            }
        }
        leader.SetMaxSpeed(Mathf.Max(flockSpeed-maxDist/5f*flockSpeed,flockSpeed*.1f));
    }

    void BuildScalable() {

    }

    void SetEmergentLeader() {

    }

    void BuildMultiLevel() {

    }
}

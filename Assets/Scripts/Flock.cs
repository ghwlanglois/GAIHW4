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

    State state = State.Static;

    public GameObject leader;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetState(State s) {
        state = s;
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

    }

    void BuildScalable() {

    }

    void SetEmergentLeader() {

    }

    void BuildMultiLevel() {

    }
}

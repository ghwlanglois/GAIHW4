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
        leader.DisplayText.text = state.ToString();

    }
	
	// Update is called once per frame
	void Update () {
		if (liveUpdate) {
            Updateformation();
        }
        if (state == State.Emergent) {
            CalculateLeaderSpeed();
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

    }

    void BuildScalable() {
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

    void SetEmergentLeader() {
        Agent leftmost = leader;
        Agent rightmost = leader;
        int dir = -1;
        foreach (Agent a in flock) {
            if (a == leader) {
                continue;
            }
            if (dir < 0) {
                a.followTarget = leftmost.transform;
                a.followDirection = dir;
                leftmost = a;
                dir = 1;
            } else {
                a.followTarget = rightmost.transform;
                a.followDirection = dir;
                rightmost = a;
                dir = -1;
            }

        }
    }

    void CalculateLeaderSpeed() {
      
        float maxDist = 0f;
        foreach (Agent a in flock) {
            if (a == leader) {
                continue;
            }
            if (a.flockDistance > maxDist) {
                maxDist = a.flockDistance;
            }
        }
        leader.SetMaxSpeed(Mathf.Max(flockSpeed - maxDist / 5f * flockSpeed, flockSpeed * .1f));
    }

    void BuildMultiLevel() {
        int count = flock.Length;
        Vector2 radius = leader.GetComponent<Agent>().GetForwardVector() * (count * separationDist / 2 * Mathf.PI);
        Vector2 center = (Vector2)leader.transform.position;
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
    }
}

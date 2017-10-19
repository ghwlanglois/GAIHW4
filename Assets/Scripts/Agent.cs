using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Agent : MonoBehaviour {

    public enum State {
        wait,
        wander,
        pursue,
        evade,
        path,
        formation
    }
    
    public enum CollisionType {
        none,
        collisionPredict,
        coneCheck
    }

    public CollisionType collisionType;
    public State curState = State.wait;
    public Transform target;
    public Transform[] path;
    public float rotation_speed;
    public float move_speed;
    public float slow_down_dist;
    public float cone_angle;
    public float cone_distance;
    public float avoidanceForce;
    public int num_whiskers;
    public Transform followTarget = null;
    public int followDirection = 0;

    SpriteRenderer circle;
    LineRenderer line;
    Ray ray;
    private Vector3 startVertex;
    public Text DisplayText;
    Transform wander_target;
    int path_index = 0;
    Vector2 targetOffset = Vector2.zero;
    Vector3 dir = Vector3.zero;

    Rigidbody2D RB;

    public float flockDistance = 0;

    // Use this for initialization
    void Start() {
        DisplayText = GetComponent<Text>();
        line = GetComponent<LineRenderer>();
        RB = GetComponent<Rigidbody2D>();
        circle = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() { 
        switch (curState) {
            case State.wait:
                break;
            case State.wander:
                Wander();
                break;
            case State.pursue:
                Pursue();
                break;
            case State.evade:
                Evade();
                break;
            case State.path:
                FollowPath();
                break;
            case State.formation:
                break;
            default:
                Debug.LogError(string.Format("{0} is not a valid state", curState));
                SetState(State.wait);
                break;
        }
        if (curState != State.formation) {
            switch (collisionType) {
                case CollisionType.collisionPredict:
                    PredictCollision();
                    break;
                case CollisionType.coneCheck:
                    ConeCheck();
                    break;
            }
        }
    }

    public void SetState(State s) {
        Debug.Log(s);
        curState = s;
    }

    public void SetTarget(Transform t) {
        target = t;
    }

    void ShowLine(Vector3 t) {
        //ray = Camera.main.ScreenPointToRay(transform.position);
        //line.positionCount = 2;
        //line.SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0));
        //line.SetPosition(1, new Vector3(t.x, t.y, 0));
    }

    void Wander() {
        if (dir == Vector3.zero || Random.Range(0, 1.0f) > .99f) {
            NewDir();
        }

        ShowLine(transform.position + dir);
        RB.velocity = dir.normalized * move_speed / 2;
        RotateTowards(transform.position + dir);
    }

    public void NewDir() {
        dir = (Vector2)wander_target.position + Random.insideUnitCircle - (Vector2)transform.position;
    }

    void Pursue() {
        float distance = Vector2.Distance(target.position, transform.position);
        ShowLine(target.position);

        if (distance > .5f) {
            RotateTowards(target.position);
            RB.velocity = (target.position - transform.position).normalized * move_speed * Mathf.Min(distance / slow_down_dist, 1);
        }
        else {
            RB.velocity = Vector3.zero;
        }
    }

    public float Formate(Vector2 t) {
        float distance = Vector2.Distance(t, transform.position);
        
        RotateTowards(t);
        RB.velocity = (t - (Vector2)transform.position).normalized * move_speed ;

        switch (collisionType) {
            case CollisionType.collisionPredict:
                PredictCollision();
                break;
            case CollisionType.coneCheck:
                ConeCheck();
                break;
        }

        return distance;
    }

    void Evade() {
        Vector3 v = transform.position - target.position;
        RotateTowards(transform.position + v);
        ShowLine(transform.position + v);
        RB.velocity = v.normalized * Time.deltaTime * move_speed;
    }

    void FollowPath() {
        float minDist = float.MaxValue;
        int minI = 0; ;
        for (int i = 0; i < path.Length; i++ ) {
            if (minDist > Vector2.Distance(path[i].position, transform.position)) {
                minDist = Vector2.Distance(path[i].position, transform.position);
                minI = i;
            }
        }
        
        if (minI < path.Length - 1) {
            //Check if within range of path point to move to next point
            Vector3 targetPoint = path[minI + 1].position;
            float distance = distance = Vector2.Distance(target.position, transform.position);

            if (followTarget != null) {
                targetPoint = followTarget.position - followTarget.right * GetComponentInParent<Flock>().separationDist - followTarget.up * followDirection * GetComponentInParent<Flock>().separationDist/2;
                flockDistance = Vector3.Distance(targetPoint, transform.position);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, targetPoint-transform.position, Vector3.Distance(targetPoint, transform.position), ~(1 << 8));
                if (hit.collider != null && hit.collider.gameObject.name[0] != this.name[0]) {
                    targetPoint = path[minI+1].position;
                }
            }
            Debug.DrawRay(transform.position, targetPoint - transform.position, Color.blue, 0);

            RB.velocity = (targetPoint - transform.position + (Vector3)targetOffset).normalized * move_speed * Mathf.Min(flockDistance!=0?flockDistance:distance  / slow_down_dist, 1);
            RotateTowards(targetPoint+(Vector3)targetOffset);
        } else {
            Debug.Log(path[minI].name);
            SetTarget(path[path.Length - 1]);
            SetState(State.pursue);
        }
    }

    void RotateTowards(Vector3 position) {
        Vector3 offset = (position - transform.position).normalized;
        transform.right = Vector3.MoveTowards(transform.right, offset, Time.deltaTime * rotation_speed);
    }

    void PredictCollision() {
        foreach (Agent a in GameManager.INSTANCE.Agents) {
            if (a.name == this.name) continue;
            Vector2 dp = a.transform.position - transform.position;
            Vector2 dv = a.RB.velocity - RB.velocity;
            float t = -1 * Vector2.Dot(dp, dv) / Mathf.Pow(dv.magnitude, 2);
            if (t > 2f) return;
            Vector2 pc = (Vector2)transform.position + RB.velocity * t;
            Vector2 pt = (Vector2)a.transform.position + a.RB.velocity * t;
            if (Vector2.Distance(pc, pt) < 2 * transform.localScale.x) {
                Debug.Log(string.Format("{0} avoiding {1}", this.name, a.name));
                //RB.AddForce((RB.velocity - pc).normalized * avoidanceForce);
                RB.velocity = Vector3.RotateTowards(RB.velocity.normalized, Quaternion.AngleAxis(180, Vector3.forward) * (RB.velocity - pc).normalized * avoidanceForce, avoidanceForce, float.PositiveInfinity) * move_speed;
                return;
            }
        }
    }

    void ConeCheck() {
        
        Quaternion start_angle = Quaternion.AngleAxis(-1*cone_angle / 2, transform.forward);
        Quaternion step_angle = Quaternion.AngleAxis(cone_angle / num_whiskers, transform.forward);
        Vector2 direction = start_angle * transform.right;

        for (int i = 0; i < num_whiskers; ++i) {
            Debug.DrawRay(transform.position, direction.normalized*cone_distance, Color.white, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, cone_distance, ~(1<<11));
            if (hit.collider != null)
            {
                Debug.Log("Hit");
                GameObject hit_agent = hit.collider.gameObject;
                if (hit_agent.name != this.name) {
                    if (curState == State.formation) {
                        StopAllCoroutines();
                        StartCoroutine(PathAndFormate());
                        return;
                    }
                    Debug.Log(string.Format("{0} avoiding {1}", this.name, hit_agent.name));
                    RB.velocity = Vector3.RotateTowards(RB.velocity.normalized, Quaternion.AngleAxis(180, Vector3.forward) * (hit.point - RB.velocity).normalized, avoidanceForce, float.PositiveInfinity) * move_speed;
                    Debug.DrawRay(transform.position, Quaternion.AngleAxis(180, Vector3.forward) * (hit.point - RB.velocity).normalized * avoidanceForce, Color.red, 10);
                    
                }
            }

            direction = step_angle * direction;
        }
    }

    IEnumerator PathAndFormate() {
        SetState(State.path);
        Debug.Log("Pathing");
        yield return new WaitForSeconds(1f);
        SetState(State.formation);
    }
    
    float ApproxDistanceBetween(Agent a, Agent b) {
        float angle = Vector2.Angle(a.RB.velocity, b.RB.velocity)*Mathf.Rad2Deg;
        return angle - Vector2.Distance(a.transform.position, b.transform.position);
    }

    public Vector2 GetForwardVector() {
        return transform.right;
    }

    public void SetMaxSpeed(float s) {
        move_speed = s;
    }
}

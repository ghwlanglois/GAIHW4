using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour {

    Rigidbody2D rb;

    public float moveSpeed= 2f;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.velocity = Vector2.right * Input.GetAxis("Horizontal");
        rb.velocity += Vector2.up * Input.GetAxis("Vertical");
        rb.velocity = rb.velocity.normalized*moveSpeed;
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.GetComponent<Agent>() !=null) {
            Destroy(collision.gameObject);
    }
} 
}

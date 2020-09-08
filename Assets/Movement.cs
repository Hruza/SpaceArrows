using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Movement : MonoBehaviour
{

    Rigidbody2D rb;

    public Vector3 rot;
    
    public float speed;

    public float reloadTime;

    public GameObject projectile;

    public float projectileSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        lastShoot = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        lastShoot += Time.deltaTime;
        /*
        if (Input.GetKey(KeyCode.W)) Forward(1);
        if (Input.GetKey(KeyCode.A)) Rot(1);
        if (Input.GetKey(KeyCode.D)) Rot(-1);
        if (Input.GetKey(KeyCode.S)) Forward(-1);
        if (Input.GetKey(KeyCode.Mouse0)) Shoot();
        */
    }

    public int shootCount=0;
    public void Forward(float val) {
        val = Mathf.Clamp(val,-1,1);
        rb.AddRelativeForce(val*speed * Vector2.up);
    }

    public void Rot(float val) {
        val = Mathf.Clamp(val, -1, 1);
        transform.Rotate(val*rot*Time.fixedDeltaTime);
    }

    bool ready=true;
    public float lastShoot;
    public void Shoot() {
        if (ready)
        {
            lastShoot = -reloadTime;
            ready = false;
            shootCount++;
            GameObject pt = Instantiate(projectile, transform.position + (0.7f * transform.up*transform.localScale.y), transform.rotation);
            pt.GetComponent<Rigidbody2D>().velocity = projectileSpeed * transform.up;
            Invoke("Reload", reloadTime);
        }
    }

    void Reload() {
        ready = true;
    }
}

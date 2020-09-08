using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIcontroller : MonoBehaviour
{

    bool zoomed = false;

    GameObject camera;

    public Column forward;

    public Column rotate;

    public Column shoot;

    private void Start()
    {
        camera = Camera.main.gameObject;
    }

    Brain selected;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && zoomed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);

            if (hit2D.collider != null)
            {
                Debug.Log(hit2D.collider.name);
                selected = hit2D.collider.GetComponent<Brain>();
            }

        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !zoomed)
        {
            Plane plane = new Plane(Vector3.forward, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float enter = 0.0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 point = ray.GetPoint(enter);

                camera.transform.position = new Vector3(Round20(point.x) + 4, Round20(point.y), -10);
                camera.GetComponent<Camera>().orthographicSize = 10;
                zoomed = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && zoomed) {
            camera.transform.position = new Vector3(60,40, -10);
            camera.GetComponent<Camera>().orthographicSize = 50;
            zoomed = false;
        }

        if (selected != null) {
            forward.Value = (float)selected.values[0];
            rotate.Value = (float)selected.values[1];
            shoot.Value = (float)selected.values[2];
        }
    }

    public static float Round20(float x) {
        return Mathf.Round(x / 20f) * 20;
    }

    public Slider slider;
    public void ChangeTime() { 
        Time.timeScale=slider.value;
    }
}

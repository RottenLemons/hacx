using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Culprit : MonoBehaviour
{
    public Transform ShootPosition;
    public int currTarget = 1;
    public bool travelling = false;
    public bool below = true;
    public bool hit = false;

    public bool hitWindow1 = false;
    public bool hitWindow2 = false;
    public bool done = false;
    public bool fDone = false;
    public bool canShoot = false;

    public float launchAngleMax = 90f;
    public float launchAngleMin = -90f;
    public float angle;
    public float launchAngle;

    public float angle1;
    public Vector3 angles;

    public float hitSpeed1;
    public float hitSpeed2;

    public int iterations1;
    public int iterations2;
    public int maxIterations;
    public float dist = Mathf.Infinity;

    [Header("RowAndColumn")]
    public int row;
    public int column;
    

    public TMP_Text AccuracyText;

    Quaternion targetRotation;
    GameObject go;
    public GameObject Ball;

    

    public void Cleanup()
    {
        done = false;
        hitWindow1 = false; hitWindow2 = false;
        iterations1 = 0; iterations2 = 0;
        AccuracyText.gameObject.SetActive(false);
        travelling = false;
        hit = false;
        below = true;
        MainGameManager.instance.posPicker.transform.position = new Vector3(0, MainGameManager.instance.posPicker.transform.position.y, 0);
    }
    private void Awake()
    {
        iterations1 = 0;
        iterations2 = 0;
        maxIterations = 90;
    }

    private void Update()
    {
        if (fDone || !canShoot)
        {
/*            Debug.Log("a");
*/            return;
        }
            

        if (iterations1 >= maxIterations)
        {
            done = true;

            Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            ShootPosition.rotation = finalRotation;
            Destroy(go);
            go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
            go.GetComponent<Ball>().SetTarget();
            go.GetComponent<Ball>().final = true;
            travelling = true;
            canShoot = false;
            return;
        }


        if (hitWindow1)
        {
            done = true;
            angle = launchAngle;

            Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            ShootPosition.rotation = finalRotation;
            Destroy(go);
            go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
            go.GetComponent<Ball>().SetTarget();
            go.GetComponent<Ball>().final = true;
            travelling = true;
            canShoot = false;
            return;

        }

        if (!go)
        {
            return;
        }
            


        // Update algo to update angle 

        if (go.GetComponent<Rigidbody>().isKinematic)
        {
            /*if ((go.transform.position.x < MainGameManager.instance.posPicker.transform.position.x || go.transform.position.z < MainGameManager.instance.posPicker.transform.position.z))
            {
                launchAngleMax = angle;
            }
            else if(go.transform.position.x > MainGameManager.instance.posPicker.transform.position.x || go.transform.position.z > MainGameManager.instance.posPicker.transform.position.z)
            {
                launchAngleMin = angle;
            }
          
            angle = (launchAngleMin + launchAngleMax) * 0.5f;*/
            //Debug.Log(angle + " " + launchAngleMin + " " + launchAngleMax);
            if (iterations1 > 0)
            {
                if (Vector3.Distance(go.transform.position, MainGameManager.instance.posPicker.transform.position) < dist)
                {
                    dist = Vector3.Distance(go.transform.position, MainGameManager.instance.posPicker.transform.position);
                    angle = launchAngle;
                }
            }

            launchAngle = iterations1 * (launchAngleMax - launchAngleMin) / maxIterations;
    
            Quaternion tiltRotation = Quaternion.Euler(launchAngle, 0, 0);
            Quaternion finalRotation = targetRotation * tiltRotation;
            ShootPosition.rotation = finalRotation;
            Destroy(go);
            go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
            go.GetComponent<Ball>().SetTarget();
            travelling = true;
            canShoot = false;

            iterations1++;
        }
    }

    public void FireProjectileAt()
    {
        maxIterations = SettingsMenu.instance.GetMaxIterations();

 /*       targets = MainGameManager.instance.GetWindows();*/

        launchAngleMax = -90f;
 
/*        currTarget = MainGameManager.instance.posPicker;*/

        Vector3 dir = MainGameManager.instance.posPicker.transform.position - ShootPosition.position;

        targetRotation = Quaternion.LookRotation(dir); // Causes rotation such that it points at window

        angle = targetRotation.eulerAngles.x;
        launchAngleMin = angle;
        ShootPosition.rotation = targetRotation;

        go = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, ShootPosition.root);
        go.GetComponent<Ball>().SetTarget();
        travelling = true;
        canShoot = false;

        iterations1++;
    }
}

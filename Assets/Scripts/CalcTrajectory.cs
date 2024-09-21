using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CalcTrajectory : MonoBehaviour
{
    public List<Window> SelectedWindows;
    private List<GameObject> Culprits;

    public List<GameObject> ViableCulprits1;
    public List<GameObject> ViableCulprits2;
    public GameObject BallPrefab;

    bool launch1 = false;

    //int maxIteration = 10;

    public SettingsMenu SM;

    public GameObject SimUI;
    public TMP_Text maxiter;
    public TMP_Text iter1;
    public TMP_Text iter2;

    int currIter1 = 0;
    public void CleanUp()
    {
        ViableCulprits1.Clear();
        ViableCulprits2.Clear();
        currIter1 = 0;
    }

    public void CalculatePath()
    {
        maxiter.text = "Max Iterations: " + SettingsMenu.instance.GetMaxIterations().ToString();
        /*FindViableCulprits();*/
        ViableCulprits1 = MainGameManager.instance.GetCulprits();
        LaunchBalls(ViableCulprits1);
        launch1 = true;
    }

    public IEnumerator DelayedSortingOfCulprits()
    {
        yield return new WaitForSeconds(1);
        MainGameManager.instance.SwapToCamOverview();
        Debug.Log("SortedList!");
/*        MainGameManager.instance.SortDuplicateHits();
        MainGameManager.instance.ToggleBothWindowHavers();*/
        SettingsMenu.instance.CalculateAccuracies();
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            SM.Balls.Add(ball);
        }
    }
    private void Update()
    {

        if (launch1)
        {
            if (!CheckIsTravelling(ViableCulprits1))
            {
                SetCanShoot(ViableCulprits1);
                if (currIter1 < SettingsMenu.instance.GetMaxIterations()) {
                    currIter1++;
                    iter1.text = "Iterations: " + currIter1;
                }
            }
        }

        if (CheckIsWindowDone(ViableCulprits1) && launch1)
        {            
            launch1 = false;
            StartCoroutine(DelayedSortingOfCulprits());
        }

       /* if (launch2)
        {
            if (!CheckIsTravelling(ViableCulprits2))
            {
                SetCanShoot(ViableCulprits2);
                currIter2++;
                iter2.text = "2nd Window: " + currIter2;
            }
        }*/

/*        if (CheckIsWindowDone(ViableCulprits2) && launch2 && !launch1)
        {
            launch2 = false;
            StartCoroutine(DelayedSortingOfCulprits());
        }*/
    }
    bool CheckIsWindowDone(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();


            if(!curr.done)
            {
                return false;
            }
            

           
        }
        return true;
    }

    bool CheckIsTravelling(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();
            if (curr.travelling)
                return true;
        }
        return false;
    }

    void SetCanShoot(List<GameObject> culprits)
    {
        for (int i = 0; i < culprits.Count; ++i)
        {
            Culprit curr = culprits[i].GetComponent<Culprit>();
            curr.canShoot = true;
        }
    }
/*    void FindViableCulprits()
    {
        Culprits = MainGameManager.instance.GetCulprits();

        for (int i = 0; i < Culprits.Count; ++i)
        {
            Culprit currCulprit = Culprits[i].GetComponent<Culprit>();

            RaycastHit hit;

            Vector3 rayDir = MainGameManager.instance.posPicker.transform.position - currCulprit.ShootPosition.position;
            Debug.Log("AAA");
            if (Physics.Raycast(currCulprit.ShootPosition.position, rayDir, out hit))
            {
                Debug.Log(hit.point);
                if (hit.collider.tag != "Hitzone")
                    Debug.Log("bruh");
                    continue;
                ViableCulprits1.Add(Culprits[i]);
            }
        }
        
    }*/

    void LaunchBalls(List<GameObject> ViableCulprits)
    {
        for (int i = 0; i < ViableCulprits.Count; ++i)
        {
            Culprit currCulprit = ViableCulprits[i].GetComponent<Culprit>();
            currCulprit.FireProjectileAt();
        }
    }
}

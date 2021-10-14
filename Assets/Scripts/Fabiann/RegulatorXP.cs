using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegulatorXP : MonoBehaviour
{
    private int xpPoints;
    private int skillPoints = 0;
    [SerializeField] private int breadCrumbXP;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillPoint"))
        {
            Debug.Log("skillpoints before skillpoint pickup:" + skillPoints);
            other.gameObject.SetActive(false);
            skillPoints += 1;
            Debug.Log("skillpoints after skillpoint pickup:" + skillPoints);

        }
        if (other.gameObject.CompareTag("BreadCrumb"))
        {
            Debug.Log("xp before breadcrumb pickup:" + xpPoints);

            other.gameObject.SetActive(false);
            xpGain(breadCrumbXP);
            Debug.Log("xp after breadcrumb pickup:" + xpPoints);

        }
    }
    public void xpGain(int xpAmount)
    {
        xpPoints += xpAmount;
        //resterende xp gaat verloren nu
        //Debug.Log("xpGain()");
        //Debug.Log("xpPoints: " + xpPoints);
        //Debug.Log("skillPoints: " + skillPoints);

        if (xpPoints >= 10)
        {
            xpPoints = 0;
            skillPoints += 1;
        }
        Debug.Log("skillPoints += 1, skillpoints: " + skillPoints);
    }
}


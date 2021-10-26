using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegulatorXP : MonoBehaviour
{
    private int xpPoints;
    //public int skillPoints = 0;
    [SerializeField] private int breadCrumbXP;
    public SkillTreeReader skillTreeReader;

    void Awake()
    {

       // SkillTreeReader skillTreeReader;
    }
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillPoint"))
        {
            other.gameObject.SetActive(false);
            skillTreeReader.availablePoints += 1;

        }
        if (other.gameObject.CompareTag("BreadCrumb"))
        {

            other.gameObject.SetActive(false);
            xpGain(breadCrumbXP);

        }
    }
    public void xpGain(int xpAmount)
    {
        xpPoints += xpAmount;
        if (xpPoints >= 10)
        {
            xpPoints = 0;
            skillTreeReader.availablePoints += 1;
        }
        Debug.Log(skillTreeReader.availablePoints);
    }
}


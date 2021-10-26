using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegulatorXP : MonoBehaviour
{
    public SkillTreeReader skillTreeReader;
    private float xpPoints;
    [SerializeField] private float breadCrumbXP;
    [SerializeField] private float xpTreshold;
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
    public void xpGain(float xpAmount)
    {
        xpPoints += xpAmount;
        if (xpPoints >= xpTreshold)
        {
            xpPoints -= xpTreshold;
            skillTreeReader.availablePoints += 1;
        }
    }

}


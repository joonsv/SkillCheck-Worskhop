using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpRegulator : MonoBehaviour
{
    // Start is called before the first frame update
    private int xpPoints;
    private int skillPoints = 0;
    [SerializeField]private int breadCrumbXP;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SkillPoint"))
        {
            other.gameObject.SetActive(false);
            skillPoints += 1;
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
        //resterende xp gaat verloren nu
        Debug.Log("xpGain()");
        Debug.Log("xpPoints: " + xpPoints);
        Debug.Log("skillPoints: " + skillPoints);

        if (xpPoints >= 10)
        {
            xpPoints = 0;
            skillPoints += 1;
        }
    }
}

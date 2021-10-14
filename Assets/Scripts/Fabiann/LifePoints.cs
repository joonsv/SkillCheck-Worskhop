using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePoints : MonoBehaviour
{
    [SerializeField] int hitPoints;
    [SerializeField] int enemyXP;
    private Player playerScript;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerScript =  player.GetComponent<Player>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            OnHit();
        }
        //met bullet tag gaat hp var naar beneden
        //als hp == 0; destory && xp += 1

    }
    private void OnHit()
    {
        Debug.Log("OnHit()");

        hitPoints -= 1;
        Debug.Log("hitPoints enemy: "+ hitPoints);

        if (hitPoints <= 0)

        {
            playerScript.xpGain(enemyXP);
            Destroy(gameObject);
        }
    }
}

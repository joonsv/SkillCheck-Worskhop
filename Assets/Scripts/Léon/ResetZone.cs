using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZone : MonoBehaviour
{
    [SerializeField] private float WaitTime = 1f;
    private Renderer ZoneRendeder;
    private Collider ZoneCollider;

    private void Start()
    {
        ZoneRendeder = this.GetComponent<Renderer>();
        ZoneCollider = this.GetComponent<Collider>();
        if(WaitTime != 0)
        {
            StartCoroutine(NotActive());
        }
    }

    private IEnumerator NotActive()
    {
        ZoneRendeder.enabled = false;
        ZoneCollider.enabled = false;
        yield return new WaitForSecondsRealtime(WaitTime);
        StartCoroutine(Active());
        yield break;
    }

    private IEnumerator Active()
    {
        ZoneRendeder.enabled = true;
        ZoneCollider.enabled = true;
        yield return new WaitForSecondsRealtime(WaitTime);
        StartCoroutine(NotActive());
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.position = new Vector3(0, 2, 0);
        }
    }
}

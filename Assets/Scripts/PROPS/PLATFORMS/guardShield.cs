using System.Collections;
using UnityEngine;

public class guardShield : MonoBehaviour
{
    private Animator guardAnimator;
    public GameObject shield;
    private BoxCollider2D shieldCollider;
    private bool canPush = true; 

    private void Start()
    {
        guardAnimator = GetComponent<Animator>();
        shieldCollider = shield.GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && canPush)
        {
            Debug.Log("detecto player");
            StartCoroutine(Push());
        }
    }
    private IEnumerator Push()
    {
        canPush = false;
        guardAnimator.SetTrigger("guardTouch");
        yield return new WaitForSecondsRealtime(0.75f);
        guardAnimator.SetTrigger("guardPush");
        //shieldCollider.isTrigger = true;
        shield.layer = 0;
        StartCoroutine(RecoverPush());

    }
    private IEnumerator RecoverPush()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        shield.layer = 26;
        //shieldCollider.isTrigger = false;
        canPush = true;
    }
}

using System;
using UnityEngine;

public class HiddenAreas : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float velocidad = 1f;
    [SerializeField] private bool oneTimeTrigger;
    private bool unlocked;

    private float targetAlpha = 1f;
    

    void Update()
    {
        Color c = sprite.color;
        c.a = Mathf.MoveTowards(c.a, targetAlpha, velocidad * Time.deltaTime);
        sprite.color = c;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetAlpha = 0f;
            if(oneTimeTrigger)
            {
                unlocked = true;
            }
        }
            
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !unlocked )
        {
            targetAlpha = 1f;
        }
            
    }
}
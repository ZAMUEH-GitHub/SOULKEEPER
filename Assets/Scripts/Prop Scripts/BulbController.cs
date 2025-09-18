using UnityEngine;

public class BulbController : MonoBehaviour
{
    public ParticleSystem bulbHitParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player Attack Collider"))
        {
            Instantiate(bulbHitParticles, transform.position, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == ("Player"))
        {
            Instantiate(bulbHitParticles, transform.position, Quaternion.identity);
        }
    }
}

using UnityEngine;
using System.Collections;

public class WallDoorController : MonoBehaviour, IUnlockable
{
    private bool hasOpened = false;

    public void Unlock(bool isUnlocked)
    {
        if (isUnlocked && !hasOpened)
        {
            hasOpened = true;
            StartCoroutine(MoveUp());
        }
    }

    private IEnumerator MoveUp()
    {
        float duration = 5f;
        float timer = 0f;

        while (timer < duration)
        {
            transform.Translate(Vector2.up * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}

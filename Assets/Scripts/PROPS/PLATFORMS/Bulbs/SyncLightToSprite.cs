using UnityEngine;
using UnityEngine.Rendering.Universal; // Required to access the Light2D component

public class SyncLightToSprite : MonoBehaviour
{
    [Header("Component References")]
    public SpriteRenderer bulbSpriteRenderer;
    public Light2D bulbLight2D;

    [Header("Sprite Sync Arrays")]
    [Tooltip("Place the frames from the bulb animation here.")]
    public Sprite[] bulbSprites;

    [Tooltip("Place the corresponding Light2D cookie sprites here (must match the order above).")]
    public Sprite[] lightSprites;

    private Sprite previousBulbSprite;

    void Start()
    {
        // Automatically grab the parent's SpriteRenderer if not manually assigned
        if (bulbSpriteRenderer == null && transform.parent != null)
        {
            bulbSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        }

        // Automatically grab the Light2D on this object if not manually assigned
        if (bulbLight2D == null)
        {
            bulbLight2D = GetComponent<Light2D>();
        }
    }

    // Using LateUpdate ensures the animation has already changed the parent's sprite for this frame
    void LateUpdate()
    {
        if (bulbSpriteRenderer == null || bulbLight2D == null) return;

        Sprite currentBulbSprite = bulbSpriteRenderer.sprite;

        // Only search the array and update if the animation actually changed the sprite this frame
        if (currentBulbSprite != previousBulbSprite)
        {
            UpdateLightSprite(currentBulbSprite);
            previousBulbSprite = currentBulbSprite;
        }
    }

    void UpdateLightSprite(Sprite currentSprite)
    {
        // Loop through the bulb sprites to find the current frame
        for (int i = 0; i < bulbSprites.Length; i++)
        {
            if (bulbSprites[i] == currentSprite)
            {
                // Ensure the lightSprites array has a matching index to prevent OutOfBounds errors
                if (i < lightSprites.Length)
                {
                    // Update the Light2D cookie sprite
                    bulbLight2D.lightCookieSprite = lightSprites[i];
                }
                return; // Stop searching once we've found the match
            }
        }
    }
}
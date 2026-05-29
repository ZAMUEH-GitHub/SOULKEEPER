using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public struct DepthZone
{
    public string sortingLayerName;
    [Tooltip("The exact Z position where this layer exists in your world.")]
    public float zPosition;
}

public class GodivaMovement : MonoBehaviour
{
    [Header("Navigation Settings")]
    [Tooltip("Drag the GodivaAnimatedTarget here from the Player hierarchy.")]
    public Transform animatedTarget;

    [Header("Rubber Band Dynamics")]
    [Tooltip("How long it takes Godiva to reach the target. Higher number = looser spring.")]
    public float smoothTime = 0.2f;
    [Tooltip("The absolute maximum speed Godiva can travel to catch up.")]
    public float maxSpeed = 40f;

    [Header("Visuals & Lighting")]
    public SpriteRenderer godivaSprite;
    public Light2D godivaLight;

    [Header("Depth Zones")]
    [Tooltip("Map your Sorting Layers to their physical Z positions here.")]
    public DepthZone[] depthZones;

    private Vector3 currentVelocity = Vector3.zero;

    private string currentActiveLayer = "";

    private void Start()
    {
        godivaSprite = GetComponentInChildren<SpriteRenderer>();
        godivaLight = GetComponentInChildren<Light2D>();
    }

    void LateUpdate()
    {
        if (animatedTarget == null) return;

        transform.position = Vector3.SmoothDamp(transform.position, animatedTarget.position, ref currentVelocity, smoothTime, maxSpeed);

        UpdateSortingLayerByZ();
    }

    private void UpdateSortingLayerByZ()
    {
        if (depthZones == null || depthZones.Length == 0) return;

        string closestLayer = currentActiveLayer;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < depthZones.Length; i++)
        {
            float distance = Mathf.Abs(transform.position.z - depthZones[i].zPosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLayer = depthZones[i].sortingLayerName;
            }
        }

        if (closestLayer != currentActiveLayer && !string.IsNullOrEmpty(closestLayer))
        {
            currentActiveLayer = closestLayer;
            ApplySortingLayer(closestLayer);
        }
    }

    private void ApplySortingLayer(string layerName)
    {
        if (godivaSprite != null)
        {
            godivaSprite.sortingLayerName = layerName;
        }

        if (godivaLight != null)
        {
            SortingLayer[] allLayers = SortingLayer.layers;
            int targetIndex = -1;

            for (int i = 0; i < allLayers.Length; i++)
            {
                if (allLayers[i].name == layerName)
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex != -1)
            {
                int[] layersToIlluminate = new int[targetIndex + 1];
                for (int i = 0; i <= targetIndex; i++)
                {
                    layersToIlluminate[i] = allLayers[i].id;
                }

                godivaLight.targetSortingLayers = layersToIlluminate;
            }
        }
    }
}
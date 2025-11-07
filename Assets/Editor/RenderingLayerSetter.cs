using UnityEngine;

public class RenderingLayerSetter : MonoBehaviour
{
    [Header("Rendering Layer Settings")]
    [Tooltip("Target Rendering Layer Index (0 = Default, 1 = Layer1, 2 = Layer2, dst.)")]
    public int targetLayerIndex = 0;

    [Tooltip("Sprite Mask Interaction setting for all children")]
    public SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;

    [Header("Sorting Settings")]
    [Tooltip("Sorting Layer Name (pastikan sesuai dengan yang ada di Project Settings > Tags and Layers)")]
    public string sortingLayerName = "Default";

    [Tooltip("Order in Layer for all children")]
    public int orderInLayer = 0;

    [ContextMenu("Apply Rendering Layer, Mask Interaction, Sorting Layer & Order In Layer To Children")]
    private void ApplyRenderingLayer()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        int count = 0;

        foreach (var sr in renderers)
        {
            // ✅ hanya aktifkan target rendering layer
            sr.renderingLayerMask = (uint)(1 << targetLayerIndex);

            // ✅ atur mask interaction
            sr.maskInteraction = maskInteraction;

            // ✅ atur sorting layer & order in layer
            sr.sortingLayerName = sortingLayerName;
            sr.sortingOrder = orderInLayer;

            count++;
        }

        Debug.Log($"[RenderingLayerSetter] Applied target layer {targetLayerIndex}, sorting layer '{sortingLayerName}', and order {orderInLayer} to {count} SpriteRenderer(s).", this);
    }
        [ContextMenu("Increase Order In Layer By 1")]
    private void IncreaseOrderInLayer()
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        int count = 0;

        foreach (var sr in renderers)
        {
            sr.sortingOrder += 1;
            count++;
        }

        Debug.Log($"[RenderingLayerSetter] Increased sortingOrder by 1 for {count} SpriteRenderer(s).", this);
    }
}

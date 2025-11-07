using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [FoldoutGroup("Movement Settings")]
    [SerializeField,ReadOnly]private float moveSpeed = 5f;

    [FoldoutGroup("Movement Settings")]
    public float jumpForce = 7f;

    [FoldoutGroup("Combat Settings")]
    public int attackPower = 10;

    [FoldoutGroup("Combat Settings")]
    public float attackCooldown = 1.5f;

    [FoldoutGroup("Inventory")]
    public string[] items;

    public bool showDebug;
}

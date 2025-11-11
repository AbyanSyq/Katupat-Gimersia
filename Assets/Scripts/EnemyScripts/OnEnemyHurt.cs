using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/OnEnemyHurt")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "OnEnemyHurt", message: "Get Hurt", category: "Events", id: "fb35c54d7d2f4b119ed824ed0450b5e6")]
public sealed partial class OnEnemyHurt : EventChannel { }


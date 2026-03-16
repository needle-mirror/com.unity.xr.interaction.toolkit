using System;
using UnityEngine;

/// <summary>
/// A component that automatically sets the GameObject to <c>DontDestroyOnLoad</c>.
/// </summary>
public class DontDestroyOnLoadSample : MonoBehaviour
{
    /// <summary>
    /// See <see cref="MonoBehaviour"/>.
    /// </summary>
    void Awake()
    {
        // Ensure this component is on the root GameObject.
        // See https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html

        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);
        else
            Debug.LogWarning("DontDestroyOnLoad only works for root GameObjects, this" +
                $" component is unable to set it for the {name} GameObject.", this);
    }
}

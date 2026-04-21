using UnityEngine;

/// <summary>
/// A component that automatically sets the GameObject to <c>DontDestroyOnLoad</c>.
/// </summary>
class DontDestroyOnLoadSample : MonoBehaviour
{
    void Awake()
    {
        // Ensure this component is on the root GameObject.
        if (transform.parent == null)
            DontDestroyOnLoad(gameObject);
        else
            Debug.LogWarning("DontDestroyOnLoad only works for root GameObjects," +
                $" can't apply it to the {name} GameObject.", this);
    }
}

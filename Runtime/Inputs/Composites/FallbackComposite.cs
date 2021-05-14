using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
    /// <summary>
    /// A single <c>Vector3</c> value, such as a position, computed from an ordered list of bindings.
    /// The value will be read from the first binding that has a valid control.
    /// </summary>
    /// <inheritdoc />
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    public class Vector3FallbackComposite : FallbackComposite<Vector3>
    {
        /// <summary>
        /// The first input control to evaluate.
        /// </summary>
        [InputControl(layout = "Vector3")]
        public int first;

        /// <summary>
        /// The second input control to evaluate.
        /// </summary>
        [InputControl(layout = "Vector3")]
        public int second;

        /// <summary>
        /// The third input control to evaluate.
        /// </summary>
        [InputControl(layout = "Vector3")]
        public int third;

        /// <inheritdoc />
        public override Vector3 ReadValue(ref InputBindingCompositeContext context)
        {
            var value = context.ReadValue<Vector3, Vector3MagnitudeComparer>(first, out var sourceControl);
            if (sourceControl != null)
                return value;

            value = context.ReadValue<Vector3, Vector3MagnitudeComparer>(second, out sourceControl);
            if (sourceControl != null)
                return value;

            value = context.ReadValue<Vector3, Vector3MagnitudeComparer>(third);
            return value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
#pragma warning disable IDE0051 // Remove unused private members
        // ReSharper disable once UnusedMember.Local
        static void Initialize()
#pragma warning restore IDE0051
        {
            // Will execute the static constructor as a side effect.
        }

        [Preserve]
        static Vector3FallbackComposite()
        {
            InputSystem.InputSystem.RegisterBindingComposite<Vector3FallbackComposite>();
        }
    }

    /// <summary>
    /// A single <c>Quaternion</c> value, such as a rotation, computed from an ordered list of bindings.
    /// The value will be read from the first binding that has a valid control.
    /// </summary>
    /// <inheritdoc />
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [Preserve]
    public class QuaternionFallbackComposite : FallbackComposite<Quaternion>
    {
        /// <summary>
        /// The first input control to evaluate.
        /// </summary>
        [InputControl(layout = "Quaternion")]
        public int first;

        /// <summary>
        /// The second input control to evaluate.
        /// </summary>
        [InputControl(layout = "Quaternion")]
        public int second;

        /// <summary>
        /// The third input control to evaluate.
        /// </summary>
        [InputControl(layout = "Quaternion")]
        public int third;

        /// <inheritdoc />
        public override Quaternion ReadValue(ref InputBindingCompositeContext context)
        {
            var value = context.ReadValue<Quaternion, QuaternionCompositeComparer>(first, out var sourceControl);
            if (sourceControl != null)
                return value;

            value = context.ReadValue<Quaternion, QuaternionCompositeComparer>(second, out sourceControl);
            if (sourceControl != null)
                return value;

            value = context.ReadValue<Quaternion, QuaternionCompositeComparer>(third);
            return value;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad), Preserve]
#pragma warning disable IDE0051 // Remove unused private members
        // ReSharper disable once UnusedMember.Local
        static void Initialize()
#pragma warning restore IDE0051
        {
            // Will execute the static constructor as a side effect.
        }

        [Preserve]
        static QuaternionFallbackComposite()
        {
            InputSystem.InputSystem.RegisterBindingComposite<QuaternionFallbackComposite>();
        }
    }

    /// <summary>
    /// Base class for a composite that returns a single value computed from an ordered list of bindings.
    /// </summary>
    /// <typeparam name="TValue">Type of value returned by the composite.</typeparam>
    /// <remarks>
    /// This composite allows for defining multiple binding paths, but unlike a Value action with
    /// multiple bindings which uses control magnitude to select the active control, this composite
    /// will use an ordered priority list of bindings. If the first input binding is not bound to
    /// an input control, it will fallback to try the second input binding, and so on.
    /// </remarks>
    [Preserve]
    public abstract class FallbackComposite<TValue> : InputBindingComposite<TValue>
        where TValue : struct
    {
        internal struct QuaternionCompositeComparer : IComparer<Quaternion>
        {
            public int Compare(Quaternion x, Quaternion y)
            {
                if (x == Quaternion.identity)
                {
                    if (y == Quaternion.identity)
                        return 0;
                    return -1;
                }

                return 1;
            }
        }
    }
}
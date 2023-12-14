using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
    /// <summary>
    /// A cache for a Unity Object reference that is used to avoid the overhead of the Unity Object alive check
    /// every time the reference is accessed after the first time.
    /// </summary>
    /// <typeparam name="T">The type of the serialized field.</typeparam>
    class UnityObjectReferenceCache<T> where T : Object
    {
        T m_CapturedField;
        T m_FieldOrNull;

        /// <summary>
        /// A fast but unsafe method for getting the field without doing a Unity Object alive check after the first time.
        /// The purpose is to avoid the overhead of the Unity Object alive check when it is known that the reference is alive,
        /// but does not have the rigor of detecting when the Object is deleted or destroyed after the first check, which should be very rare.
        /// This will handle the user modifying the field in the Inspector window by invalidating the cached version.
        /// </summary>
        /// <param name="field">The serialized field to get.</param>
        /// <param name="fieldOrNull">The Unity Object or actual <see langword="null"/>.</param>
        /// <returns>Returns <see langword="true"/> if the reference is not null. Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGet(T field, out T fieldOrNull)
        {
            if (ReferenceEquals(m_CapturedField, field))
            {
                fieldOrNull = m_FieldOrNull;
#pragma warning disable UNT0029 // bypass Unity Object alive check
                return m_FieldOrNull is not null;
#pragma warning restore UNT0029
            }

            m_CapturedField = field;
            if (field != null)
            {
                m_FieldOrNull = field;
                fieldOrNull = field;
                return true;
            }

            m_FieldOrNull = null;
            fieldOrNull = null;
            return false;
        }
    }

    /// <summary>
    /// A cache for a serialized Unity Object reference that represents an interface type.
    /// </summary>
    /// <typeparam name="TInterface">Interface that the reference Unity Object should implement.</typeparam>
    /// <typeparam name="TObject">Serialized field type, usually Unity <see cref="Object"/>.</typeparam>
    /// <seealso cref="RequireInterfaceAttribute"/>
    class UnityObjectReferenceCache<TInterface, TObject> where TInterface : class where TObject : Object
    {
        TObject m_CapturedObject;
        TInterface m_Interface;

        /// <summary>
        /// Gets the interface-typed Object reference.
        /// </summary>
        /// <param name="field">The serialized field to get.</param>
        /// <returns>Returns the interface-typed Object reference, which may be <see langword="null"/>.</returns>
        public TInterface Get(TObject field)
        {
            if (ReferenceEquals(m_CapturedObject, field))
                return m_Interface;

            m_CapturedObject = field;
            m_Interface = field as TInterface;

            return m_Interface;
        }

        /// <summary>
        /// Sets the Object reference to the interface-typed reference.
        /// </summary>
        /// <param name="field">The serialized field to set.</param>
        /// <param name="value">The interface-typed value.</param>
        // ReSharper disable once RedundantAssignment -- ref field is used to update the serialized field with the new value
        public void Set(ref TObject field, TInterface value)
        {
            field = value as TObject;
            m_CapturedObject = field;
            m_Interface = value;
        }
    }
}

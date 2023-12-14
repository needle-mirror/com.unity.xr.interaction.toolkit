using System;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
    static class InputActionUtility
    {
        public static InputAction CreateValueAction(Type valueType, string name = null)
        {
            return new InputAction(name, expectedControlType: GetExpectedControlType(valueType));
        }

        public static InputAction CreateButtonAction(string name = null, bool wantsInitialStateCheck = false)
        {
            return new InputAction(name, type: InputActionType.Button) { wantsInitialStateCheck = wantsInitialStateCheck };
        }

        public static InputAction CreatePassThroughAction(Type valueType = null, string name = null, bool wantsInitialStateCheck = false)
        {
            return new InputAction(name, type: InputActionType.PassThrough, expectedControlType: GetExpectedControlType(valueType)) { wantsInitialStateCheck = wantsInitialStateCheck };
        }

        static string GetExpectedControlType(Type valueType)
        {
            switch (valueType)
            {
                case not null when valueType == typeof(float):
                    return "Axis";

                case not null when valueType == typeof(int):
                case not null when valueType == typeof(InputTrackingState):
                    return "Integer";

                case not null when valueType == typeof(Quaternion):
                    return "Quaternion";

                case not null when valueType == typeof(Vector2):
                    return "Vector2";

                case not null when valueType == typeof(Vector3):
                    return "Vector3";
            }

            return null;
        }
    }
}

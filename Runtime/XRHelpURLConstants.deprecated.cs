using System;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Audio;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Transformation;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.UI;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Audio;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Theme.Primitives;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.Interaction.Toolkit.Transformers;

namespace UnityEngine.XR.Interaction.Toolkit
{
    static partial class XRHelpURLConstants
    {
        /// <summary>
        /// Scripting API URL for <see cref="XRRig"/>.
        /// </summary>
        [Obsolete("k_XRRig is now deprecated since XRRig was replaced by XROrigin. Please use documentation from com.unity.xr.core-utils instead.", true)]
        public const string k_XRRig = k_BaseApi + k_BaseNamespace + nameof(XRRig) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="XRLegacyGrabTransformer"/>.
        /// </summary>
        [Obsolete("k_XRLegacyGrabTransformer is now deprecated since XRLegacyGrabTransformer was deprecated.", true)]
        public const string k_XRLegacyGrabTransformer = k_BaseApi + k_BaseNamespace + "Transformers." + nameof(XRLegacyGrabTransformer) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="LocomotionSystem"/>.
        /// </summary>
        [Obsolete("k_LocomotionSystem has been deprecated in version 3.0.0.")]
        public const string k_LocomotionSystem = k_BaseApi + k_BaseNamespace + nameof(LocomotionSystem) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="XRController"/>.
        /// </summary>
        [Obsolete("k_XRController has been deprecated in version 3.0.0.")]
        public const string k_XRController = k_BaseApi + k_BaseNamespace + nameof(XRController) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="XRScreenSpaceController"/>.
        /// </summary>
        [Obsolete("k_XRScreenSpaceController has been deprecated in version 3.0.0.")]
        public const string k_XRScreenSpaceController = k_BaseApi + k_BaseNamespace + nameof(XRScreenSpaceController) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="DeviceBasedContinuousMoveProvider"/>.
        /// </summary>
        [Obsolete("k_DeviceBasedContinuousMoveProvider has been deprecated in version 3.0.0.")]
        public const string k_DeviceBasedContinuousMoveProvider = k_BaseApi + k_BaseNamespace + nameof(DeviceBasedContinuousMoveProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="DeviceBasedContinuousTurnProvider"/>.
        /// </summary>
        [Obsolete("k_DeviceBasedContinuousTurnProvider has been deprecated in version 3.0.0.")]
        public const string k_DeviceBasedContinuousTurnProvider = k_BaseApi + k_BaseNamespace + nameof(DeviceBasedContinuousTurnProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="DeviceBasedSnapTurnProvider"/>.
        /// </summary>
        [Obsolete("k_DeviceBasedSnapTurnProvider has been deprecated in version 3.0.0.")]
        public const string k_DeviceBasedSnapTurnProvider = k_BaseApi + k_BaseNamespace + nameof(DeviceBasedSnapTurnProvider) + ".html";

                /// <summary>
        /// Scripting API URL for <see cref="ActionBasedContinuousMoveProvider"/>.
        /// </summary>
        [Obsolete("k_ActionBasedContinuousMoveProvider has been deprecated in version 3.0.0.")]
        public const string k_ActionBasedContinuousMoveProvider = k_BaseApi + k_BaseNamespace + nameof(ActionBasedContinuousMoveProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ActionBasedContinuousTurnProvider"/>.
        /// </summary>
        [Obsolete("k_ActionBasedContinuousTurnProvider has been deprecated in version 3.0.0.")]
        public const string k_ActionBasedContinuousTurnProvider = k_BaseApi + k_BaseNamespace + nameof(ActionBasedContinuousTurnProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ActionBasedController"/>.
        /// </summary>
        [Obsolete("k_ActionBasedController has been deprecated in version 3.0.0.")]
        public const string k_ActionBasedController = k_BaseApi + k_BaseNamespace + nameof(ActionBasedController) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ActionBasedSnapTurnProvider"/>.
        /// </summary>
        [Obsolete("k_ActionBasedSnapTurnProvider has been deprecated in version 3.0.0.")]
        public const string k_ActionBasedSnapTurnProvider = k_BaseApi + k_BaseNamespace + nameof(ActionBasedSnapTurnProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="AudioAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_AudioAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Audio." + nameof(AudioAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ColorAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_ColorAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(ColorAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="FloatAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_FloatAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(FloatAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="QuaternionAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_QuaternionAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(QuaternionAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="QuaternionEulerAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_QuaternionEulerAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(QuaternionEulerAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector2AffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector2AffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(Vector2AffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector3AffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector3AffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(Vector3AffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector4AffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector4AffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Primitives." + nameof(Vector4AffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="BlendShapeAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_BlendShapeAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(BlendShapeAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ColorGradientLineRendererAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_ColorGradientLineRendererAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(ColorGradientLineRendererAffordanceReceiver) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="ColorMaterialPropertyAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_ColorMaterialPropertyAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(ColorMaterialPropertyAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="FloatMaterialPropertyAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_FloatMaterialPropertyAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(FloatMaterialPropertyAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector2MaterialPropertyAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector2MaterialPropertyAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(Vector2MaterialPropertyAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector3MaterialPropertyAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector3MaterialPropertyAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(Vector3MaterialPropertyAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector4MaterialPropertyAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector4MaterialPropertyAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Rendering." + nameof(Vector4MaterialPropertyAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ImageColorAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_ImageColorAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.UI." + nameof(ImageColorAffordanceReceiver) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="UniformTransformScaleAffordanceReceiver"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_UniformTransformScaleAffordanceReceiver = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Receiver.Transformation." + nameof(UniformTransformScaleAffordanceReceiver) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="MaterialInstanceHelper"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_MaterialInstanceHelper = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Rendering." + nameof(MaterialInstanceHelper) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="MaterialPropertyBlockHelper"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_MaterialPropertyBlockHelper = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Rendering." + nameof(MaterialPropertyBlockHelper) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="XRInteractableAffordanceStateProvider"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_XRInteractableAffordanceStateProvider = k_BaseApi + k_BaseNamespace + "AffordanceSystem.State." + nameof(XRInteractableAffordanceStateProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="XRInteractorAffordanceStateProvider"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_XRInteractorAffordanceStateProvider = k_BaseApi + k_BaseNamespace + "AffordanceSystem.State." + nameof(XRInteractorAffordanceStateProvider) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="AudioAffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_AudioAffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Audio." + nameof(AudioAffordanceThemeDatum) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ColorAffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_ColorAffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Primitives." + nameof(ColorAffordanceThemeDatum) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="FloatAffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_FloatAffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Primitives." + nameof(FloatAffordanceThemeDatum) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector2AffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_Vector2AffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Primitives." + nameof(Vector2AffordanceThemeDatum) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector3AffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")] 
        public const string k_Vector3AffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Primitives." + nameof(Vector3AffordanceThemeDatum) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="Vector4AffordanceThemeDatum"/>.
        /// </summary>
        [Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
        public const string k_Vector4AffordanceThemeDatum = k_BaseApi + k_BaseNamespace + "AffordanceSystem.Theme.Primitives." + nameof(Vector4AffordanceThemeDatum) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="ARAnnotationInteractable"/>.
        /// </summary>
        [Obsolete("ARAnnotationInteractable is marked for deprecation and will be removed in a future version.")]
        public const string k_ARAnnotationInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARAnnotationInteractable) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARGestureInteractor"/>.
        /// </summary>
        [Obsolete("ARGestureInteractor is marked for deprecation and will be removed in a future version.")]
        public const string k_ARGestureInteractor = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARGestureInteractor) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARPlacementInteractable"/>.
        /// </summary>
        [Obsolete("ARPlacementInteractable is marked for deprecation and will be removed in a future version.")]
        public const string k_ARPlacementInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARPlacementInteractable) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARRotationInteractable"/>.
        /// </summary>
        [Obsolete("ARRotationInteractable is marked for deprecation and will be removed in a future version.")]
        public const string k_ARRotationInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARRotationInteractable) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARScaleInteractable"/>.
        /// </summary>
        [Obsolete("ARScaleInteractable is marked for deprecation and will be removed in a future version.")]
        public const string k_ARScaleInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARScaleInteractable) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARSelectionInteractable"/>.
        /// </summary>
        [Obsolete("ARSelectionInteractable is marked for deprecation and will be removed in a future version.")]
        public const string k_ARSelectionInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARSelectionInteractable) + ".html";

        /// <summary>
        /// Scripting API URL for <see cref="ARTranslationInteractable"/>.
        /// </summary>
        [Obsolete("ARTranslationInteractable is marked for deprecation and will be removed in a future version.")]        
        public const string k_ARTranslationInteractable = k_BaseApi + k_BaseNamespace + "AR." + nameof(ARTranslationInteractable) + ".html";
        
        /// <summary>
        /// Scripting API URL for <see cref="CharacterControllerDriver"/>.
        /// </summary>
        [Obsolete("k_CharacterControllerDriver has been deprecated in version 3.0.0.")]
        public const string k_CharacterControllerDriver = k_BaseApi + k_BaseNamespace + nameof(CharacterControllerDriver) + ".html";
    }
}

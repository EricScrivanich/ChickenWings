// =======================================================
// Text Animator for Unity - Copyright (c) 2018-Today, Febucci SRL, febucci.com
// - LICENSE: https://www.textanimatorforgames.com/legal/eula
// - DOCUMENTATION: https://docs.febucci.com/text-animator-unity/
// - WEBSITE: https://www.textanimatorforgames.com/
// =======================================================

using Febucci.TextAnimatorCore.BuiltIn;
using UnityEngine;

namespace Febucci.TextAnimatorForUnity.Effects
{
    [System.Serializable]
    class RotationData
    {
        public float loopDegrees = 45;
        public float oscillationDegrees = 45;
    }

    [UnityEngine.Scripting.Preserve]
    [CreateAssetMenu(menuName = ScriptablePaths.EFFECT_STATES_DIRECT+"Continuous Rotation", fileName = "Continuous Rotation Effect")]
    [EffectInfo("rot", EffectCategory.Behaviors)]
    sealed class RotationEffectScriptable : ManagedEffectScriptable<RotationEffectState, RotationData>
    {
        protected override RotationEffectState CreateState(RotationData parameters)
            => new RotationEffectState(parameters.loopDegrees, parameters.oscillationDegrees);
    }

}
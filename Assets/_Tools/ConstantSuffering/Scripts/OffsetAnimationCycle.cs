using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConstantSuffering.Tools {
    /// <summary>
    /// To make this script work, you must have a varaiable
    /// named "CycleOffset", and it has to be connected as
    /// the cycle offset
    /// </summary>

    public class AnimOffsetAnimationCycle : MonoBehaviour
    {
        // Public variables
        [Range(0, 1)]
        public float CycleOffset;
        public bool UseRandomValueInstead;

        // Constants
        private const string AnimCycleVariableName = "CycleOffset";

        // Start is called before the first frame update
        private void Start()
        {
            if (!TryGetComponent(out Animator anim))
            {
                // Throws an error if you don't have an animator component
                Debug.LogError("Couldn't find an Animator component to offset the animation.");
            }
            else
            {
                if (UseRandomValueInstead)
                {
                    // Offset anim by random
                    anim.SetFloat(AnimCycleVariableName, Random.Range(0.0f, 1.0f));
                } else
                {
                    // Offset anim by the cycle offset
                    anim.SetFloat(AnimCycleVariableName, CycleOffset);
                }
            }
            
        }
    }
}

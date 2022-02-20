using UnityEngine;

namespace ConstantSuffering.Tools
{
    /// <summary>
    /// Notes/comments attached to GameObjects.
    /// https://www.codeproject.com/Tips/1208852/How-to-Add-Comments-Notes-to-a-GameObject-in-Unity
    /// </summary>
    public class NotesForWhenUnityTriesToBackstabYouInYourSleepAgain : MonoBehaviour
    {
        [TextArea]
        public string Notes = "I hate Unity";
    }
}

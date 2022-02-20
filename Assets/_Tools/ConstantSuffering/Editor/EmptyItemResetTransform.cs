using UnityEngine;
using UnityEditor;

namespace ConstantSuffering.Tools 
{
    public class EmptyItemResetTransform : MonoBehaviour
    {
        // 16 characters long
        private const string BORDER = "----------------";

        // Creates empty object and resets its transform (Ctrl + Alt + N)
        [MenuItem("GameObject/Create Empty Object with Reset Transform %&n")]
        static void CreateEmptyObj() 
        {
            // Create a custom game object
            GameObject go = new GameObject("0,0,0 GameObject");
        }

        // Creates empty object and resets its transform (Ctrl + Shift + Alt + N)
        [MenuItem("GameObject/Create Empty Object Border %#&n")]
        static void CreateBorderObj() 
        {
            // Create a custom game object
            GameObject go = new GameObject(BORDER);
        }
    }
}

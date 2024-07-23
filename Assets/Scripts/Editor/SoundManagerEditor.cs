using UnityEngine;
using UnityEditor;

namespace Guizzan
{
    [CustomEditor(typeof(SoundManager))]
    public class SoundManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SoundManager Script = (SoundManager)target;

            if(GUILayout.Button("Load names"))
            {
                for(int i = 0; i < Script.SoundList.Count; ++i)
                {
                    Script.SoundList[i].Name = Script.SoundList[i].Sound.name;
                }
            }      
        }
    }
}
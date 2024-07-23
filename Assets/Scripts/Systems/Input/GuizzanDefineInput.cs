using UnityEngine;
using UnityEngine.UI;
namespace Guizzan
{
    namespace Input
    {
        namespace GIM
        {
            [System.Serializable]
            public struct UICanvas
            {
                public GameObject Canvas;
                public Selectable FirstSelection;
                public bool UIMode;
                public void SetActive(bool value)
                {
                    if (Canvas != null) Canvas.SetActive(value);
                    if (value)
                    {
                        GuizzanInputManager.UIMode = UIMode;
                        if (FirstSelection != null) FirstSelection.Select();
                    }
                }
            }
            public enum InputValue
            {
                Trigger,
                Toggle,
                Down,
                Up
            }
            public enum InputModes
            {
                MenuUI,
                PauseMenuUI,
                Player,
                DeathMenuUI,
                None
            }
            public enum Controller
            {
                Keyboard,
                None
            }

            namespace Player
            {
                public enum PlayerInputs
                {
                    Run,
                    Jump,
                    Crouch,
                    CamMode
                }
            }
        }
    }
}

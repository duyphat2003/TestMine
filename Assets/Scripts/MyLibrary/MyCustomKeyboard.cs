using UnityEngine;

namespace MyLibrary
{
    public static class MyCustomKeyboard
    {
        public static bool KEY_W => Input.GetKey(KeyCode.W);
        public static bool KEY_S => Input.GetKey(KeyCode.S);
        public static bool KEY_D => Input.GetKey(KeyCode.D);
        public static bool KEY_A => Input.GetKey(KeyCode.A);
        public static bool KEY_Q => Input.GetKey(KeyCode.Q);
        public static bool KEY_E => Input.GetKey(KeyCode.E);
        public static bool KEY_LSHIFT => Input.GetKey(KeyCode.LeftShift);
        public static bool MOUSE_L => Input.GetKey(KeyCode.Mouse0);
        public static bool MOUSE_R => Input.GetKeyDown(KeyCode.Mouse1);
        public static bool MOUSE_M => Input.GetKeyDown(KeyCode.Mouse2);
        public static bool KEY_R => Input.GetKeyDown(KeyCode.R);
        public static bool KEY_ESC => Input.GetKeyDown(KeyCode.Escape);
    }
}

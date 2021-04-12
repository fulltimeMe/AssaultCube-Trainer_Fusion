using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System;

namespace MYClasses
{
    class STK_Converter
    {
        public Keys[] keys = {Keys.Control, Keys.Alt, Keys.CapsLock, Keys.ShiftKey, Keys.Back,
            Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Z, Keys.U, Keys.I, Keys.O, Keys.P,
            Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L, Keys.Multiply,
            Keys.Divide, Keys.Shift, Keys.Add, Keys.Y, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N,
            Keys.M, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
            Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, Keys.ControlKey,
            Keys.MButton, Keys.LButton, Keys.RButton, Keys.CapsLock};
            
        public Keys GetKeyFromString(string keyString)
        {
            Keys resKey = Keys.CapsLock;
            
            foreach (Keys key in keys)
            {
                string keyStr = key.ToString();
                if (keyString == keyStr)
                {
                    resKey = key;
                }
            }
            return resKey;
        }
    }
}

using System.Runtime.InteropServices;

namespace KMS
{
    public class KMSimulator
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, int dx, int dy, uint cButtons, uint dwExtraInfo);

        const int MOUSEEVENTF_MOVE = 0x0001;
        //const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //const int MOUSEEVENTF_LEFTUP = 0x0004;
        //const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //const int MOUSEEVENTF_ABSOLUTE = 0x8000;


        public KMSimulator()
        {
        }

        public void MoveDelta(int dx, int dy)
        {
           mouse_event(MOUSEEVENTF_MOVE, dx, dy, 0, 0);
        }

    }
}

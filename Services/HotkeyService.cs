using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;

namespace DropShell.Services.Hotkey
{
    public class HotkeyService
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static readonly Dictionary<string, uint> VkCodes = new Dictionary<string, uint>()
        {
            // Letters
            { "a", 0x41 }, { "b", 0x42 }, { "c", 0x43 }, { "d", 0x44 },
            { "e", 0x45 }, { "f", 0x46 }, { "g", 0x47 }, { "h", 0x48 },
            { "i", 0x49 }, { "j", 0x4A }, { "k", 0x4B }, { "l", 0x4C },
            { "m", 0x4D }, { "n", 0x4E }, { "o", 0x4F }, { "p", 0x50 },
            { "q", 0x51 }, { "r", 0x52 }, { "s", 0x53 }, { "t", 0x54 },
            { "u", 0x55 }, { "v", 0x56 }, { "w", 0x57 }, { "x", 0x58 },
            { "y", 0x59 }, { "z", 0x5A },

            // Numbers
            { "0", 0x30 }, { "1", 0x31 }, { "2", 0x32 }, { "3", 0x33 },
            { "4", 0x34 }, { "5", 0x35 }, { "6", 0x36 }, { "7", 0x37 },
            { "8", 0x38 }, { "9", 0x39 },

            // Function keys
            { "F1", 0x70 }, { "F2", 0x71 }, { "F3", 0x72 }, { "F4", 0x73 },
            { "F5", 0x74 }, { "F6", 0x75 }, { "F7", 0x76 }, { "F8", 0x77 },
            { "F9", 0x78 }, { "F10", 0x79 }, { "F11", 0x7A }, { "F12", 0x7B },

            // Arrows
            { "left", 0x25 }, { "up", 0x26 }, { "right", 0x27 }, { "down", 0x28 },

            // Special keys
            { "backspace", 0x08 }, { "tab", 0x09 }, { "enter", 0x0D },
            { "shift", 0x10 }, { "ctrl", 0x11 }, { "alt", 0x12 },
            { "pause", 0x13 }, { "capslock", 0x14 }, { "esc", 0x1B },
            { "space", 0x20 }, { "pageup", 0x21 }, { "pagedown", 0x22 },
            { "end", 0x23 }, { "home", 0x24 }, { "insert", 0x2D }, { "DELETE", 0x2E },

            // Numpad keys
            { "num0", 0x60 }, { "num1", 0x61 }, { "num2", 0x62 }, { "num3", 0x63 },
            { "num4", 0x64 }, { "num5", 0x65 }, { "num6", 0x66 }, { "num7", 0x67 },
            { "num8", 0x68 }, { "num9", 0x69 },

            { "multiply", 0x6A }, { "add", 0x6B }, { "separator", 0x6C }, { "subtract", 0x6D },
            { "decimal", 0x6E }, { "divide", 0x6F },

            // Other
            { "oem_1", 0xBA }, { "oem_plus", 0xBB }, { "oem_comma", 0xBC }, { "oem_minus", 0xBD },
            { "oem_period", 0xBE }, { "oem_2", 0xBF }, { "oem_3", 0xC0 },
            { "oem_4", 0xDB }, { "oem_5", 0xDC }, { "oem_6", 0xDD }, { "oem_7", 0xDE }
        };

        public void Register(string hotKey, Window window)
        {
            var helper = new WindowInteropHelper(window);
            IntPtr hWnd = helper.Handle;

            const uint MOD_ALT = 0x0001;
            const uint MOD_CONTROL = 0x0002;
            const uint MOD_SHIFT = 0x0004;
            const uint MOD_WIN = 0x0008;

            uint modifiers = 0;
            uint vk = 0;

            List<string> keys = hotKey
                .Trim()
                .ToLower()
                .Split('+')
                .Select(k => k.Trim())
                .ToList();

            foreach (string key in keys)
            {
                if (key == "ctrl") modifiers |= MOD_CONTROL;
                else if (key == "alt") modifiers |= MOD_ALT;
                else if (key == "shift") modifiers |= MOD_SHIFT;
                else if (key == "win") modifiers |= MOD_WIN;
                else
                    vk = VkCodes[key];
            }

            int HOTKEY_ID = 9000;

            MessageBox.Show($"Modifiers {modifiers}, vk: {vk}");

            bool registered = RegisterHotKey(hWnd, HOTKEY_ID, modifiers, vk);
            if (!registered)
                MessageBox.Show("Failed to register global hotkey.");

            HwndSource source = HwndSource.FromHwnd(hWnd);
            source.AddHook(HwndHook);
        }


        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();
                if (id == 9000)
                {
                    // Hotkey pressed
                    MessageBox.Show("Hotkey pressed");
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public void Unregister(Window window)
        {
            var helper = new WindowInteropHelper(window);
            IntPtr hWnd = helper.Handle;

            int HOTKEY_ID = 9000;
            UnregisterHotKey(hWnd, HOTKEY_ID);
        }
    }
}

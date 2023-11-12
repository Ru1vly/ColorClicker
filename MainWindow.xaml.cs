using System;
using System.Windows; // OS
using System.Drawing; // Color, Bitmap, Graphics
using System.Windows.Forms; // Screen height and width
using MessageBox = System.Windows.MessageBox; // Use message box of wpf (not forms)
using System.Runtime.InteropServices; // User32.dll (and dll import)

namespace ColorBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Click()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN,0,0,0,0);
            mouse_event(MOUSEEVENTF_LEFTUP,0,0,0,0);
        }
        private void OnButtonSearchPixelClick(object sender, RoutedEventArgs i)
        {
            string inputHexColorCode = TextBoxHexColor.Text;
            SearchPixel(inputHexColorCode);
        }

        private void DoubleClickAtPosition(int posX, int posY)
        {
            SetCursorPos(posX, posY);
            Click();
            System.Threading.Thread.Sleep(200);
            Click();
        }

        private bool SearchPixel(string hexcode)
        {
            // Create an empty bitmap with the size of the screen
            //Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            // Create an empty bitmap with the size of the screen
            Bitmap bitmap = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height);
            // Create a new graphics object that can capture the screen
            Graphics graphics = Graphics.FromImage(bitmap as Image);
            // Screenshot moment -> screen content to graphics object
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            // Create a variable for hold translated user input hexcode to an color object
            Color desiredPixelColor = ColorTranslator.FromHtml(hexcode);
            // This 2 for loops runs commands for every pixel on the screen
            for(int x=0; x < SystemInformation.VirtualScreen.Width; x++)
            {
                for (int y = 0; y < SystemInformation.VirtualScreen.Height; y++)
                {
                    // Gets the current pixel color
                    Color currentPixelColor = bitmap.GetPixel(x, y);
                    // Compare the pixels hex color and the desired hex color (if they match we found a pixel)
                    if (desiredPixelColor == currentPixelColor)
                    {
                        MessageBox.Show(String.Format("Found Pixel at {0}, {1} - Now set mouse cursor", x, y));
                        // Set mouse cursor and double click
                        DoubleClickAtPosition(x, y);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

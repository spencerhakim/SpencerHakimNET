using System.Windows.Forms;
using SpencerHakim;

namespace DWMThumbnailDemoApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.dwmThumbnail1.SourceWindow = ConsoleManager.Handle;
        }
    }
}

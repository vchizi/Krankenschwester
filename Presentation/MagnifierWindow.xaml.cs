using Krankenschwester.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Krankenschwester.Presentation
{
    /// <summary>
    /// Interaction logic for MagnifierWindow.xaml
    /// </summary>
    public partial class MagnifierWindow : Window
    {
        private Magnifier _magnifier;

        public MagnifierWindow()
        {
            InitializeComponent();
            _magnifier = new Magnifier(this, MagnifierImage);

            TmpLogger.WriteLine("MAGNIFIER!");
        }

        protected override void OnClosed(EventArgs e)
        {
            _magnifier.Stop();
            base.OnClosed(e);
        }
    }
}

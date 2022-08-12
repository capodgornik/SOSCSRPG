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

namespace WPFUI.Windows
{    
    public partial class YesNoWindow : Window
    {
        public bool ClickedYes { get; private set; }

        public YesNoWindow(string title, string message)
        {
            InitializeComponent();

            Title = title;
            Message.Content = message;
        }

        private void Yes_OnClick(object sender, RoutedEventArgs e)
        {
            ClickedYes = true;
            Close();
        }

        private void No_OnClick(object sender, RoutedEventArgs e)
        {
            ClickedYes = false;
            Close();
        }
    }
}

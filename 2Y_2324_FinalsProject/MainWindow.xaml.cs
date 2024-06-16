using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;

namespace _2Y_2324_FinalsProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FilterInfoCollection fic = null;
        VideoCaptureDevice vcd = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            ////txtName.Style = (Style)FindResource("TxtBoxStyle");
            //txtName.Style = null;
            imgPatient.Source = new BitmapImage(new Uri("/Images/BlankImage.png", UriKind.Relative));

        }

        private void btnTakeImg_Click(object sender, RoutedEventArgs e)
        {
            CameraWin cam = new CameraWin();
            cam.Show();
        }

        private void btnUploadPic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Browse Photos...";
            ofd.DefaultExt = "png";
            ofd.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                "All files (*.*)|*.*";

            ofd.ShowDialog();

            //FOR SAVING FILE LOC TO DATABASE ATA
            if (ofd.FileName.Length > 0)
            {
                //txtPath.Text = ofd.FileName; eto
                imgPatient.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
            }
        }
    }
}

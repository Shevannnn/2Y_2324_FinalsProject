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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {
            if (pbPass.PasswordChar == '*')
            {
                pbPass.Visibility = Visibility.Hidden;
                txtPass.Visibility = Visibility.Visible;
                txtPass.Text = pbPass.Password;
                pbPass.PasswordChar = '\0';

                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/show.png")),
                    Stretch = Stretch.UniformToFill
                };
                btnToggle.Background = imageBrush;
            }
            else
            {
                pbPass.Visibility = Visibility.Visible;
                txtPass.Visibility = Visibility.Hidden;
                pbPass.Password = txtPass.Text;
                pbPass.PasswordChar = '*';

                var imageBrush = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/unshow.png")),
                    Stretch = Stretch.UniformToFill
                };
                btnToggle.Background = imageBrush;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            btnEdit.Visibility = Visibility.Collapsed;
            btnTakeImg.Visibility = Visibility.Visible;
            btnUploadPic.Visibility = Visibility.Visible;
            btnCancelEdit.Visibility = Visibility.Visible;
            btnSaveChanges.Visibility = Visibility.Visible;

            txtName.Style = null;
            cbSex.Style = null;
            txtAge.Style = null;
            txtDob.Style = null;
            cbBloodType.Style = null;
            txtECName.Style = null;
            txtECNum.Style = null;
            cbPStatus.Style = null;
        }

        private void btnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            btnCancelEdit.Visibility = Visibility.Collapsed;
            btnTakeImg.Visibility = Visibility.Collapsed;
            btnUploadPic.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Visible;
            btnSaveChanges.Visibility = Visibility.Collapsed;

            txtName.Style = (Style)FindResource("TxtBoxStyle2");
            cbSex.Style = (Style)FindResource("cmbStyle");
            txtAge.Style = (Style)FindResource("TxtBoxStyle2");
            txtDob.Style = (Style)FindResource("TxtBoxStyle2");
            cbBloodType.Style = (Style)FindResource("cmbStyle");
            txtECName.Style = (Style)FindResource("TxtBoxStyle2");
            txtECNum.Style = (Style)FindResource("TxtBoxStyle2");
            cbPStatus.Style = (Style)FindResource("cmbStyle");
        }
    }
}

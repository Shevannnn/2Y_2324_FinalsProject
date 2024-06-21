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
        DataClasses1DataContext _dbConn = null;
        int access = 0;
        string currPID = null;

        public MainWindow()
        {
            InitializeComponent();
            _dbConn = new DataClasses1DataContext(Properties.Settings.Default.FinalsConnectionString);
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //For Logging In
            if (txtName.Text.Length > 0 && pbPass.Password.Length > 0)
            {
                IQueryable<Staff> selectResults = from s in _dbConn.Staffs
                                                  where s.Staff_Name == txtUser.Text
                                                  select s;
                if (selectResults.Count() == 1)
                {
                    foreach (Staff s in selectResults)
                    {
                        if (s.Staff_Password == pbPass.Password || s.Staff_Password == txtPass.Text)
                        {
                            MessageBox.Show($"Login complete... {s.Staff_Name}");

                            if (s.StaffRole_Id == "SR01")
                            {
                                pnlMain.Visibility = Visibility.Visible;
                                pnlLogin.Visibility = Visibility.Collapsed;
                                pnlHeader.Visibility = Visibility.Collapsed;

                                access = 1;
                            }
                            else if (s.StaffRole_Id == "SR02" || s.StaffStatus_Id == "SR03")
                            {
                                pnlMain.Visibility = Visibility.Visible;
                                pnlLogin.Visibility = Visibility.Collapsed;
                                pnlHeader.Visibility = Visibility.Collapsed;

                                access = 2;
                            }
                        }
                        else
                            MessageBox.Show("Incorrect Password...");
                    }
                }
            }
            else
                MessageBox.Show("Please Input Name and Password...");
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
            ofd.Filter = "Images (.JPG,*.PNG)|*.JPG;*.PNG|" +
                "All files (*.*)|*.*";

            ofd.ShowDialog();

            //FOR SAVING FILE LOC TO DATABASE ATA
            if (ofd.FileName.Length > 0)
            {
                //txtPath.Text = ofd.FileName; eto
                imgPatient.Source = new BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
            }
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

        private void btnPatient_Click(object sender, RoutedEventArgs e)
        {
            pnlMain.Visibility = Visibility.Collapsed;
            pnlPatient.Visibility = Visibility.Visible;
            GetPatients();
        }

        private void GetPatients()
        {
            lvPatients.Items.Clear();
            IQueryable<Patient> selectResults = from s in _dbConn.Patients
                                            select s;
            if (selectResults.Count() >= 1)
            {
                foreach (Patient s in selectResults)
                {
                    lvPatients.Items.Add(new { Column1 = s.Patient_Id, Column2 = s.Patient_Name, Column3 = GetPatientStatus(s.PatientStatus_Id), Column4 = GetStaffName(s.Staff_Id)});
                }
            }

        }

        private string GetPatientStatus(string id)
        {
            IQueryable<PatientStatus> selectResults = selectResults = from s in _dbConn.PatientStatus
                                                              where s.PatientStatus_Id == id
                                                              select s;
            PatientStatus pet = selectResults.FirstOrDefault();
            if (pet != null)
            {
                return pet.PatientStatus_Desc;
            }
            return null;
        }

        private string GetStaffName(string id)
        {
            IQueryable<Staff> selectResults = selectResults = from s in _dbConn.Staffs
                                                            where s.Staff_Id == id
                                                            select s;
            Staff pet = selectResults.FirstOrDefault();
            if (pet != null)
            {
                return pet.Staff_Name;
            }
            return null;
        }

        private string GetPatientName(string id)
        {
            IQueryable<Patient> selectResults = selectResults = from s in _dbConn.Patients
                                                                      where s.Patient_Id == id
                                                                      select s;
            Patient pet = selectResults.FirstOrDefault();
            if (pet != null)
            {
                return pet.Patient_Name;
            }
            return null;
        }

        private void lvPatients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lvPatients.SelectedItem != null)
            {
                pnlPatient.Visibility = Visibility.Collapsed;
                pnlPatientInfo.Visibility = Visibility.Visible;
                pnlHeader.Visibility = Visibility.Visible;

                Header.Text = "Patient Information";
                dynamic selectedItem = lvPatients.SelectedItem;
                currPID = selectedItem.Column1;

                IQueryable<Patient> selectResults = from s in _dbConn.Patients
                                                   where s.Patient_Id == currPID
                                                   select s;

                foreach (Patient p in selectResults)
                {
                    txtName.Text = p.Patient_Name;
                    txtAge.Text = p.Patient_Age.ToString();
                    txtDob.Text = p.Patient_Birth.ToString("yyyy-MM-dd");
                    txtHeight.Text = p.Patient_Height.ToString();
                    txtWeight.Text = p.Patient_Weight.ToString();
                    txtECName.Text = p.Patient_EmergencyContactName;
                    txtECNum.Text = p.Patient_EmergencyContactNum.ToString();

                    switch (p.Patient_Sex)
                    {
                        case "Male":
                            cbSex.SelectedIndex = 0; 
                            break;
                        case "Female":
                            cbSex.SelectedIndex = 1;
                            break;
                    }

                    switch (p.BloodType_Id)
                    {
                        case "BT01":
                            cbBloodType.SelectedIndex = 0; 
                            break;
                        case "BT02":
                            cbBloodType.SelectedIndex = 1;
                            break;
                        case "BT03":
                            cbBloodType.SelectedIndex = 2;
                            break;
                        case "BT04":
                            cbBloodType.SelectedIndex = 3;
                            break;
                        case "BT05":
                            cbBloodType.SelectedIndex = 4;
                            break;
                        case "BT06":
                            cbBloodType.SelectedIndex = 5;
                            break;
                        case "BT07":
                            cbBloodType.SelectedIndex = 6;
                            break;
                    }

                    switch (p.PatientStatus_Id)
                    {
                        case "DS01":
                            cbPStatus.SelectedIndex = 0;
                            break;
                        case "DS02":
                            cbPStatus.SelectedIndex = 1;
                            break;
                        case "DS03":
                            cbPStatus.SelectedIndex = 2;
                            break;
                        case "DS04":
                            cbPStatus.SelectedIndex = 3;
                            break;
                        case "DS05":
                            cbPStatus.SelectedIndex = 4;
                            break;
                    }

                }
            }
        }

        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCheckUp_Click(object sender, RoutedEventArgs e)
        {

                pnlPatientInfo.Visibility = Visibility.Collapsed;
                pnlHealthInfo.Visibility = Visibility.Visible;
                pnlHeader.Visibility = Visibility.Visible;

                Header.Text = "Health Information";

                IQueryable<HealthInfo> selectResults = from s in _dbConn.HealthInfos
                                                    where s.Patient_Id == currPID
                                                    select s;

                foreach (HealthInfo p in selectResults)
                {
                    txtName2.Text = GetPatientName(currPID);
                    txtMed.Text = p.HealthInfo_Medications;
                    txtAlrgy.Text = p.HealthInfo_Allergies;
                    txtSurg.Text = p.HealthInfo_Surgeries;
                    txtFamHist.Text = p.HealthInfo_FamilyHistory;
                }
        }
    }
}

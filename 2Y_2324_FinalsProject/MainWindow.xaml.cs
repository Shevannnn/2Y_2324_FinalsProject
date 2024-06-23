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
        DataClasses1DataContext _dbConn = null;
        int access = 1; //CHANGE TO 0 I JUST NEED IT FOR TESTING
        string loggedIn = null;
        string currPID = "S01";
        string currSID = null;
        string currVID = null;
        string picPath = @"C:\Users\Evan\source\repos\2Y_2324_FinalsProject\2Y_2324_FinalsProject\Images\Pictures\";
        string fileName = "";

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
                                                  where s.Staff_Id == txtUser.Text
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

                                access = 1;
                                loggedIn = s.Staff_Id;
                            }
                            else if (s.StaffRole_Id == "SR02" || s.StaffStatus_Id == "SR03")
                            {
                                pnlMain.Visibility = Visibility.Visible;
                                pnlLogin.Visibility = Visibility.Collapsed;

                                access = 2;
                                loggedIn = s.Staff_Id;
                            }

                            txtLogin.Text = s.Staff_Name;
                        }
                        else
                            MessageBox.Show("Incorrect Password...");
                    }
                }
            }
            else
                MessageBox.Show("Please Input Name and Password...");
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

            if (ofd.FileName.Length > 0)
            {
                fileName = System.IO.Path.GetFileName(ofd.FileName); // Fully qualify Path
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

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            pnlLogin.Visibility = Visibility.Visible;
            pnlMain.Visibility = Visibility.Collapsed;
            txtUser.Text = string.Empty;
            txtPass.Text = string.Empty;
            access = 0;
            pbPass.Clear();
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
            txtHeight.Style = null;
            txtWeight.Style = null;
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

        private void btnBack2Main_Click(object sender, RoutedEventArgs e)
        {
            pnlPatient.Visibility = Visibility.Collapsed;
            pnlStaff.Visibility = Visibility.Collapsed;
            pnlMain.Visibility = Visibility.Visible;
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
                btnBack2PList.Visibility = Visibility.Visible;
                txtHeader.Text = "Patient Information";

                dynamic selectedItem = lvPatients.SelectedItem;
                currPID = selectedItem.Column1;

                IQueryable<Patient> selectResults = from s in _dbConn.Patients
                                                   where s.Patient_Id == currPID
                                                   select s;

                foreach (Patient p in selectResults)
                {
                    imgPatient.Source = new BitmapImage(new Uri(picPath + p.Patient_Image));

                    txtName.Text = p.Patient_Name;
                    txtAge.Text = p.Patient_Age.ToString();
                    txtHeight.Text = p.Patient_Height.ToString();
                    txtWeight.Text = p.Patient_Weight.ToString();
                    txtECName.Text = p.Patient_EmergencyContactName;
                    txtECNum.Text = p.Patient_EmergencyContactNum.ToString();

                    if (p.Patient_Birth.HasValue)
                    {
                        txtDob.Text = p.Patient_Birth.Value.ToString("MM-dd-yyyy");
                    }
                    else
                    {
                        txtDob.Text = "N/A";
                    }

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

        private void btnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            pnlPatient.Visibility = Visibility.Collapsed;
            pnlPatientInfo.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
            btnBack2PList.Visibility = Visibility.Visible;
            txtHeader.Text = "Patient Information";
            imgPatient.Source = new BitmapImage(new Uri(picPath + "Default.png"));
            ClearTBCB();
        }

        private string GetPID()
        {
            var highestPet = _dbConn.Patients.OrderByDescending(p => p.Patient_Id).FirstOrDefault();
            if (highestPet != null)
            {
                return highestPet.Patient_Id;
            }

            else
            {
                return "P00";
            }
        }

        private string GeneratePID(string highestID)
        {
            int num = int.Parse(highestID.Substring(1));
            num++;
            return "P" + num.ToString("D2");
        }

        private DateTime? GetDateTime(TextBox txtbox)
        {
            DateTime dateValue;
            bool isDate = DateTime.TryParse(txtbox.Text, out dateValue);

            if (!isDate)
            {
                MessageBox.Show($"{txtbox.Text} is not a valid date. Please enter a valid date.");
                return null;
            }

            return dateValue;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            // Validate numeric inputs
            int height = GetNum(txtHeight);
            int weight = GetNum(txtWeight);
            int age = GetNum(txtAge);

            if (height == -1 || weight == -1 || age == -1)
            {
                // Invalid input, exit the method
                return;
            }

            Patient nPatient = new Patient();
            nPatient.Patient_Id = "P07";
            nPatient.Patient_Name = txtName.Text;
            nPatient.Patient_Height = height;
            nPatient.Patient_Weight = weight;
            nPatient.Patient_Age = age;
            nPatient.Patient_EmergencyContactName = txtECName.Text;
            nPatient.Patient_EmergencyContactNum = txtECNum.Text;
            nPatient.Staff_Id = loggedIn;

            DateTime? birthDate = GetDateTime(txtDob);
            if (birthDate != null)
            {
                nPatient.Patient_Birth = birthDate.Value;
            }
            else
            {
                MessageBox.Show("wrong");
                return;
            }

            switch (cbSex.SelectedIndex)
            {
                case 0:
                    nPatient.Patient_Sex = "Male";
                    break;
                case 1:
                    nPatient.Patient_Sex = "Female";
                    break;
                default:
                    nPatient.Patient_Sex = "";
                    break;
            }

            switch (cbBloodType.SelectedIndex)
            {
                case 0:
                    nPatient.BloodType_Id = "BT01";
                    break;
                case 1:
                    nPatient.BloodType_Id = "BT02";
                    break;
                case 2:
                    nPatient.BloodType_Id = "BT03";
                    break;
                case 3:
                    nPatient.BloodType_Id = "BT04";
                    break;
                case 4:
                    nPatient.BloodType_Id = "BT05";
                    break;
                case 5:
                    nPatient.BloodType_Id = "BT06";
                    break;
                case 6:
                    nPatient.BloodType_Id = "BT07";
                    break;
                default:
                    nPatient.BloodType_Id = "";
                    break;
            }

            switch (cbPStatus.SelectedIndex)
            {
                case 0:
                    nPatient.PatientStatus_Id = "DS01";
                    break;
                case 1:
                    nPatient.PatientStatus_Id = "DS02";
                    break;
                case 2:
                    nPatient.PatientStatus_Id = "DS03";
                    break;
                case 3:
                    nPatient.PatientStatus_Id = "DS04";
                    break;
                default:
                    nPatient.PatientStatus_Id = "";
                    break;
            }

            Uri defaultImageUri = new Uri(picPath + "Default.png");

            // Load the default image
            BitmapImage temp = new BitmapImage(defaultImageUri);

            // Check if imgPatient.Source is the same as the default image
            if (imgPatient.Source != null && imgPatient.Source is BitmapImage currentImage)
            {
                if (currentImage.UriSource == defaultImageUri)
                {
                    nPatient.Patient_Image = "Default.png";
                }
                else
                {
                    nPatient.Patient_Image = fileName;
                }
            }

            _dbConn.Patients.InsertOnSubmit(nPatient);
            // Add if statement to handle not null fields if necessary
            _dbConn.SubmitChanges();
            MessageBox.Show("Successfully added patient!");
            ClearTBCB();

        }

        private int GetNum(TextBox txtbox)
        {
            bool isNum = false;
            string uInput = "";
            int num = 0;

            uInput = txtbox.Text;
            isNum = int.TryParse(uInput, out num);

            if (!isNum)
            {
                MessageBox.Show($"{txtbox.Text} is not a number. Please try again.");
                return -1;
            }

            return num;
        }

        private void ClearTBCB()
        {
            imgPatient.Source = new BitmapImage(new Uri(picPath + "Default.png"));

            txtName.Text = "";
            txtAge.Text = "";
            txtDob.Text = "";
            txtHeight.Text = "";
            txtWeight.Text = "";
            txtECName.Text = "";
            txtECNum.Text = "";
            cbSex.SelectedIndex = -1;
            cbBloodType.SelectedIndex = -1;
            cbPStatus.SelectedIndex = -1;

        }

        private void btnBack2PList_Click(object sender, RoutedEventArgs e)
        {
            pnlPatient.Visibility = Visibility.Visible;
            pnlPatientInfo.Visibility = Visibility.Collapsed;
            pnlHeader.Visibility = Visibility.Collapsed;
            btnBack2PList.Visibility = Visibility.Collapsed;
            txtHeader.Text = null;
            currPID = null;
        }

        private void GetVitals()
        {
            lvVitals.Items.Clear();
            IQueryable<Vital> selectResults = from s in _dbConn.Vitals
                                              where s.Patient_Id == currPID
                                              select s;

            if (selectResults.Count() >= 1)
            {
                foreach (Vital s in selectResults)
                {
                    lvVitals.Items.Add(new { Column1 = s.Vitals_Id, Column2 = s.Checkup_Date, Column3 = GetStaffName(s.Staff_Id) });
                }
            }
        }

        private void btnHealthInfo_Click(object sender, RoutedEventArgs e)
        {
            btnBack2PList.Visibility = Visibility.Collapsed;
            pnlPatientInfo.Visibility = Visibility.Collapsed;

            pnlHealthInfo.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
            btnBack2PInfo.Visibility = Visibility.Visible;
            txtHeader.Text = "Health Information";

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
                GetVitals();
            }
        }

        private void btnBack2PInfo_Click(object sender, RoutedEventArgs e)
        {
            pnlHealthInfo.Visibility = Visibility.Collapsed;
            btnBack2PInfo.Visibility = Visibility.Collapsed;
            pnlPatientInfo.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
            btnBack2PList.Visibility = Visibility.Visible;
            txtHeader.Text = "Patient Information";
        }

        private void btnEditHealth_Click(object sender, RoutedEventArgs e)
        {
            btnEditHealth.Visibility = Visibility.Collapsed;
            btnSaveHealth.Visibility = Visibility.Visible;
            btnCancelEditHealth.Visibility = Visibility.Visible;

            txtName2.Style = null;
            txtMed.Style = null;
            txtAlrgy.Style = null;
            txtSurg.Style = null;
            txtFamHist.Style = null;
        }

        private void btnCancelEditHealth_Click(object sender, RoutedEventArgs e)
        {
            SwapStyleHealth();
        }

        private void SwapStyleHealth()
        {
            btnSaveHealth.Visibility = Visibility.Collapsed;
            btnEditHealth.Visibility = Visibility.Visible;
            btnCancelEditHealth.Visibility = Visibility.Collapsed;

            txtName2.Style = (Style)FindResource("TxtBoxStyle2");
            txtMed.Style = (Style)FindResource("TxtBoxStyle2");
            txtAlrgy.Style = (Style)FindResource("TxtBoxStyle2");
            txtSurg.Style = (Style)FindResource("TxtBoxStyle2");
            txtFamHist.Style = (Style)FindResource("TxtBoxStyle2");
        }

        private void btnSaveHealth_Click(object sender, RoutedEventArgs e)
        {
            HealthInfo existing = _dbConn.HealthInfos.FirstOrDefault(p => p.Patient_Id == currPID);
            existing.HealthInfo_Medications = txtMed.Text;
            existing.HealthInfo_Allergies = txtAlrgy.Text;
            existing.HealthInfo_Surgeries= txtSurg.Text;
            existing.HealthInfo_FamilyHistory = txtFamHist.Text;
            MessageBox.Show("Succesfully saved health information");
            _dbConn.SubmitChanges();

            SwapStyleHealth();
        }

        private void btnNewVitals_Click(object sender, RoutedEventArgs e)
        {
            pnlHealthInfo.Visibility = Visibility.Collapsed;
            btnBack2PInfo.Visibility = Visibility.Collapsed;
            btnEditVital.Visibility = Visibility.Collapsed;
            pnlVitalsInfo.Visibility = Visibility.Visible;
            pnlHeader.Visibility = Visibility.Visible;
            btnBack2HInfo.Visibility = Visibility.Visible;
            btnAddVitals.Visibility = Visibility.Visible;
            txtHeader.Text = "Vitals Information";
            txtDname.Text = GetStaffName(currPID);
            SwapStyleVital();
        }

        private void SwapStyleVital()
        {
            if (txtName3.Style == null)
            {
                txtName3.Style = (Style)FindResource("TxtBoxStyle2");
                txtDate.Style = (Style)FindResource("TxtBoxStyle2");
                txtTemp.Style = (Style)FindResource("TxtBoxStyle2");
                txtPulse.Style = (Style)FindResource("TxtBoxStyle2");
                txtRespi.Style = (Style)FindResource("TxtBoxStyle2");
                txtsys.Style = (Style)FindResource("TxtBoxStyle2");
                txtdia.Style = (Style)FindResource("TxtBoxStyle2");
            }
            else
            {
                txtName3.Style = null;
                txtDate.Style = null;
                txtTemp.Style = null;
                txtPulse.Style = null;
                txtRespi.Style = null;
                txtsys.Style = null;
                txtdia.Style = null;
            }
        }

        private void btnEditVital_Click(object sender, RoutedEventArgs e)
        {
            btnEditVital.Visibility = Visibility.Collapsed;
            btnCancelEditVital.Visibility = Visibility.Visible;
            btnSaveVitals.Visibility = Visibility.Visible;
            SwapStyleVital();
        }

        private void btnCancelEditVital_Click(object sender, RoutedEventArgs e)
        {
            btnEditVital.Visibility = Visibility.Visible;
            btnCancelEditVital.Visibility = Visibility.Collapsed;
            btnSaveVitals.Visibility = Visibility.Collapsed;
            SwapStyleVital();
        }

        private void btnSaveVitals_Click(object sender, RoutedEventArgs e)
        {
            // Validate numeric inputs
            int temp = GetNum(txtTemp);
            int pulse = GetNum(txtPulse);
            int respi = GetNum(txtRespi);
            int sys = GetNum(txtsys);
            int dia = GetNum(txtdia);

            if (temp == -1 || pulse == -1 || respi == -1 || sys == -1 || dia == -1)
            {
                // Invalid input, exit the method
                return;
            }

            Vital existing = _dbConn.Vitals.FirstOrDefault(p => p.Vitals_Id == currVID);
            //existing.Checkup_Date = DateTime.Now;
            existing.Patient_Temp = temp;
            existing.Patient_PulseRate = pulse;
            existing.Patient_Respiration = respi;
            existing.Patient_Systolic = sys;
            existing.Patient_Diastolic = dia;

            MessageBox.Show("Succesfully saved vitals information");
            _dbConn.SubmitChanges();
        }

        private string GetVID()
        {
            var highestPet = _dbConn.Vitals.OrderByDescending(p => p.Vitals_Id).FirstOrDefault();
            if (highestPet != null)
            {
                return highestPet.Vitals_Id;
            }

            else
            {
                return "C00";

            }
        }

        private string GenerateVID(string highestID)
        {
            int num = int.Parse(highestID.Substring(1));
            num++;
            return "C" + num.ToString("D2");
        }

        private void btnAddVitals_Click(object sender, RoutedEventArgs e)
        {
            // Validate numeric inputs
            int temp = GetNum(txtTemp);
            int pulse = GetNum(txtPulse);
            int respi = GetNum(txtRespi);
            int sys = GetNum(txtsys);
            int dia = GetNum(txtdia);

            if (temp == -1 || pulse == -1 || respi == -1 || sys == -1 || dia == -1)
            {
                // Invalid input, exit the method
                return;
            }

            Vital nVital = new Vital();
            nVital.Vitals_Id = GenerateVID(GetVID());
            nVital.Patient_Id = currPID;
            nVital.Checkup_Date = DateTime.Now;
            nVital.Patient_Temp = temp;
            nVital.Patient_PulseRate = pulse;
            nVital.Patient_Respiration = respi;
            nVital.Patient_Systolic = sys;
            nVital.Patient_Diastolic = dia;
            nVital.Staff_Id = loggedIn;
        }

        private void lvVitals_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            currVID = null;
            if (lvVitals.SelectedItem != null)
            {
                pnlHealthInfo.Visibility = Visibility.Collapsed;
                btnBack2PInfo.Visibility = Visibility.Collapsed;
                btnEditVital.Visibility = Visibility.Visible;
                pnlVitalsInfo.Visibility = Visibility.Visible;
                pnlHeader.Visibility = Visibility.Visible;
                btnBack2HInfo.Visibility = Visibility.Visible;
                txtHeader.Text = "Vitals Information";

                dynamic selectedItem = lvVitals.SelectedItem;
                currVID = selectedItem.Column1;

                IQueryable<Vital> selectResults = from s in _dbConn.Vitals
                                                  where s.Vitals_Id == currVID
                                                  select s;

                foreach (Vital v in selectResults)
                {
                    txtDname.Text = GetStaffName(v.Staff_Id);
                    txtName3.Text = GetPatientName(v.Patient_Id);
                    txtDate.Text = v.Checkup_Date.ToString();
                    txtTemp.Text = v.Patient_Temp.ToString();
                    txtPulse.Text = v.Patient_PulseRate.ToString();
                    txtRespi.Text = v.Patient_Respiration.ToString();
                    txtsys.Text = v.Patient_Systolic.ToString();
                    txtdia.Text = v.Patient_Diastolic.ToString();
                }
            }
        }

        private void btnBack2HInfo_Click(object sender, RoutedEventArgs e)
        {
            pnlHealthInfo.Visibility = Visibility.Visible;
            btnBack2PInfo.Visibility = Visibility.Visible;
            pnlVitalsInfo.Visibility = Visibility.Collapsed;
            pnlHeader.Visibility = Visibility.Visible;
            btnBack2HInfo.Visibility = Visibility.Collapsed;
            btnAddVitals.Visibility = Visibility.Collapsed;
            txtHeader.Text = "Health Information";
            txtName3.Style = (Style)FindResource("TxtBoxStyle2");
            txtDate.Style = (Style)FindResource("TxtBoxStyle2");
            txtTemp.Style = (Style)FindResource("TxtBoxStyle2");
            txtPulse.Style = (Style)FindResource("TxtBoxStyle2");
            txtRespi.Style = (Style)FindResource("TxtBoxStyle2");
            txtsys.Style = (Style)FindResource("TxtBoxStyle2");
            txtdia.Style = (Style)FindResource("TxtBoxStyle2");
        }

        private void btnStaff_Click(object sender, RoutedEventArgs e)
        {
            if (access == 2)
            {
                MessageBox.Show("Invalid Access...");
            }
            else
            {
                pnlMain.Visibility = Visibility.Collapsed;
                pnlStaff.Visibility = Visibility.Visible;
                GetStaffs();
            }
        }

        private void GetStaffs()
        {
            lvStaff.Items.Clear();
            IQueryable<Staff> selectResults = from s in _dbConn.Staffs
                                                select s;
            if (selectResults.Count() >= 1)
            {
                foreach (Staff s in selectResults)
                {
                    lvStaff.Items.Add(new { Column1 = s.Staff_Id, Column2 = s.Staff_Name, Column3 = GetStaffRole(s.StaffRole_Id), Column4 = GetStaffStatus(s.StaffStatus_Id) });
                }
            }
        }

        private string GetStaffRole(string id)
        {
            IQueryable<StaffRole> selectResults = selectResults = from s in _dbConn.StaffRoles
                                                              where s.StaffRole_Id == id
                                                              select s;
            StaffRole pet = selectResults.FirstOrDefault();
            if (pet != null)
            {
                return pet.StaffRole_Desc;
            }
            return null;
        }

        private string GetStaffStatus(string id)
        {
            IQueryable<StaffStatus> selectResults = selectResults = from s in _dbConn.StaffStatus
                                                              where s.StaffStatus_Id == id
                                                              select s;
            StaffStatus pet = selectResults.FirstOrDefault();
            if (pet != null)
            {
                return pet.StaffStatus_Desc;
            }
            return null;
        }

        private void lvStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvStaff.SelectedItem != null)
            {
                pnlStaff.Visibility = Visibility.Collapsed;
                pnlStaffInfo.Visibility = Visibility.Visible;
                pnlHeader.Visibility = Visibility.Visible;
                btnBack2SList.Visibility = Visibility.Visible;
                txtHeader.Text = "Staff Information";

                dynamic selectedItem = lvStaff.SelectedItem;
                currSID = selectedItem.Column1;

                IQueryable<Staff> selectResults = from s in _dbConn.Staffs
                                                    where s.Staff_Id == currSID
                                                    select s;

                foreach (Staff s in selectResults)
                {
                    imgStaff.Source = new BitmapImage(new Uri(picPath + s.Staff_Image));

                    txtNameS.Text = s.Staff_Name;
                    txtPassS.Text = s.Staff_Password;

                    switch (s.StaffRole_Id)
                    {
                        case "SR01":
                            cbRole.SelectedIndex = 0;
                            break;
                        case "SR02":
                            cbRole.SelectedIndex = 1;
                            break;
                        case "SR03":
                            cbRole.SelectedIndex = 2;
                            break;
                    }

                    switch (s.StaffStatus_Id)
                    {
                        case "SS01":
                            cbStatus.SelectedIndex = 0;
                            break;
                        case "SS02":
                            cbStatus.SelectedIndex = 1;
                            break;
                        case "SS03":
                            cbStatus.SelectedIndex = 2;
                            break;
                    }
                }
            }
        }

        private void btnBack2SList_Click(object sender, RoutedEventArgs e)
        {
            pnlStaffInfo.Visibility = Visibility.Collapsed;
            pnlHeader.Visibility = Visibility.Collapsed;
            txtHeader.Text = null;
            pnlStaff.Visibility = Visibility.Visible;
        }

        private void btnAddS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            btnEdit1.Visibility = Visibility.Collapsed;
            btnTakeImg1.Visibility = Visibility.Visible;
            btnUploadPic1.Visibility = Visibility.Visible;
            btnCancelEdit1.Visibility = Visibility.Visible;
            btnSaveChanges1.Visibility = Visibility.Visible;

            txtNameS.Style = null;
            txtPassS.Style = null;
            cbRole.Style = null;
            cbStatus.Style = null;
        }

        private void btnCancelEdit1_Click(object sender, RoutedEventArgs e)
        {
            btnCancelEdit1.Visibility = Visibility.Collapsed;
            btnTakeImg1.Visibility = Visibility.Collapsed;
            btnUploadPic1.Visibility = Visibility.Collapsed;
            btnEdit1.Visibility = Visibility.Visible;
            btnSaveChanges1.Visibility = Visibility.Collapsed;

            txtNameS.Style = (Style)FindResource("TxtBoxStyle2");
            txtPassS.Style = (Style)FindResource("TxtBoxStyle2");
            cbRole.Style = (Style)FindResource("cmbStyle");
            cbStatus.Style = (Style)FindResource("cmbStyle");
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToUpper();
            SearchPatients(search);
        }

        private void SearchPatients(string searchText)
        {
            lvPatients.Items.Clear();
            IQueryable<Patient> searchResults = from s in _dbConn.Patients
                                                where s.Patient_Name.ToUpper().Contains(searchText)
                                                select s;
            if (searchResults.Count() >= 1)
            {
                foreach (Patient s in searchResults)
                {
                    lvPatients.Items.Add(new { Column1 = s.Patient_Id, Column2 = s.Patient_Name, Column3 = GetPatientStatus(s.PatientStatus_Id), Column4 = GetStaffName(s.Staff_Id) });
                }
            }
        }

        private void txtSearch1_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = (string)txtSearch1.Text.ToUpper();
            SearchStaff(search);
        }

        private void SearchStaff(string searchText)
        {
            lvStaff.Items.Clear();
            IQueryable<Staff> searchResults = from s in _dbConn.Staffs
                                                where s.Staff_Name.ToUpper().Contains(searchText)
                                                select s;
            if (searchResults.Count() >= 1)
            {
                foreach (Staff s in searchResults)
                {
                    lvStaff.Items.Add(new { Column1 = s.Staff_Id, Column2 = s.Staff_Name, Column3 = GetStaffRole(s.StaffRole_Id), Column4 = GetStaffStatus(s.StaffStatus_Id) });
                }
            }
        }
    }
}

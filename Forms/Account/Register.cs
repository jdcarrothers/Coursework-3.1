using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Coursework_0._0.Classes;
using Coursework_0._0.Forms.MainMenu;
using System.Runtime.InteropServices;

namespace Coursework_0._0
{
    public partial class Register : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
       (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // height of ellipse
           int nHeightEllipse // width of ellipse
       );
        Player thisPlayer = new Player();
        List<Player> players = new List<Player>();
        bool avatarSelected = false;
        private PictureBox selectedPictureBox = null;
        private Image playerSelelectedAvatar;
        public Register()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            readFileToList();
            btnRegister.Enabled = false;
            txtPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;
        }
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            UpdateUsernameValidation();
        }
        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordValidation();
        }
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordValidation();
        }
        bool userNameExists = false;
        private bool usernameExistsInList()
        {
            foreach (Player player in players)
            {
                if (player.Username == txtUsername.Text)
                    return true;
            }
            return false;
        
        }
        private void btnRegister_Click(object sender, EventArgs e)
        {
            userNameExists = usernameExistsInList();
            if (userNameExists)
            {
                MessageBox.Show("Username already exists");
                return;
            }
            else
            {
                thisPlayer.Username = txtUsername.Text;
                thisPlayer.Password = txtPassword.Text;
                players.Add(thisPlayer);
                Stream sw;
                BinaryFormatter bf = new BinaryFormatter();
                try
                {
                    sw = File.OpenWrite("Players.bin");
                    bf.Serialize(sw, players);
                    sw.Close();
                    MessageBox.Show("Logging in");
                    this.Hide();
                    Form newMainMenu = new MainMenuParentForm(thisPlayer);
                    newMainMenu.Show();
                    thisPlayer = new Player();
                }
                catch (SerializationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                UpdateListWithAllAskedBeforeFalse();

            }
        }
        List<ManageQuestions> questionsList = new List<ManageQuestions>();
        private void readFileToListQuestions()
        {
            Stream sr;
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                sr = File.OpenRead("Questions.bin");
                questionsList = (List<ManageQuestions>)bf.Deserialize(sr);
                sr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SaveQuestions()
        {
            try
            {
                using (Stream sr = File.OpenWrite("Questions.bin"))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(sr, questionsList);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void UpdateListWithAllAskedBeforeFalse()
        {
            readFileToListQuestions();
            foreach (ManageQuestions question in questionsList)
            {
                question.AskedBefore = false;
            }
            SaveQuestions(); // saves all questions as false;
        }
        private void checkBoxShowPas_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar == true)
            {
                txtPassword.UseSystemPasswordChar = false;
                txtConfirmPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                txtConfirmPassword.UseSystemPasswordChar = true;
            }
        }
        private void UpdateUsernameValidation()
        {
            SetValidationLabel(lblUsernameLengthCheck, txtUsername.Text.Length >= 5);
            Check();
        }
        private bool IsPasswordValid()
        {
            return txtPassword.Text.Length >= 7 && ContainsNum();
        }
        private bool ArePasswordsMatching()
        {
            return txtPassword.Text == txtConfirmPassword.Text;
        }
        private void SetValidationLabel(Label label, bool isValid)
        {
            label.ForeColor = isValid ? Color.Lime : Color.Maroon;
        }
        private void SetMatchingPasswordsLabel(bool areMatching)
        {
            lblMatchingPasswords.ForeColor = areMatching ? Color.Lime : Color.Maroon;
            //lblMatchingPasswords.Text = areMatching ? "Matching Passwords" : "Passwords do not match or they match but the criteria haven't been met";
        }
        private bool ContainsNum()
        {
            string password = txtPassword.Text;
            foreach (char character in password)
            {
                if (char.IsDigit(character))
                    return true;
            }
            return false;
        }
        private void Check()
        {
            bool usernameValid = txtUsername.Text.Length >= 5;
            bool passwordValid = IsPasswordValid();
            bool passwordsMatching = ArePasswordsMatching();
            bool avatarselected = avatarSelected;
            btnRegister.Enabled = usernameValid && passwordValid && passwordsMatching && avatarselected;
        }
        private void UpdatePasswordValidation()
        {
            bool passwordValid = IsPasswordValid();
            bool passwordsMatching = ArePasswordsMatching();
            SetValidationLabel(lblPasswordLengthCheck, txtPassword.Text.Length >= 7);
            SetValidationLabel(lblPasswordNumberCheck, ContainsNum());
            SetMatchingPasswordsLabel(passwordsMatching && passwordValid);
            Check();
        }

        private void readFileToList()
        {
            Stream sr;
            BinaryFormatter bf = new BinaryFormatter();
            try
            {
                sr = File.OpenRead("Players.bin");
                players = (List<Player>)bf.Deserialize(sr);

                sr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnUploadOwnImage_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog();
                if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string path = file.FileName;
                    if (file.ValidateNames == true) // ensures the image selected is of a valid file type
                    {
                        playerSelelectedAvatar = Image.FromFile(path);
                        imgAvatar.Image = playerSelelectedAvatar;
                        btnUploadOwnImage.Text = path;
                        SelectPfp(imgAvatar, e);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void SelectPfp(object sender, EventArgs e)
        {
            var clickedPfp = (PictureBox)sender;
            if (selectedPictureBox != null)
            {
                selectedPictureBox.BorderStyle = BorderStyle.None;
            }
            thisPlayer.Avatar = clickedPfp.Image;
            clickedPfp.BorderStyle = BorderStyle.Fixed3D;
            selectedPictureBox = clickedPfp;
            avatarSelected = true;
            lblAvatarSelected.ForeColor = Color.Lime;
            lblAvatarSelected.Text = "Selected";
            Check();
        }

        private void pbExit_Click(object sender, EventArgs e)
        {
            Point originalLocation = pbExit.Location;
            pbExit.Location = new Point(originalLocation.X + 2, originalLocation.Y + 2);//animates the exit button for user feedback
            System.Threading.Thread.Sleep(100);
            pbExit.Location = originalLocation;
            if (MessageBox.Show("Do you wish to exit the application?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form loginForm = new Login();
            loginForm.Show();
            thisPlayer = new Player();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish to quit?", "Quit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void lblPasswordNumberCheck_Click(object sender, EventArgs e)
        {

        }
    }
}




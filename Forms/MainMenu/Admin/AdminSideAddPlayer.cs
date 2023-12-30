using Coursework_0._0.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coursework_0._0.Forms.MainMenu.Admin
{
    public partial class AdminSideAddPlayer : Form
    {
        Player thisPlayer = new Player();
        List<Player> players = new List<Player>();
        bool avatarSelected = false;
        private PictureBox selectedPictureBox = null;
        private Image playerSelelectedAvatar;
        public AdminSideAddPlayer()
        {
            InitializeComponent();
            readFileToList();
            btnRegister.Enabled = false;
            txtPassword.UseSystemPasswordChar = true;
            txtConfirmPassword.UseSystemPasswordChar = true;
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

        private void btnAddPlayer_Click(object sender, EventArgs e)
        {
            MainMenuParentForm parentForm = (MainMenuParentForm)this.ParentForm;
            parentForm.LoadChildForm(new AdminMenuPlayerEditor());
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            UpdateUsernameValidation();
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
        private void Check()
        {
            bool usernameValid = txtUsername.Text.Length >= 5;
            bool passwordValid = IsPasswordValid();
            bool passwordsMatching = ArePasswordsMatching();
            bool avatarselected = avatarSelected;
            btnRegister.Enabled = usernameValid && passwordValid && passwordsMatching && avatarselected;
        }
        private bool ArePasswordsMatching()
        {
            return txtPassword.Text == txtConfirmPassword.Text;
        }
        private void SetValidationLabel(Label label, bool isValid)
        {
            label.ForeColor = isValid ? Color.Lime : Color.Maroon;
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
        private void UpdatePasswordValidation()
        {
            bool passwordValid = IsPasswordValid();
            bool passwordsMatching = ArePasswordsMatching();
            SetValidationLabel(lblPasswordLengthCheck, txtPassword.Text.Length >= 7);
            SetValidationLabel(lblPasswordNumberCheck, ContainsNum());
            SetMatchingPasswordsLabel(passwordsMatching && passwordValid);
            Check();
        }
        private void SetMatchingPasswordsLabel(bool areMatching)
        {
            lblMatchingPasswords.ForeColor = areMatching ? Color.Lime : Color.Maroon;
            //lblMatchingPasswords.Text = areMatching ? "Matching Passwords" : "Passwords do not match or they match but the criteria haven't been met";
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

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordValidation();
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordValidation();
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
                    MessageBox.Show("Player added");
                }
                catch (SerializationException ex)
                {
                    MessageBox.Show(ex.Message);
                }
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
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Coursework_0._0.Classes;
using Coursework_0._0.Forms;
using Coursework_0._0.Forms.MainMenu;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Coursework_0._0
{
    public partial class Login : Form
    {
        List<ManageQuestions> questionsList = new List<ManageQuestions>();
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
        public Login()
        {
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;


            List<Player> players = ReadPlayersFromFile();
            Player foundPlayer = players.Find(player => player.Username == username && player.Password == password);

            if (foundPlayer != null)
            {
                this.Hide();
                Form newMainMenu = new MainMenuParentForm(foundPlayer);
                newMainMenu.Show();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.");
            }
            UpdateListWithAllAskedBeforeFalse();
        }
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
        private List<Player> ReadPlayersFromFile()
        {
            List<Player> players = new List<Player>();
            Stream sr;
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                sr = File.OpenRead("Players.bin");
                players = (List<Player>)bf.Deserialize(sr);
                sr.Close();
            }
            catch (FileNotFoundException)
            {
                // Handle the case where the file does not exist.
                // You may want to create the file if it doesn't exist initially.
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while reading player data: " + ex.Message);
            }

            return players;
        }
        private void checkBoxShowPas_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar == true)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtUsername.Text = "admin";
            txtPassword.Text = "admin";
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form RegisterForm = new Register();
            RegisterForm.Show();
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you wish to quit?", "Quit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}

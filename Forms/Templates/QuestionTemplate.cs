using Coursework_0._0.Classes;
using Coursework_0._0.Forms.MainMenu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coursework_0._0.Forms.Templates
{
    public partial class QuestionTemplate : Form
    {
        public static Timer questionBarTimer;
        public static int easyPoint = 1;
        public static int hardPoint = 2;
        public static int easyBonus = 2;
        public static int hardBonus = 3;
        public static bool selectedGame = false;
        public QuestionTemplate()
        {
            InitializeComponent();
            timer.Start();
        }

        public void SetQuestionLabel(string question) 
        {
            lblQuestion.Text = $"{question}";
        }
        public void SetNewScore(int score)
        {
            lblScore.Text = $"Score: {score}";
        }
        public void EasySetSummaryScreen(string totalscore)
        {
            lblScore.Text= $"You scored {totalscore} ";
        }
        public void ClearSubmitAndNextPB()
        {
            nextTemp.Visible = false;
            btnSubmitTemp.Visible = false;
        }
        public void ClearProgressBar()
        {
            QuestionProgressBar.Visible = false;
        }
        public void stopProgressTimer()//method to stop bar timer in derived classes
        {
            timer.Stop();
        
        }
        public void setUpdatedHS(int HS)
        {
            lblHighScore.Text = $"Highscore: {HS}";
        
        }
        public static bool needExtraTime = false;
        private void timer_Tick(object sender, EventArgs e)
        {
            if (needExtraTime)
            {
                QuestionProgressBar.Maximum = 250;
                if (QuestionProgressBar.Value != 250)
                {
                    QuestionProgressBar.Value += 1;
                }
            }
            else
            {
                   
                if (QuestionProgressBar.Value != 150)
                {
                    QuestionProgressBar.Value += 1;
                }
            }
        }
        Player currentPlayer;
        private void QuestionTemplate_Load(object sender, EventArgs e)
        {
            if (this.ParentForm is MainMenuParentForm parentForm)
            {
                currentPlayer = parentForm.getCurrentPlayer;
                int scoreDisplayed;
                if (!selectedGame)
                    scoreDisplayed = currentPlayer.HighScore;
                else
                    scoreDisplayed = currentPlayer.HardHighScore;
                if (!selectedGame)
                    lblHighScore.Text = $"Easy Highscore: {scoreDisplayed}";
                else
                    lblHighScore.Text = $"Hard Highscore: {scoreDisplayed}";
            }
        }
    }
}

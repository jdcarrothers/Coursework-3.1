﻿using Coursework_0._0.Classes;
using Coursework_0._0.Forms.CustomBoxes;
using Coursework_0._0.Forms.MainMenu;
using Coursework_0._0.Forms.Questions;
using Coursework_0._0.Forms.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace Coursework_0._0.Forms.Question
{
    public partial class EasyRadioButtonQuestion : QuestionTemplate
    {
        List<ManageQuestions> questionsList = new List<ManageQuestions>();
        int tempscore;
        bool easySelected = false;
        public EasyRadioButtonQuestion(bool choice)
        {
            InitializeComponent();
            setQuestionType(choice);
            base.SetNewScore(tempscore);
            base.SetQuestionLabel("Which one of these statements are true?");
            btnSubmit.Enabled = false;
            btnNext.Enabled = false;
            timer1.Start();
        }
        private void setQuestionType(bool qType)
        {
            if (!qType)
            {
                selectedGame = false;
                easySelected = true;
                readFileToListQuestions(); // if easy randomise the question
                LoadRandomQuestion();
            }
            else
            {
                selectedGame = true;
                easySelected = false;   //if hard show the hard question
                LoadHardQuestion();
            }
        }
        private void LoadHardQuestion()
        {
            lblRadioButtonQuestion.Text = "How does rapid prototyping benefit workshops in product development?";
            rbCorrect.Text = "Speeds up testing and cuts costs.";
            rbIncorrect_1.Text = "Enhances design complexity.";
            rbIncorrect_2.Text = "Prioritizes marketing strategies.";
            rbIncorrect_3.Text = "Reduces collaboration needs.";
            lblRadioButtonQuestion.Location = new System.Drawing.Point(400, 133);

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
        List<ManageQuestions> unaskedQuestions = new List<ManageQuestions>();
        private void LoadRandomQuestion()
        {
            Random rnd = new Random();
            foreach (var question in questionsList)
            {
                if (!question.AskedBefore)
                {
                    unaskedQuestions.Add(question);
                }
            }
            if (unaskedQuestions.Count == questionsList.Count - questionsList.Count)
            {
                MessageBox.Show("All question variations have been exhausted, you are going to see repeats");
                UpdateListWithAllAskedBeforeFalse();
                LoadRandomQuestion();
            }
            int randomIndex = rnd.Next(0, unaskedQuestions.Count);
            ManageQuestions randomQuestion = unaskedQuestions[randomIndex];
            lblRadioButtonQuestion.Text = randomQuestion.Question;
            rbCorrect.Text = randomQuestion.CorrectAnswer;
            rbIncorrect_1.Text = randomQuestion.IncorrectAnswer1;
            rbIncorrect_2.Text = randomQuestion.IncorrectAnswer2;
            rbIncorrect_3.Text = randomQuestion.IncorrectAnswer3;
            foreach (var q in questionsList)
            {
                if (q.Equals(randomQuestion)) 
                {
                    q.AskedBefore = true;
                }
            }
            SaveQuestions();
            readFileToListQuestions();
        }
        private void UpdateListWithAllAskedBeforeFalse()
        {
            readFileToListQuestions();
            foreach (ManageQuestions question in questionsList)
            {
                question.AskedBefore = false;
            }
            SaveQuestions();
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
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            base.stopProgressTimer();
            btnSubmit.Text = "Submitted";
            btnSubmit.Enabled = false;
            btnNext.Enabled = true;
            if (rbCorrect.Checked)
            {
                pbCorrect.Visible = true;
                if (val > 50)
                {
                    if (easySelected)
                        tempscore = tempscore + easyPoint;
                    else
                        tempscore = tempscore + hardPoint;
                }
                else if (val < 75)
                {
                    if (easySelected)
                        tempscore = tempscore + easyBonus + easyPoint;
                    else
                        tempscore = tempscore + hardBonus + hardPoint;
                }
                SetNewScore(tempscore);
            }
            if (rbIncorrect_1.Checked)
                pbIncorrect1.Visible = true;
            if (rbIncorrect_2.Checked)
                pbIncorrect2.Visible = true;
            if (rbIncorrect_3.Checked)
                pbIncorrect3.Visible = true;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            EasyDragAndDrop EasyDragAndDrop = new EasyDragAndDrop(tempscore, easySelected);
            EasyDragAndDrop.Tag = "a";
            MainMenuParentForm parentForm = (MainMenuParentForm)this.ParentForm;
            parentForm.LoadChildForm(EasyDragAndDrop);
        }

        private void EnableSubmit(object sender, EventArgs e)
        {
            btnSubmit.Enabled = true;
        }
        int val;
        private void timer1_Tick(object sender, EventArgs e)
        {
            val++;
            if (val == 150)
            {
                Form ranOutOfTime = new RanOutOfTime();
                ranOutOfTime.ShowDialog();
                btnSubmit.Enabled = false;
                rbCorrect.Enabled = false;
                rbIncorrect_1.Enabled = false;
                rbIncorrect_2.Enabled = false;
                rbIncorrect_3.Enabled = false;
                btnNext.Enabled = true;
                timer1.Stop();
            }
        }
    }
}

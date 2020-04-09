// This script handles the response text and objects for the first part of the survey

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace Interactive360
{

    public class MultiChoiceQns : MonoBehaviour
    {

        public static MultiChoiceQns instance = null;

        public GameObject choice1button;
        public GameObject choice2button;
        public GameObject choice3button;
        public GameObject choice4button;
        public GameObject choice5button;
        public TextMeshProUGUI choice1text;
        public TextMeshProUGUI choice2text;
        public TextMeshProUGUI choice3text;
        public TextMeshProUGUI choice4text;
        public TextMeshProUGUI choice5text;
        private bool selected1 = false;
        private bool selected2 = false;
        private bool selected3 = false;
        private string selectedVideo;

        public int qnNo = 0;
        List<string> answers = new List<string>();
        string currentQn;
        string response;

        // If file doesn't exist, create it (TODO)
        static readonly string multiChoiceTextFile = @"D:\GitHub\soundscape_player\multiQns.txt";
        string multiChoiceAnswerFile = @"D:\GitHub\soundscape_player\multiAns.txt";

        // For switching questions, the left or right side of the trackpad has to be touched and the entire trackpad has to be pushed down
        [SerializeField] string m_trackpadHori = "TrackpadHori";
        [SerializeField] string m_trackpadDown = "SecondaryThumbstick";

        // To remove?? (Makes testing easier but might not need for final)
        private void Update()
        {
            if (Input.GetAxis(m_trackpadHori) < 0 && Input.GetButtonDown(m_trackpadDown) && MenuManager.instance.surveyState == "multi")
            {
                PrevQn();
            }
            if (Input.GetAxis(m_trackpadHori) > 0 && Input.GetButtonDown(m_trackpadDown) && MenuManager.instance.surveyState == "multi")
            {
                NextQn();
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        public void Select1()
        {
            qnNo = 0;

            //ReaperManager.instance.SendOscReaper("/track/" + (1).ToString() + "/solo/toggle");
            selected1 = true;
            selectedVideo = "Video 1";

            if (selected2 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (2).ToString() + "/solo/toggle");
                selected2 = false;
            }

            if (selected3 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (3).ToString() + "/solo/toggle");
                selected3 = false;
            }

            readQuestion();
            ShowComponents();

        }

        public void Select2()
        {
            qnNo = 0;

            ReaperManager.instance.SendOscReaper("/track/" + (2).ToString() + "/solo/toggle");
            selected2 = true;
            selectedVideo = "Video 2";

            /*
            if (selected1 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (1).ToString() + "/solo/toggle");
                selected1 = false;
            }
            */

            if (selected3 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (3).ToString() + "/solo/toggle");
                selected3 = false;
            }

            readQuestion();
            ShowComponents();

        }

        public void Select3()
        {
            qnNo = 0;

            ReaperManager.instance.SendOscReaper("/track/" + (3).ToString() + "/solo/toggle");
            selected3 = true;
            selectedVideo = "Video 3";

            /*
            if (selected1 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (1).ToString() + "/solo/toggle");
                selected1 = false;
            }
            */

            if (selected2 == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (2).ToString() + "/solo/toggle");
                selected2 = false;
            }

            readQuestion();
            ShowComponents();

        }

        // Read from text file
        public void readQuestion()
        {
            string[] lines = File.ReadAllLines(multiChoiceTextFile);
            if (qnNo < 0)
            {
                qnNo = 0;
            }
            // Checks which part of the questionnaire to set appropriate choice text
            currentQn = lines[qnNo];

             SetChoiceText(lines[qnNo]);
             MenuManager.instance.questionText.SetText(lines[qnNo] + "\n" + lines[qnNo + 1]);
            // If qnNo > string array count, cycle back to start
        }

        public void RecordResponse(string response)
        {
            response = currentQn + "," + response + "," + selectedVideo;
            answers.Insert(qnNo / 2, response);
            using (StreamWriter sw = new StreamWriter(multiChoiceAnswerFile))
            {
                foreach (String s in answers)
                    sw.WriteLine(s);
            }
            readQuestion();
            NextQn();
        }

        // Each qn has 3 lines in textfile
        public void NextQn()
        {
            qnNo += 2;
            // Bring to respective quizzes
            readQuestion();
            MenuManager.instance.UpdateMenu();
        }

        public void PrevQn()
        {
            qnNo -= 2;
            readQuestion();
            MenuManager.instance.UpdateMenu();
        }

        // Bind to each choice
        public void Choice1()
        {
            RecordResponse("1");
        }

        public void Choice2()
        {
            RecordResponse("2");
        }

        public void Choice3()
        {
            RecordResponse("3");
        }

        public void Choice4()
        {
            RecordResponse("4");
        }

        public void Choice5()
        {
            RecordResponse("5");
        }

        public void SetChoiceText(string part)
        {
            if (part.Contains("Part 1"))
            {
                choice1text.SetText("Not at all");
                choice2text.SetText("A little");
                choice3text.SetText("Moderately");
                choice4text.SetText("A lot");
                choice5text.SetText("Dominates completely");
            }
            else if (part.Contains("Part 2"))
            {
                choice1text.SetText("Strongly agree");
                choice2text.SetText("Agree");
                choice3text.SetText("Neutral");
                choice4text.SetText("Disagree");
                choice5text.SetText("Strongly disagree");
            }
            else if (part.Contains("Part 3"))
            {
                choice1text.SetText("Very good");
                choice2text.SetText("Good");
                choice3text.SetText("Neutral");
                choice4text.SetText("Bad");
                choice5text.SetText("Very bad");
            }
            else if (part.Contains("Part 4"))
            {
                choice1text.SetText("Not at all");
                choice2text.SetText("Slightly");
                choice3text.SetText("Moderately");
                choice4text.SetText("Very");
                choice5text.SetText("Perfectly"); 
            }
            else
            {
                HideComponents();
            }      
        }

        // Reset track states to false when starting for the first time
        public void ResetTrackStates()
        {
            selected1 = false;
            selected2 = false;
            selected3 = false;
        }

        // Used when transitioning to another question format (eg Mushra)
        // TODO: Hide other components from other scripts when completed
        public void HideComponents()
        {
            choice1button.SetActive(false);
            choice2button.SetActive(false);
            choice3button.SetActive(false);
            choice4button.SetActive(false);
            choice5button.SetActive(false);
        }

        public void ShowComponents()
        {
            choice1button.SetActive(true);
            choice2button.SetActive(true);
            choice3button.SetActive(true); 
            choice4button.SetActive(true);
            choice5button.SetActive(true);
        }
    }
}

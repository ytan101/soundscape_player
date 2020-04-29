using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interactive360
{
    public class MushraQns : MonoBehaviour
    {

        public static MushraQns instance = null;

        public GameObject track1_media;
        public Slider slider1;

        public GameObject track2_media;
        public Slider slider2;

        public GameObject track3_media;
        public Slider slider3;

        public GameObject next_button;
        public GameObject prev_button;

        public GameObject markers;

        private bool Slider1Selected;
        private bool Slider2Selected;
        private bool Slider3Selected;

        private bool track1state = false;
        private bool track2state = false;
        private bool track3state = false;

        private int VideoCounter = 4; // First video of mushra corresponds to scene_4
        private int VideoMax = 7;
        // private int TrackCounter = 0; // Handler for selecting which set of tracks to play
        private int ReaperCounter = 2; // First two tracks are reserved for COMPASS

        private float valueChange;

        // For switching questions, the left or right side of the trackpad has to be touched and the entire trackpad has to be pushed down
        [SerializeField] string m_trackpadHori = "TrackpadHori";
        [SerializeField] string m_trackpadDown = "SecondaryThumbstick";

        public int qnNo = 0;
        List<string> answers = new List<string>();
        string currentQn;
        string response;

        static readonly string mushraTextFile = @"D:\GitHub\soundscape_player\mushraQns.txt";
        string mushraAnswerFile = @"D:\GitHub\soundscape_player\mushraAns.txt";

        // Read from text file
        public void readQuestion()
        {
            string[] lines = File.ReadAllLines(mushraTextFile);
            if (qnNo < 0)
            {
                qnNo = 0;
            }
            // Checks which part of the questionnaire to set appropriate choice text
            currentQn = lines[qnNo];

            MenuManager.instance.questionText.SetText(lines[qnNo] + "\n" + lines[qnNo + 1]);
        }

        // Each qn has 3 lines in textfile
        public void NextQn()
        {
            ReaperManager.instance.StopOsc();
            // Change to count max number of scenes
            if (VideoCounter == VideoMax)
            {
                return;
            }
            qnNo += 2;
            // Bring to respective quizzes
            readQuestion();
            MenuManager.instance.UpdateMenu();
            VideoCounter++;
            GameManager.instance.SelectScene("scene_" + (VideoCounter).ToString());
            ReaperManager.instance.PlayOsc();
        }

        public void PrevQn()
        {
            ReaperManager.instance.StopOsc();
            if (VideoCounter == 4)
            {
                return;
            }
            qnNo -= 2;
            readQuestion();
            MenuManager.instance.UpdateMenu();
            VideoCounter--;
            GameManager.instance.SelectScene("scene_" + (VideoCounter).ToString());
            ReaperManager.instance.PlayOsc();
        }

        public void RecordResponse(string response)
        {
            response = currentQn + "," + response;
            answers.Insert(qnNo / 2, response);
            using (StreamWriter sw = new StreamWriter(mushraAnswerFile))
            {
                foreach (String s in answers)
                    sw.WriteLine(s);
            }
            readQuestion();
            NextQn();
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

        private void Update()
        {

            // Controls the speed of the slider
            valueChange = Input.GetAxis(m_trackpadHori);

            // Slider values handler
            if (Slider1Selected == true)
            {
                slider1.value += valueChange;
            }

            if (Slider2Selected == true)
            {
                slider2.value += valueChange;
            }

            if (Slider3Selected == true)
            {
                slider3.value += valueChange;
            }
        }

        // Each video has 3 tracks
        // First track handler
        private void Check1State()
        {
            if (track1state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/"+(1+ReaperCounter).ToString()+"/solo/toggle");
                track1state = false;
            }
        }

        // Second track handler
        private void Check2State()
        {
            if (track2state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/"+(2+ReaperCounter).ToString()+"/solo/toggle");
                track2state = false;
            }
        }

        // Third track handler
        private void Check3State()
        {
            if (track3state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/"+(3+ReaperCounter).ToString()+"/solo/toggle");
                track3state = false;
            }
        }

        // Slider 1 components
        public void Enter1()
        {
            Slider1Selected = true;
        }

        public void Exit1()
        {
            Slider1Selected = false;
        }

        // Make boolean values to tag to each track to allow simultaneous
        public void Play1()
        {
            track1_media.SetActive(false);
            track2_media.SetActive(true);
            track3_media.SetActive(true);

            Check2State();
            Check3State();

            track1state = true;
            Debug.Log("Playing track" + ReaperCounter.ToString());
            ReaperManager.instance.SendOscReaper("/track/" + (1 + ReaperCounter).ToString() + "/solo/toggle");
        }

        // Slider 2 components
        public void Enter2()
        {
            Slider2Selected = true;
        }

        public void Exit2()
        {
            Slider2Selected = false;
        }

        public void Play2()
        {
            track1_media.SetActive(true);
            track2_media.SetActive(false);
            track3_media.SetActive(true);

            Check1State();
            Check3State();

            track2state = true;
            ReaperManager.instance.SendOscReaper("/track/" + (2 + ReaperCounter).ToString() + "/solo/toggle");
        }

        // Slider 3 components
        public void Enter3()
        {
            Slider3Selected = true;
        }

        public void Exit3()
        {
            Slider3Selected = false;
        }

        public void Play3()
        {
            track1_media.SetActive(true);
            track2_media.SetActive(true);
            track3_media.SetActive(false);

            Check1State();
            Check2State();

            track3state = true;
            ReaperManager.instance.SendOscReaper("/track/" + (3 + ReaperCounter).ToString() + "/solo/toggle");
        }

        public void HideComponents()
        {
            slider1.gameObject.SetActive(false);
            track1_media.SetActive(false);

            slider2.gameObject.SetActive(false);
            track2_media.SetActive(false);

            slider3.gameObject.SetActive(false);
            track3_media.SetActive(false);

            next_button.SetActive(false);
            prev_button.SetActive(false);

            markers.SetActive(false);
        }

        public void ShowComponents()
        {
            markers.SetActive(true);

            slider1.gameObject.SetActive(true);
            track1_media.SetActive(true);

            slider2.gameObject.SetActive(true);
            track2_media.SetActive(true);

            slider3.gameObject.SetActive(true);
            track3_media.SetActive(true);

            // Since the first video has no "previous", we only show the next button.
            next_button.SetActive(true);

            markers.SetActive(true);
        }

        // Only NextRating records response
        public void NextRating()
        {
            // Max videos reached and then return to main menu
            if (VideoCounter == VideoMax)
            {
                string current_values = (slider1.value).ToString() + "," + (slider2.value).ToString() + "," + (slider3.value).ToString();
                RecordResponse(current_values);
                MenuManager.instance.DisplayMain();
            }
            else 
            {
                string current_values = (slider1.value).ToString() + "," + (slider2.value).ToString() + "," + (slider3.value).ToString();
                RecordResponse(current_values);

                // Reset all slider values
                ResetSliders();

                prev_button.SetActive(true);
                ResetSequence();
                ReaperCounter+=3;
            }
        }

        public void PrevRating()
        {
            ResetSliders();
            PrevQn();

            // First video reached
            if (VideoCounter == 4)
            {
                ResetSequence();
                ReaperCounter-=3;
                prev_button.SetActive(false);
                return;
            }

            ResetSequence();
            ReaperCounter-=3;
        }

        private void ResetSequence()
        {

            track1_media.SetActive(true);
            track2_media.SetActive(true);
            track3_media.SetActive(true);

            if (track1state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (1 + ReaperCounter).ToString() + "/solo/toggle");
            }

            if (track2state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (2 + ReaperCounter).ToString() + "/solo/toggle");
            }

            if (track3state == true)
            {
                ReaperManager.instance.SendOscReaper("/track/" + (3 + ReaperCounter).ToString() + "/solo/toggle");
            }

            track1state = false;
            track2state = false;
            track3state = false;
        }

        public void ResetSliders()
        {
            slider1.value = 0;
            slider2.value = 0;
            slider3.value = 0;
        }

        // Reset counters used and track states
        public void ResetCounters()
        {
            VideoCounter = 4;
            ReaperCounter = 2;
            qnNo = 0;
            track1state = false;
            track2state = false;
            track3state = false;
        }
    }
}


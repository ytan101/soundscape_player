using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

// TODO: Change all functions to capital letters
// TODO: Integrate osc func from Reaper to play/pause
// TODO: Export functions to separate file

// FEATURES TODO:
// Look up Mushra test (next menu after initial questions)
// Look up destroy existing objects and replace with sliders
// Sliders corresponding to number of videos


namespace Interactive360
{

    public class MenuManager : MonoBehaviour
    {

        public static MenuManager instance = null;

        public GameObject videoButtonManager;
        public Button[] m_videosInScene; //A reference to all the buttons in the scene that would load new scenes
        public GameObject m_menu; //A reference to the menu being rendered
        public GameObject playPauseManager;
        public GameObject m_playButton; //A reference to the button that toggles the video content to play
        public GameObject m_pauseButton; //A reference to the button that toggle the video content to pause

        public GameObject environmentSurvey;
        public GameObject mushraSurvey;
        public GameObject backToMain;

        public TextMeshProUGUI questionText; // For TextMeshPro label

        public string surveyState; // Shows which survey it is currently at

        // For naming output file according to selected video
        public string videoSelected;

        [SerializeField] string m_oculusMenuToggle = "MenuToggle"; //The name of the oculus button input that will toggle the scene on and off
        
        // For switching questions, the left or right side of the trackpad has to be touched and the entire trackpad has to be pushed down
        [SerializeField] string m_trackpadHori = "TrackpadHori";
        [SerializeField] string m_trackpadDown = "SecondaryThumbstick";

        private AudioSource m_menuToggleAudio; //Audio clip to play when the menu is closed

        // Time for modifying update rate
        private float time = 0.0f;
        public float interpolationPeriod = 500f;

        // Use this to prevent object reference error when calling from different classes
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

        // on Start, bind all buttons to their respective scenes and call DontDestroyOnLoad to keep the Main Menu in every scene
        void Start()
        {           
            DontDestroyOnLoad(gameObject);
            BindButtonsToScenes();
            m_menuToggleAudio = GetComponent<AudioSource>();

            // For binding TextMeshPro to GUI on the screen
            questionText = questionText.GetComponent<TextMeshProUGUI>();

            // Load initial menu components and then hide it
            DisplayMain();
            m_menu.SetActive(!m_menu.activeInHierarchy);

            Debug.Log("Opening Ports");

            // Add relevant components
            ReaperManager ReaperManager = gameObject.AddComponent<ReaperManager>();
            //MultiChoiceQns MultiChoiceQns = gameObject.AddComponent<MultiChoiceQns>();

            ReaperManager.instance.OpenPort();

            //openPorts();

            StartCoroutine("UpdateAudio");
        }

        // Menu handlers
        public void DisplayMain()
        {
            // Hide Components
            playPauseManager.SetActive(false);
            videoButtonManager.SetActive(false);
            questionText.enabled = false;
            backToMain.SetActive(false);
            MultiChoiceQns.instance.HideComponents();
            MushraQns.instance.HideComponents();

            // Show components
            environmentSurvey.SetActive(true);
            mushraSurvey.SetActive(true);
        }

        // Environment (multi-option survey) handler
        public void StartMulti()
        {
            MultiChoiceQns.instance.ResetTrackStates();

            ReaperManager.instance.OpenMultiChoicePorts();

            string sceneName = GameManager.instance.scenesToLoad[0];
            GameManager.instance.SelectScene(sceneName);

            surveyState = "multi";

            // Modify size of question text box
            RectTransform rt = questionText.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(200, rt.sizeDelta.y);

            // Show components
            videoButtonManager.SetActive(true);
            backToMain.SetActive(true);
            questionText.enabled = true;
            playPauseManager.SetActive(true);
            MultiChoiceQns.instance.ShowComponents();

            // Hide components
            MushraQns.instance.HideComponents();
            environmentSurvey.SetActive(false);
            mushraSurvey.SetActive(false);

            MultiChoiceQns.instance.readQuestion();

            ReaperManager.instance.PlayOsc();

            MultiChoiceQns.instance.Select1();
        }

        // Mushra handler
        public void StartMushra()
        {
            MushraQns.instance.ResetCounters();

            ReaperManager.instance.OpenMushraPorts();

            // Mushra videos start from the 4th scene, the first 3 belong to the multiple choice survey
            // !! Change to instance of counter in mushraQns

            GameManager.instance.SelectScene("scene_4");

            surveyState = "mushra";

            // Modify size of question text box
            RectTransform rt = questionText.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(215, rt.sizeDelta.y);

            // Show components
            backToMain.SetActive(true);
            questionText.enabled = true;
            playPauseManager.SetActive(true);
            MushraQns.instance.ShowComponents();

            // Hide components
            MultiChoiceQns.instance.HideComponents();
            environmentSurvey.SetActive(false);
            mushraSurvey.SetActive(false);

            MushraQns.instance.readQuestion();

            ReaperManager.instance.PlayOsc();
        }

        //call the CheckForInput method once per frame
        void Update()
        {
            CheckForInput();
        }

        // Updates audio according to a time period (adjust for latency)
        IEnumerator UpdateAudio()
        {
           for(; ; )
            {
                ReaperManager.instance.SendYPR();
                yield return new WaitForSeconds(0.128f);
            }
        }

        //if the menu is active, turn it off. If it is inactive, turn it on
        public void toggleMenu()
        {
            m_menu.transform.position = Camera.main.transform.forward*17;
            m_menu.transform.right = Camera.main.transform.right;
            m_menu.SetActive(!m_menu.activeInHierarchy); 
        }

        //If we detect input, call the toggleMenu method 
        private void CheckForInput()
        {
            bool menuState;
            //Debug.Log("MenuManager checking for input");
            //Debug.Log("MenuManger Input value" + Input.GetButton(m_oculusMenuToggle));
            //check for input from specified Oculus Touch button or the App button on Google Daydream Controller
            if (Input.GetButtonDown(m_oculusMenuToggle) || GvrControllerInput.AppButtonDown)
            {
                System.Threading.Thread.Sleep(100);
                toggleMenu();
                //if we have an audio source to play with menu toggle, play it now
                if (m_menuToggleAudio)
                    m_menuToggleAudio.Play();
            }
        }

        // Check if current question is answered using array (NOT IMPLEMENTED)
        public void CheckAnswered()
        {

        }

        // Toggle between showing play and pause button when pressed
        public void TogglePlayPause()
        {
            //TODO: Set state of button (true/false) according to icon display
            m_pauseButton.SetActive(!m_pauseButton.activeInHierarchy);
            m_playButton.SetActive(!m_playButton.activeInHierarchy);

        }

        /*
        public void showPlay()
        {
            m_playButton.SetActive(!m_playButton.activeInHierarchy);
        }
        */

        // Updates Menu
        public void UpdateMenu()
        {
            // Call twice as it hides then shows the updated menu
            m_menu.SetActive(!m_menu.activeInHierarchy);
            m_menu.SetActive(!m_menu.activeInHierarchy);
        }

        // Each button will match up to a respective scene. Button 1 in the Menu Manager will match up to Scene 1 in the Video Manager
        // Instantiate class from GameManager to add scenes
        public void BindButtonsToScenes()
        {
            for (int i = 0; i < m_videosInScene.Length; i++)
            {
                string sceneName = GameManager.instance.scenesToLoad[i];
                Debug.Log(sceneName);
                // Calls GameManager to change scene (under VideoManager in hierarchy)
                m_videosInScene[i].onClick.AddListener(() => GameManager.instance.SelectScene(sceneName));
                // Starts audio track based on selected video
                videoSelected = "video_choice_" + i;
            }
        }
    }
}


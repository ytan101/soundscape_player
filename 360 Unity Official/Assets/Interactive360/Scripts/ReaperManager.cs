using System;
using UnityEngine;


namespace Interactive360

{
    public class ReaperManager : MonoBehaviour
    {

        public static ReaperManager instance = null;

        public OscOut oscOut; // For OSC implementation to REAPER

        private float xAngle;
        private float yAngle;
        private float zAngle;

        private int YPRPorts;

        [SerializeField] OscOut _oscOutReaper; // Port for REAPER

        [SerializeField] OscOut _oscOutCOMPASSMulti; // Port for COMPASS (Multi Choice)

        // Ports for COMPASS (MUSHRA)
        [SerializeField] OscOut _oscOutCOMPASSMushra1;
        [SerializeField] OscOut _oscOutCOMPASSMushra2;

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

        public void OpenPort()
        {
            // Open OSC port for REAPER
            if (!_oscOutReaper) _oscOutReaper = gameObject.AddComponent<OscOut>();
            //_oscOutReaper.Open(9000, "127.0.0.1");
            _oscOutReaper.Open(9000, "192.168.1.46");

            Debug.Log("Ports opened");
        }

        public void OpenMultiChoicePorts()
        {
            YPRPorts = 1;

            if (!_oscOutCOMPASSMulti) _oscOutCOMPASSMulti = gameObject.AddComponent<OscOut>();
            _oscOutCOMPASSMulti.Open(1000, "127.0.0.1");
        }

        public void OpenMushraPorts()
        {
            YPRPorts = 2;

            if (!_oscOutCOMPASSMushra1) _oscOutCOMPASSMushra1 = gameObject.AddComponent<OscOut>();
            _oscOutCOMPASSMushra1.Open(1000, "127.0.0.1");

            if (!_oscOutCOMPASSMushra2) _oscOutCOMPASSMushra2 = gameObject.AddComponent<OscOut>();
            _oscOutCOMPASSMushra2.Open(1100, "127.0.0.1");
        }

        public void SendOscReaper(string cmd)
        {
            Debug.Log("Send Reaper Command" + cmd);
            OscMessage command = new OscMessage(cmd);
            _oscOutReaper.Send(command);
        }

        public void PlayOsc()
        {
            SendOscReaper("/play");
        }

        public void StopOsc()
        {
            SendOscReaper("/stop");
        }

        // Updates YPR values according to Vive tracker
        public void SendYPR()
        {
            xAngle = Camera.main.transform.eulerAngles.x; // pitch
            yAngle = Camera.main.transform.eulerAngles.y; // yaw
            zAngle = Camera.main.transform.eulerAngles.z; // roll
            xAngle = (float)Math.Round(xAngle - 180, 2);
            yAngle = (float)Math.Round(-(yAngle - 180), 2);
            zAngle = (float)Math.Round(zAngle - 180, 2);

            Debug.Log("Sending " + yAngle + " " + xAngle + " " + zAngle);

            switch (YPRPorts)
            {
                case 1:
                    _oscOutCOMPASSMulti.Send("/ypr", yAngle, xAngle, zAngle);
                    break;
                case 2:
                    _oscOutCOMPASSMushra1.Send("/ypr", yAngle, xAngle, zAngle);
                    _oscOutCOMPASSMushra2.Send("/ypr", yAngle, xAngle, zAngle);
                    break;
                default:
                    Debug.Log("No YPR ports are connected");
                    break;
            }

        }

    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Adnc.Cutscene {

    public class CutsceneSkipApi : MonoBehaviour {

        bool debug = false;
        bool _skipCutscene = false;

        [SerializeField, Range(1.0f, 5.0f), Tooltip("How long the graphic will display after initially pressing a button.")]
        float displayTime = 3f;


        [SerializeField, Tooltip("The key to monitor for to skip a cutscene.")]
        string skipAction;
        KeyCode skipActionCode;

        [SerializeField, Tooltip("The canvas used to contain the skip graphic.")]
        GameObject container;

        [SerializeField, Tooltip("Reference to the Animator placed on the skip graphic.")]
        Animator animGraphic;

        /* BEGIN Example area for the animator. This is good to have in-case something is altered with the animator's functionality
        [Header("Animated Graphic")]
        [SerializeField] Animator animGraphic;
        [SerializeField] List<string> resetTriggers;
        [SerializeField] string defaultState;
        END Example */

        public bool SkipCutscene {
            get { return _skipCutscene; }
        }

        public void Awake() {
            skipActionCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), skipAction);

            // @REVIEW I would add a bool kind of like debug that auto starts this up (ex. bool forceStart), then you don't need to delete this later
            BeginMonitor(); //@TODO delete after testing
        }

        //At the start of a cutscene, run this script to begin monitoring for button pressing
        public void BeginMonitor() {
            if (debug) Debug.LogFormat("{0} triggered BeginMonitor", name);

            animGraphic.SetTrigger("Idle");

            container.SetActive(true);
            StartCoroutine("LoopMonitor");
        }

        //Run this script either at the end of a cutscene or if the scene was skipped with a button press
        public void EndMonitor() {
            if (debug) Debug.LogFormat("{0} triggered EndMonitor", name);
            container.SetActive(false);
            StopAllCoroutines();
        }

        IEnumerator LoopMonitor() {

            if (debug) Debug.LogFormat("{0} triggered LoopMonitor", name);

            //yield return at the bottom of this page prevents this loop from crashing
            //The only way to exit this loop is to run EndMonitor, found in the second while loop
            while (true) {

                while (!Input.anyKey) yield return null;

                if (debug) Debug.LogFormat("A key or mouse click has been detected. Press '{0}' to stop the monitor.", skipAction);
                animGraphic.SetTrigger("Show");

                yield return null;
                float timer = displayTime;

                while (timer > 0) {

                    //@TODO Replace keycode with an input from Rewired
                    if (Input.GetKeyDown(skipActionCode)) {
                        if (debug) Debug.LogFormat("'{0}' has been pressed.", skipAction);

                        _skipCutscene = true;
                        yield return null;
                        _skipCutscene = false;

                        EndMonitor();
                    }

                    timer -= Time.deltaTime;
                    yield return null;

                }

                if (debug) Debug.LogFormat("The '{0}' key was never pressed. Going back to checking for any key press", skipAction);
                animGraphic.SetTrigger("Hide");
                yield return null;

            }

        }

    }

}
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Adnc.Cutscene {
    // @REVIEW I recommend adding a [Tooltip("stuff")] to variables visible in the inspector
    public class CutsceneSkipApi : MonoBehaviour {
        // @REVIEW Remove public, no need to change this at run-time
        public bool debug = false;
        bool _skipCutscene = false;

        // @REVIEW You can separate these two by just a comma, ex. [SerializeField, Range(1.0f, 5.0f)]
        [Range(1.0f, 5.0f)] [SerializeField] float displayTime = 3f;

        [SerializeField] string skipAction;
		KeyCode skipActionCode;

        // @REVIEW Change this to container (me being picky)
        [SerializeField] GameObject canvas;

        // @REVIEW Change this to Animator animGraphic;
        [SerializeField] GameObject text;

        /* BEGIN Example area for the animator. This is good to have in-case something is altered with the animator's functionality
        [Header("Animated Graphic")]
        [SerializeField] Animator animGraphic;
        [SerializeField] List<string> resetTriggers;
        [SerializeField] string defaultState;
        END Example */

        public void Awake () {
            skipActionCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), skipAction);

            // @REVIEW I would add a bool kind of like debug that auto starts this up (ex. bool forceStart), then you don't need to delete this later
            BeginMonitor(); //@TODO delete after testing
        }

        //At the start of a cutscene, run this script to begin monitoring for button pressing
        public void BeginMonitor () {
            if (debug) Debug.LogFormat("{0} triggered BeginMonitor", name);

            // @REVIEW Before the canvas is set to active, you need to clear both triggers on the animator and set it to the Idle state let me know if you
            // need help on this one

            canvas.SetActive(true);
            StartCoroutine("LoopMonitor");
        }

        //Run this script either at the end of a cutscene or if the scene was skipped with a button press
        public void EndMonitor () {
            if (debug) Debug.LogFormat("{0} triggered EndMonitor", name);
            canvas.SetActive(false);
            StopAllCoroutines();
        }

        IEnumerator LoopMonitor () {

            if (debug) Debug.LogFormat("{0} triggered LoopMonitor", name);

            // @REVIEW We don't need this since we can serialize the Animator directly
            // Be warned that GetComponent is resource intensive, best to only run it in Awake as a good rule of thumb
            Animator textAnimator = text.GetComponent<Animator>();

            // @REVIEW A few comments in crazier loop code like this wouldn't be a bad idea, should help to quickly figure out what's going on when someone
            // else pulls it up or you open it after 6 months
            while (true) {

                while (!Input.anyKey) yield return null;

                if (debug) Debug.LogFormat("A key or mouse click has been detected. Press '{0}' to stop the monitor.", skipAction);
                textAnimator.SetTrigger("Show");

                yield return null;
                float timer = displayTime;

                while (timer > 0) {

                    //@TODO Replace keycode with an input from Rewired
                    if (Input.GetKeyDown(skipActionCode)) {
                        if (debug) Debug.LogFormat("'{0}' has been pressed.", skipAction);
                        // @REVIEW Don't worry about this (remove it), you can force skip the cutscene (an abrupt skip is common in most games)
                        textAnimator.SetTrigger("Hide");

                        _skipCutscene = true;
                        yield return null;
                        _skipCutscene = false;

                        // @REVIEW You shouldn't need this here, should immediately skip the cutscene
                        yield return new WaitForSeconds(1f);
                        EndMonitor();
                    }

                    timer -= Time.deltaTime;
                    yield return null;

                }

                if (debug) Debug.LogFormat("The '{0}' key was never pressed. Going back to checking for any key press", skipAction);
                textAnimator.SetTrigger("Hide");
                yield return null;

            }

        }

        // @REVIEW Accessors should be immeidately after fields (towards top of page). Order should go fields -> accessors w/ field (if any) -> methods
        public bool SkipCutscene {
            get { return _skipCutscene; }
        }

    }

}
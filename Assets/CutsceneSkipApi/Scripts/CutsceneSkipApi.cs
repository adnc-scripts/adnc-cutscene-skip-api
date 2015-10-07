using UnityEngine;
using System.Collections;

namespace Adnc.Cutscene {

    public class CutsceneSkipApi : MonoBehaviour {
        bool _skipCutscene = false;
		public bool SkipCutscene {
			get { return _skipCutscene; }
		}

        [SerializeField, Range(1.0f, 5.0f), Tooltip("How long the graphic will display after initially pressing a button.")]
        float displayTime = 3f;

        [SerializeField, Tooltip("The key to monitor for to skip a cutscene.")]
        string skipAction;
        KeyCode skipActionCode;

        [SerializeField, Tooltip("The canvas used to contain the skip graphic.")]
        GameObject container;

		[Header("Animated Graphic")]

        [SerializeField, Tooltip("Reference to the Animator placed on the skip graphic.")]
        Animator animGraphic;

		[SerializeField, Tooltip("Default starting state when reset")]
		string animStartState = "Idle";

		[Header("Debugging")]

		[SerializeField, Tooltip("Adds additional log details")] 
		bool debug = false;

		[SerializeField, Tooltip("Will automatically activate the cutscene skip API when the game starts")] 
		bool autoStart = false;

        public void Awake() {
            skipActionCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), skipAction);

            if (autoStart) BeginMonitor();
        }

        //At the start of a cutscene, run this script to begin monitoring for button pressing
        public void BeginMonitor() {
            if (debug) Debug.LogFormat("{0} triggered BeginMonitor", name);

			AnimatorExt.SetAllTriggers(animGraphic, false);
            animGraphic.Play(animStartState);

            container.SetActive(true);
            StopAllCoroutines();
            StartCoroutine("LoopMonitor");
        }

        //Run this script either at the end of a cutscene or if the scene was skipped with a button press
        public void EndMonitor() {
            if (debug) Debug.LogFormat("{0} triggered EndMonitor", name);
            container.SetActive(false);
            StopAllCoroutines();
        }

        IEnumerator LoopMonitor() {
			float timer;

            if (debug) Debug.LogFormat("{0} triggered LoopMonitor", name);

            //yield return at the bottom of this page prevents this loop from crashing
            //The only way to exit this loop is to run EndMonitor, found in the second while loop
            while (true) {

				// Constantly prevents advancing logic unless a button press is detected
                while (!Input.anyKey) yield return null;

                if (debug) Debug.LogFormat("A key or mouse click has been detected. Press '{0}' to stop the monitor.", skipAction);
                animGraphic.SetTrigger("Show");

                yield return null;
                timer = displayTime;

                while (timer > 0) {

                    //@TODO Replace keycode with an input from Rewired
                    if (Input.GetKeyDown(skipActionCode)) {
                        if (debug) Debug.LogFormat("'{0}' has been pressed.", skipAction);

                        _skipCutscene = true;
                        yield return null;
                        _skipCutscene = false;

                        EndMonitor();
					} else if (Input.anyKey) {
						timer = displayTime;
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
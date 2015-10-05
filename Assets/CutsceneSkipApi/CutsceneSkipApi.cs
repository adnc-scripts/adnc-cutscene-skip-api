using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Adnc.Cutscene {

    public class CutsceneSkipApi : MonoBehaviour {

        public bool debug = false;
        bool _skipCutscene = false;

        [Range(1.0f, 5.0f)] [SerializeField] float displayTime = 3f;

        [SerializeField] string skipAction;
		KeyCode skipActionCode;

        [SerializeField] GameObject canvas;
        [SerializeField] GameObject text;

        public void Awake () {
            skipActionCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), skipAction);
            BeginMonitor(); //@TODO delete after testing
        }

        //At the start of a cutscene, run this script to begin monitoring for button pressing
        public void BeginMonitor () {
            if (debug) Debug.LogFormat("{0} triggered BeginMonitor", name);
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
            Animator textAnimator = text.GetComponent<Animator>();

            while (true) {

                while (!Input.anyKey) yield return null;

                if (debug) Debug.LogFormat("A key or mouse click has been detected. Press '{0}' to stop the monitor.", skipAction);
                textAnimator.SetTrigger("Show");

                yield return null;
                float timer = displayTime;

                while (timer > 0) {

                    //@TODO Replace keycode with an input from Rewired
                    if (Input.GetKeyDown(skipActionCode))
                    {
                        if (debug) Debug.LogFormat("'{0}' has been pressed.", skipAction);
                        textAnimator.SetTrigger("Hide");

                        _skipCutscene = true;
                        yield return null;
                        _skipCutscene = false;

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

        public bool SkipCutscene {
            get { return _skipCutscene; }
        }

    }

}
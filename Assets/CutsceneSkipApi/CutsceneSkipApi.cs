using UnityEngine;
using System.Collections;

namespace Adnc.Cutscene
{

    public class CutsceneSkipApi : MonoBehaviour
    {

        [SerializeField]
        float displayTime = 3;

        [SerializeField]
        string skipAction;

        public void BeginMonitor ()
        {
            Debug.Log("Starting monitor");
            StartCoroutine("LoopMonitor");
        }

        public void EndMonitor()
        {
            Debug.Log("Ending monitor");
            StopCoroutine("LoopMonitor");
        }

        IEnumerator LoopMonitor ()
        {

            Debug.Log("Loop monitor start.");

            while (!Input.anyKey) yield return null;

            Debug.Log("A key or mouse click has been detected. Press 'A' to stop the monitor.");

            yield return new WaitForSeconds(0.1f);
            float timer = displayTime;

            while (timer > 0)
            {

                if (Input.GetKey(KeyCode.A))
                {
                    Debug.Log("'A' has been pressed.");
                    SkipScene();
                    EndMonitor();
                    break;
                }

                timer -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);

            }

            if (timer > 0) {
                yield return null;
            } 
            else
            {
                Debug.Log("The 'A' key was never pressed. Going back to checking for any key press.");
                yield return StartCoroutine("LoopMonitor");
            }

        }

        public void SkipScene ()
        {
            Debug.Log("Skip the scene.");
        }

        //delete after testing
        public void Start()
        {
            BeginMonitor();
        }

    }

}



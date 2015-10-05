using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Adnc.Cutscene
{

    public class CutsceneSkipApi : MonoBehaviour
    {

        [SerializeField]
        float displayTime = 3;

        [SerializeField]
        string skipAction;
		KeyCode skipActionCode;

        public void BeginMonitor ()
        {
			//At the start of a cutscene, run this script to begin monitoring for button pressing

            Debug.Log("Starting monitor");
            StartCoroutine("LoopMonitor");
        }

        public void EndMonitor()
        {
			//Run this script either at the end of a cutscene or if the scene was skipped with a button press

            Debug.Log("Ending monitor");
            StopCoroutine("LoopMonitor");
        }

        IEnumerator LoopMonitor ()
        {

            Debug.Log("Loop monitor start.");

            while (!Input.anyKey) yield return null;

            Debug.Log("A key or mouse click has been detected. Press '" + skipAction + "' to stop the monitor.");

			StartCoroutine ("ShowUIText");

            yield return new WaitForSeconds(0.1f);
            float timer = displayTime;

            while (timer > 0)
            {

				//Replace keycode with an input from Rewired
				if (Input.GetKey(skipActionCode))
                {
                    Debug.Log("'" + skipAction + "' has been pressed.");
                    SkipScene();
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
                Debug.Log("The " + skipAction + " key was never pressed. Going back to checking for any key press.");

				StartCoroutine ("HideUIText");
								
				yield return StartCoroutine("LoopMonitor");
            }

        }

        public void SkipScene ()
        {

			Debug.Log("Skip the scene.");
						
			EndMonitor();
			StartCoroutine ("HideUIText");

			//Add code to end scene via Playmaker
        }

		IEnumerator ShowUIText() 
		{

			Text text = GetComponent<Text> ();
			float currentTime = 0;

			while (text.color.a < 1f) 
			{

				currentTime += Time.deltaTime;

				float alpha = Mathf.Lerp(text.color.a, 1f, 1f * currentTime/3f);
				text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
				yield return new WaitForEndOfFrame();

			}

			text.color = Color.white;

			yield return null;

		}

		IEnumerator HideUIText()
		{
			Debug.Log ("Hiding Text");
			StopCoroutine ("ShowUIText");

			Text text = GetComponent<Text> ();
			float currentTime = 0f;

			while (text.color.a > 0f) 
			{

				currentTime += Time.deltaTime;

				float alpha = Mathf.Lerp(1f, 0f, 1f * currentTime);
				text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
				yield return new WaitForEndOfFrame();
				
			}

			yield return null;

		}

        public void Start()
        {	
			skipActionCode = (KeyCode)System.Enum.Parse (typeof(KeyCode), skipAction);
			BeginMonitor(); //delete after testing
        }

    }

}



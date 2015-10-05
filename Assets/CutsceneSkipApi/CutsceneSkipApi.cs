using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// @REVIEW Make sure all brackets are lined up directly as so "namespace Adnc.Cutscene {", putting a bracket on a new line is an
// older MS standard we don't follow. It eats up a lot of page space fast
namespace Adnc.Cutscene
{
	// @REVIEW Consider hiding the Canvas completely as a GameObject until BeginMonitor is run to prevent a
	// performance impact

	// @REVIEW Thsi should be designed to be placed directly on the top most element (the Canvas). Be careful not to
	// nest scripts with interfaces on child elements as they can easily get lost.

	// @REVIEW Try to avoid adding too much whitespace, pushes code below the fold quickly
    public class CutsceneSkipApi : MonoBehaviour
    {

    	// @REVIEW Make this a range slider from 1 - 5 for ease of adjustment
        [SerializeField]
        float displayTime = 3;

        [SerializeField]
        string skipAction;

        // @REVIEW This will
		KeyCode skipActionCode;

		// @REVIEW Canvas and text container should both be targetable serialized fields as GameObjects
		// Ex. [SerializeField] GameObject canvas;

		// @REVIEW Place documentation for a method here instead of inside the method. Some programs will auto pull comments
		// if you put them here.
        public void BeginMonitor ()
        {
			//At the start of a cutscene, run this script to begin monitoring for button pressing

        	// @REVIEW Debug.Log commands should be togglable with a serialized private bool. Also you might want to be more specific
        	// with the debug logs in-case several things are all running at the same time
        	// Example if (debug) Debug.LogFormat("{0} triggered BeginMonitor", name);
            Debug.Log("Starting monitor");
            StartCoroutine("LoopMonitor");
        }

        // @REVIEW Same as BeginMonitor comments
        // @REVIEW Please be consistent with method spacing. EndMonitor () represents a method declaration. EndMonitor() represents
        // a method being called in code.
        public void EndMonitor()
        {
			//Run this script either at the end of a cutscene or if the scene was skipped with a button press

            Debug.Log("Ending monitor");

            // @REVIEW I recommend using StopAllCoroutines here
            StopCoroutine("LoopMonitor");

            // @REVIEW Needs to force hide the canvas element so it instantly vanishes
        }

        IEnumerator LoopMonitor ()
        {

            Debug.Log("Loop monitor start.");

            while (!Input.anyKey) yield return null;

            // @REVIEW Use Debug.LogFormat() to insert variables instead of using string concatenation
            Debug.Log("A key or mouse click has been detected. Press '" + skipAction + "' to stop the monitor.");

            // @REVIEW Coroutine not necessary, see notes on method
			StartCoroutine ("ShowUIText");

			// @REVIEW Simply skip a frame instead with "yield return null;"
            yield return new WaitForSeconds(0.1f);
            float timer = displayTime;

            while (timer > 0)
            {
            	// @REVIEW Use GetKeyDown instead of GetKey so it only triggers on a full button press

            	// @REVIEW Mark comments that indicate icomplete code with @TODO, this makes them globally searchable
				//Replace keycode with an input from Rewired
				if (Input.GetKey(skipActionCode))
                {
                    Debug.Log("'" + skipAction + "' has been pressed.");
                    SkipScene();
                    break;
                    // @REVIEW Don't execute a break, stop the coroutine completely
                }

                timer -= Time.deltaTime;

                // @REVIEW Use "yield return null;" instead to skip a frame, this could introduce bugs
                yield return new WaitForSeconds(Time.deltaTime);

            }

            // @REVIEW You should get rid of this entire chunk of code. Instead wrap the while (timer > 0) code with another
            // while loop that looks like "while (true) { yield return null; }" and place all your logic there. Cosntantly
            // triggering the same coroutine is quite expensive.
            if (timer > 0) {
                yield return null;
            } 
            else // @REVIEW correct nesting is "} else {"
            {
                Debug.Log("The " + skipAction + " key was never pressed. Going back to checking for any key press.");

                // @REVIEW Should throw a mecanim command at an animator, see notes on HideUIText for additional details
				StartCoroutine ("HideUIText");
								
				yield return StartCoroutine("LoopMonitor");
            }

        }

        // @REVIEW This should be an accessor with get; that returns true on the first frame of the cutscene being skipped only
        public void SkipScene ()
        {

			Debug.Log("Skip the scene.");
						
			EndMonitor();
			StartCoroutine ("HideUIText");

			//Add code to end scene via Playmaker
        }

        // @REVIEW We will be animating a complex image at some point. This should enable a GameObject container with a Mecanim
        // animation command insted of directly targeting text. On 2nd thought you shouldn't need this coroutine at all if Mecanim
        // handles the animation instead (also more flexible for designers like Rachel).
		IEnumerator ShowUIText() 
		{

			Text text = GetComponent<Text> ();
			float currentTime = 0;

			// @REVIEW Fading should be done with a Mecanim animator instead of directly in code, this avoids
			// having to manually adjust the code for display.
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

		// @REVIEW Same as ShowUIText, no reason to manually animate this with a coroutine, send a Mecanim command instead
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

		// @REVIEW Start, Awake, and other initializaiton commands should always be the first listed method
        public void Start()
        {	
			skipActionCode = (KeyCode)System.Enum.Parse (typeof(KeyCode), skipAction);
			BeginMonitor(); //delete after testing
        }

    }

}



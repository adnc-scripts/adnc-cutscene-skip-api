using UnityEngine;
using System.Collections;

namespace Adnc {
	static public class AnimatorExt {
		static public void SetAllTriggers (Animator anim, bool triggerState) {
			foreach (AnimatorControllerParameter param in anim.parameters) {
				if (param.type == AnimatorControllerParameterType.Trigger) {
					if (triggerState) {
						anim.SetTrigger(param.name);
					} else {
						anim.ResetTrigger(param.name);
					} 
				}
			}
		}
	}
}

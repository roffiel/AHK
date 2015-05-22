using UnityEngine;
using System.Collections;

namespace Wink{

	/// <summary>Script que se agrega a una camara para ser controlada por giroscopio.</summary>
	public class GyroCamBehaviour : MonoBehaviour {

		/// <summary>Activa o desactiva el control por giroscopio.</summary>
		private bool m_enable = true;

		/// <summary>Target que activa o desactiva el control por orientacion.</summary>
		//public EnhancedTrackableEventHandler m_target;

		/// <summary>Indica si el dispositivo soporta giroscopio.</summary>
		private bool gyroBool;

		/// <summary>Instancia del giroscopio.</summary>
		private Gyroscope gyro;

		/// <summary>Compensacion de rotacion segun la orientacion de la pantalla.</summary>
		private Quaternion rotFix;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Wink.GyroCam"/> enable gyro tracking.
		/// </summary>
		/// <value><c>true</c> if enable gyro tracking; otherwise, <c>false</c>.</value>
		public bool Enable{
			set { m_enable = value; }
			get { return m_enable; }
		}



		// Use this for initialization
		void Start () {

			//Linkear eventos del target seleccionado
			//m_target.onTargetFound += DesactivarGyroControl;
			//m_target.onTargetLost += ActivarGyroControl;

			Transform originalParent = transform.parent; // check if this transform has a parent
			GameObject camParent = new GameObject ("camParent"); // make a new parent
			camParent.transform.position = transform.position; // move the new parent to this transform position
			transform.parent = camParent.transform; // make this transform a child of the new parent
			camParent.transform.parent = originalParent; // make the new parent a child of the original parent
			
			gyroBool = SystemInfo.supportsGyroscope;
			
			if (gyroBool) {
				
				gyro = Input.gyro;
				gyro.enabled = true;
				
				if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
					camParent.transform.eulerAngles = new Vector3(90f,90f,0);
				} else if (Screen.orientation == ScreenOrientation.Portrait) {
					camParent.transform.eulerAngles = new Vector3(90f,180f,0);
				}
				
				if (Screen.orientation == ScreenOrientation.LandscapeLeft) {
					rotFix = new Quaternion(0,0,0.7071f,0.7071f);
				} else if (Screen.orientation == ScreenOrientation.Portrait) {
					rotFix = new Quaternion(0,0,1f,0);
				}
				//Screen.sleepTimeout = 0;
			} else {
				print("NO GYRO");
			}
		}
		
		// Update is called once per frame
		void Update () {

			if (gyroBool && m_enable) {
				Quaternion camRot = gyro.attitude * rotFix;
				transform.localRotation = camRot;
			}
		}

		/*void ActivarGyroControl(EnhancedTrackableEventHandler sender){
			m_enable = true;
		}
		
		void DesactivarGyroControl(EnhancedTrackableEventHandler sender){
			m_enable = false;
		}*/

	}
}

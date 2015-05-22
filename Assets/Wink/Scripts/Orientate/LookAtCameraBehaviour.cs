/*********************************************************************
 * 01-10-14 Raschid
 *  - Creado
 * *******************************************************************/

using UnityEngine;
using System.Collections;

namespace Wink{

	/// <summary>
	/// Comportamiento para hacer un objeto plano girar de frente hacia la camara sobre su eje Y.
	/// </summary>
	/// <remarks>Por ahora solo funciona cuando este objeto se encuentra dentro de un padre cuyo eje Z
	/// es perpendicular al eje Y de este objeto.
	public class LookAtCameraBehaviour : MonoBehaviour {

		/// <summary>Si es verdadero, el objeto gira hacia la camara. De lo contrario, se detiene.</summary>
		public bool m_active = false;

		/// <summary>Rotacion adicional en X</summary>
		public float m_rotX;

		/// <summary>Rotacion adicional en Y</summary>
		public float m_rotY;

		/// <summary>Rotacion adicional en Z</summary>
		public float m_rotZ;

		// Update is called once per frame
		void Update () {

			if(m_active){
				float x = m_rotX, y = m_rotY, z = m_rotZ;
				Vector3 cameraPos = new Vector3(Camera.main.transform.position.x,
				                                 transform.position.y,
				                                 Camera.main.transform.position.z);

				transform.LookAt(cameraPos, Vector3.right);

				//Funciona solo entre los primeros 180°
				Vector3 cameraPosInParentSpace = transform.parent.InverseTransformPoint(cameraPos);
				if(cameraPosInParentSpace.z > 0){
					y += 180f;
					z += 180f;
				}

				//Rotar para acomodar el objeto de frente
				transform.Rotate (x,y,z);
			}

		}//update()
	}
}
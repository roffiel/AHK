using UnityEngine;
using System.Collections;
using Wink;

/// <summary>
/// Este script orienta un objeto hacia el norte magnetico en el eje Z.
/// </summary>
public class PointNorthBehaviour : MonoBehaviour {

	/// <summary>Camara en la escena controlada por giroscopio.</summary>
	public GyroCamBehaviour m_gyroCam;

	/// <summary>Para indicar si ya ha sido inicializada la brujula.</summary>
	private bool m_oriented = false;

	// Use this for initialization
	void Start () {
		m_gyroCam.Enable = false;
		Input.compass.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(!m_oriented){

			if(Input.compass.timestamp <= 0) return;

			// Orient an object to point to magnetic north.
			//float currentY = m_gyroCam.gameObject.transform.localRotation.y;
			transform.rotation = Quaternion.Euler(0, -Input.compass.magneticHeading, 0);
			m_gyroCam.Enable = true;
			m_oriented = true;
		}
	}
}

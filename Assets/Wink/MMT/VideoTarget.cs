/*************************************
 * Las modified: 2014-02-13 Raschid
 * ***********************************/

using UnityEngine;
using System.Collections;
using MMT;

namespace Wink{

	[RequireComponent (typeof (AudioSource))]
	[RequireComponent (typeof (MobileMovieTexture))]
	///<summary>
	///Reproduce una videotextura y su audio automáticamente al ser encontrado el target y la pausa al perderse.
	///</summary>
	public class VideoTarget : Wink.EnhancedTrackableEventHandler {

		#region PUBLIC_VARIABLES
		/// <summary>(Opcional) Videomanager para coordinar la reproduccion junto con otros videos.</summary>
		public VideoManager m_videoManager;

		///<summary>Controla la reproducción/pausa.</summary>
		public bool m_pause = false;

		/// <summary>Pausar o rebobinar al perder el target.</summary>
		public OnLostOptions m_onLost = OnLostOptions.ContinueMute;

		/// <summary>Repite la reproduccion si el valor es verdadero.</summary>
		public bool m_loop = false;

		///<summary>Opciones en caso de perder el target.</summary>
		public enum OnLostOptions{
			/// <summary>La reproduccion se detiene y no se muestra. 
			/// Comienza desde el inicio al detectar el target de nuevo.</summary>
			Stop,
			
			/// <summary>La reproduccion se pausa y no se muestra. 
			/// Se reaunda al detectar el target de nuevo.</summary>
			Pause,
			
			/// <summary>La reproduccion no se muestra pero continua y se escucha
			/// el audio.</summary>
			ContinueAudio,
			
			/// <summary>La reproduccion no se muestra pero continua sin audio hasta 
			/// detectar  el target de nuevo.</summary>
			ContinueMute
		}
		#endregion

		#region PRIVATE_VARIABLES
		///<summary>Latch para pausa.</summary>
		private bool m_lastPauseState;
		
		///<summary>Componente de audio a reproducir junto con el video.</summary>
		private AudioSource m_audio;
		
		///<summary>Videotextura a reproducir.</summary>
		private MobileMovieTexture m_mmt;
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Asigna o devuelve la accion a tomar en caso de perder el target.
		/// </summary>
		/// <value>Accion.</value>
		public OnLostOptions OnLostAction{
			get{ return m_onLost;}
			set{ m_onLost = value;}
		}
		#endregion


		new void Start(){
			base.Start();
			
			//Iniciar latch
			m_lastPauseState = m_pause;
			
			//Inicializar componente
			m_mmt = GetComponentInChildren<MobileMovieTexture>();
			m_audio = GetComponentInChildren<AudioSource>();

			//Sobrescribir configuracion
			m_mmt.LoopCount = 0;
			m_mmt.PlayAutomatically = false;
			m_audio.loop = false;
			m_audio.playOnAwake = false;
			
			//Eventos
			m_mmt.onFinished += HandleFinish;
			onTargetFound += HandleTargetFound;
			onTargetLost += HandleTargetLost;
		}
		
		new void Update(){
			base.Update();
			
			if(m_pause != m_lastPauseState){
				m_lastPauseState = m_pause;
				if(m_pause)
					PauseAudioVideo();
				else
					PlayAudioVideo();
			}
		}
		
		#region PLAYBACK_METHODS
		/// <summary>
		/// Comienza o reanuda la reproduccion de audio y video.
		/// </summary>
		public void PlayAudioVideo(){
			if (m_audio != null){ 
				if(!m_audio.isPlaying) m_audio.Play ();
				m_audio.mute = false;
			}
			
			//por si no se ha inicializado aun la MMT
			if(m_mmt != null){
				if(!m_mmt.Pause) 
					if(!m_mmt.IsPlaying) m_mmt.Play ();

				m_mmt.Pause = false;
				m_pause = m_mmt.Pause;
				
				//activar render
				MeshRenderer mr = m_mmt.gameObject.GetComponentInChildren<MeshRenderer>();
				mr.enabled = true;
			}
		}

		/// <summary>
		/// Silencia el video. Utilce PlayAudioVideo() para desilenciar.
		/// </summary>
		public void MuteAudio(){
			m_audio.mute = true;
		}

		/// <summary>
		/// Pausa audio y video
		/// </summary>
		public void PauseAudioVideo(){
			if (m_audio != null) m_audio.Pause ();

			//por si no se ha inicializado aun la MMT
			if(m_mmt != null){
				m_mmt.Pause = false;
				m_pause = m_mmt.Pause;
				m_lastPauseState = m_pause;
			}
		}
		
		/// <summary>
		/// Detiene audio y video y libera la memoria.
		/// </summary>
		public void StopAudioVideo(){
			if (m_audio != null) m_audio.Stop ();
			
			//por si no se ha inicializado aun la MMT
			if(m_mmt != null){
				m_mmt.Stop ();
				m_mmt.Pause = false;
				m_pause = m_mmt.Pause;
				m_lastPauseState = m_pause;
				
				//desactivar render
				MeshRenderer mr = m_mmt.gameObject.GetComponentInChildren<MeshRenderer>();
				mr.enabled = false;
			}
		}
		
		/// <summary>
		/// Rebobina el video y lo pausa.
		/// </summary>
		public void RewindAudioVideo(){
			if (m_audio != null) m_audio.Stop ();
			
			if(m_mmt != null){//por si no se ha inicializado aun la MMT
				m_mmt.Pause = true;
				m_pause = m_mmt.Pause;
				m_lastPauseState = m_pause;

				m_mmt.Play();
				m_mmt.PlayPosition = 0;
			}
		}
		#endregion
		
		#region EVENT_HANDLING
		/// <summary>
		/// Al terminar la reproduccion del video lo rebobina y detiene.
		/// </summary>
		public void HandleFinish(MobileMovieTexture sender){
			Debug.Log ("video finished.");
			RewindAudioVideo();
			if(m_loop) PlayAudioVideo();
		}

		/// <summary>
		/// Al encontrar el target reanuda la reproduccion o da aviso al <see cref="Wink.VideoManager"/> en 
		/// caso de contar con uno.
		/// </summary>
		public void HandleTargetFound(EnhancedTrackableEventHandler sender){
			if(m_videoManager != null)
				m_videoManager.HandleTargetFound(this);
			else
				PlayAudioVideo();
		}

		/// <summary>
		/// Al perder el target detiene la reproduccion o da aviso al <see cref="Wink.VideoManager"/> en 
		/// caso de contar con uno.
		/// </summary>
		public void HandleTargetLost(EnhancedTrackableEventHandler sender){
			if(m_videoManager != null)
				m_videoManager.HandleTargetLost(this);
			else{
				switch(m_onLost){
				case OnLostOptions.Stop:
					StopAudioVideo();
					break;
				case OnLostOptions.Pause:
					PauseAudioVideo();
					break;
				case OnLostOptions.ContinueMute:
					MuteAudio ();
					break;
				default:
					break;
				}
			}
		}
		#endregion
		
	}
}

using UnityEngine;
using System.Collections;
using MMT;

namespace Wink{
	/// <summary>
	/// Reproduce o detiene automáticamente el video contenido al encontrar o perder un <see cref="Wink.VideoTarget"/>.
	/// Controla el playback de los videos para que solo se reproduzca uno a la vez.
	/// </summary>
	public class VideoManager : MonoBehaviour {

		/// <summary>Video actual en reproduccion.</summary>
		private VideoTarget m_lastPlayedVideo ;


		#region EVENT_HANDLING
		/// <summary>
		/// Detiene todas las reproducciones y reproduce solo el target especifico.
		/// </summary>
		/// <param name="sender">Target encontrado.</param>
		public void HandleTargetFound(VideoTarget sender){

			//Detener video actual
			if(m_lastPlayedVideo != null && m_lastPlayedVideo != sender)
				m_lastPlayedVideo.StopAudioVideo();

			//Reproducir nuevo
			sender.PlayAudioVideo();
			m_lastPlayedVideo = sender;
		}


		/// <summary>
		/// Detiene y reinicia la reproducción del video target.
		/// </summary>
		/// <param name="sender">Target perdido.</param>
		public void HandleTargetLost(VideoTarget sender){
			switch(sender.OnLostAction){
			case VideoTarget.OnLostOptions.Stop:
				sender.StopAudioVideo();
				break;
			case VideoTarget.OnLostOptions.Pause:
				sender.PauseAudioVideo();
				break;
			case VideoTarget.OnLostOptions.ContinueMute:
				sender.MuteAudio();
				break;
			default:
				break;
			}
		}
		#endregion

	}
}
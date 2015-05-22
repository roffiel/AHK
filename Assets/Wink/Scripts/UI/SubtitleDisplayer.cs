using UnityEngine;
using System.Collections;

namespace Wink{
	/// <summary>
	/// Esta clase se encarga de desplegar subtitulos en el GUI.
	/// </summary>
	public class SubtitleDisplayer : MonoBehaviour {

		/// <summary>Skin para aplicar a los subtitulos.</summary>
		public GUISkin m_skin;

		/// <summary>Muestra el texto de inmediato.</summary>
		public bool m_show;
		/// <summary>Latch</summary>
		private bool m_showLastState;

		/// <summary>Numero de lineas de texto a mostrar.</summary>
		public float m_textLines = 1f;

		/// <summary>Caracteres por segundo a mostrar en secuencia. 0 para mostrar todo de una sola vez.</summary>
		public float m_charsPerSecond = 0f;

		/// <summary>Activar la animacion del cursor parpadeando al final del texto.</summary>
		public bool m_showCursor = false;
		/// <summary>Latch</summary>
		private bool m_showCursorLastState;

		/// <summary>Texto a mostrar.</summary>
		private string m_finalText = "";

		/// <summary>Texto mostrado actualmente.</summary>
		private string m_actualText = "";

		/// <summary>El texto separado en caracteres.</summary>
		private char[] m_charArray;

		/// <summary>Bandera para evitar que dos corrutinas escriban al mismo texto simultaneamente.</summary>
		private bool m_textoOcupado = false;

		/// <summary>
		/// Muestra de inmediato el texto en la capa GUI.
		/// </summary>
		/// <value><c>true</c> if mostrar; otherwise, <c>false</c>.</value>
		public bool ForzarMostrar{get {return m_show;} set{m_show = value;} }

		void Update(){
			if(m_show != m_showLastState){
				m_showLastState = m_show;

				if(m_show) SetText (m_finalText);
			}
		}

		/// <summary>Muestra o no el texto segun la variabla <see cref="m_show"/>.</summary>
		void OnGUI(){
			if(m_show) Draw();
		}

		/// <summary>
		/// Al llamar este metodo desde algun metedo OnGUI se escribira el texto cargado.
		/// </summary>
		public void Draw(){

			//Cargar estilo
			GUI.skin = m_skin;
				
			//Ajustar tamaño de fuente
			if(Tools.IsTablet()){
				GUI.skin.label.fontSize = (int) (32f * ((float)(Screen.width) / 600f)); //basado en una pantalla de 600x1024
			}else{
				GUI.skin.label.fontSize = (int) (24f * ((float)(Screen.width) / 600f));
			}

			//establecer area del texto
			float areaHeight = Screen.height * .1f * m_textLines;

			//Desplegar texto
			GUILayout.BeginVertical();
				GUILayout.Space(Screen.height - areaHeight);
				GUILayout.Label (m_actualText, GUILayout.Width(Screen.width));
			GUILayout.EndVertical();
		}

		/// <summary>
		/// Devuelve el texto que actualmente se esta mostrando en pantalla, incluyendo el cursor.
		/// </summary>
		/// <returns>El texto.</returns>
		public string GetActualText(){
			return m_actualText;
		}

		/// <summary>
		/// Fija el texto a mostrar instantaneamente.
		/// </summary>
		/// <param name="text">Texto.</param>
		public void SetText(string text){
			StopAllCoroutines();
			m_finalText = text;
			m_actualText = "";

			if(m_charsPerSecond >0){
				m_charArray = text.ToCharArray(0, text.Length);

				if(m_showCursor){
					m_actualText += "_";
					StartCoroutine (StartCursorBlink());
				}

				StartCoroutine (WriteCharactersSequentially());
			}else{
				m_actualText = text;

				if(m_showCursor){
					m_actualText += "_";
					StartCoroutine (StartCursorBlink());
				}
			}//if-else
		}

		/// <summary>
		/// Agrega al mensaje a mostrar.
		/// </summary>
		/// <param name="text">Texto.</param>
		public void AppendText(string text){
			SetText(m_finalText + text);
		}

		/// <summary>
		/// Corrutina que agrega los caracteres del mensaje uno a uno al texto que se despliega en el GUI.
		/// </summary>
		private IEnumerator WriteCharactersSequentially(){
			foreach(char c in m_charArray){

				while(m_textoOcupado); //esperar a q se desocupe el texto
				m_textoOcupado = true;

				if(m_showCursor){
					string sub = m_actualText.Substring(0, m_actualText.Length-1);
					char end = m_actualText[m_actualText.Length-1];
					m_actualText = sub + c + end;
				}else{
					m_actualText += c;
				}

				m_textoOcupado = false;
				yield return new WaitForSeconds(1f/m_charsPerSecond);
			}
		}

		/// <summary>
		/// Inicia el parpadeo del cursor al final del texto mostrado.
		/// </summary>
		private IEnumerator StartCursorBlink(){

			while(m_showCursor){
				while(m_textoOcupado); //esperar a q se desocupe el texto
				m_textoOcupado = true;

				string sub = m_actualText.Substring(0, m_actualText.Length-1);

				if(m_actualText[m_actualText.Length-1] == '_'){
					m_actualText = sub + " ";
				}else{
					m_actualText = sub + "_";
				}

				m_textoOcupado = false;
				yield return new WaitForSeconds(.3f);
			}//while
		}

	}
}

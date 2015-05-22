using UnityEngine;
using System.Collections;
using System;

namespace Wink{

	[RequireComponent(typeof(Animator))]
	/// <summary>
	/// Clase para sincronizar las animaciones de todos los dispositivos a traves de un servidor.
	/// </summary>
	/// <remarks>
	/// Esta clase requiere un script PHP en un servidor que pueda guardar los datos enviados como parametro en el URL
	/// y devolverlos en orden como respuesta plain text.
	/// El formato de la respuesta viene especificado en <see cref="UpdateAnimationStateFromResponse(WWW)"/>.
	/// </remarks>
	public class SyncAnimation : MonoBehaviour{

		#region PUBLIC_VARIABLES
		/// <summary>Direccion del script php.</summary>
		public string m_serverURL = "http://www.bondzu.com/template/unity/timesync.php";
		
		/// <summary>Indica si esta instancia es el objeto master, es decir, el que sincroniza a todos los demás.</summary>
		public bool m_master = false;
		
		/// <summary>ID unico del Gameobjet a sincronizar.</summary>
		public int id = 1;

		/// <summary>Nombres de los triggers que usa la maquina de estados.</summary>
		public string[] m_triggerNames;
		
		/// <summary>Variable que controla la conexion/desconexion del servidor.</summary>
		public bool m_connect = false;
		
		/// <summary>Intervalo de tiempo para interactuar con el servidor.</summary>
		public float m_syncInterval = -1f;
		
		/// <summary>Segundo en el cual empezar a reproducir la animacion actual.</summary>
		public float m_setTime = 0;
		#endregion
		
		#region PRIVATE_VARIABLES
		/// <summary>Animation controller de este elemento.</summary>
		private Animator m_animator;
		
		/// <summary>Estados originales de los renderers.</summary>
		private bool[] m_savedRenderStates;
		
		/// <summary>Ultima fecha de actualizacion de la animacion en el servidor.</summary>
		private DateTime m_lastSnapDate;
		
		/// <summary>Indica si una transaccion ya se esta llevando a cabo.</summary>
		private bool m_waitingForResponse = false;

		/// <summary>Indica que hay una transaccion push pendiente.</summary>
		private bool m_pushPendiente = false;
		
		/// <summary>Instancia de conexion web.</summary>
		private WWW m_www;
		
		/// <summary>Latch para la variable connect.</summary>
		private bool m_latchConnect = false;
		
		/// <summary>Latch para m_setTime</summary>
		private float m_latchSetTime;

		/// <summary>Ultima transicion reproducida.</summary>
		private int m_lastTransitionHash;

		/// <summary>Ultimo estado reproducido.</summary>
		private int m_lastStateHash;

		/// <summary>ID de la transaccion</summary>
		private int m_trxID = 0;
		#endregion
		
		#region PROPERTIES
		/// <summary>Devuelve el componente de animacion del Gameobject asociado a este script.</summary>
		public Animator Animator{ get{return m_animator;} }
		#endregion


		#region PUBLIC_METHODS
		/// <summary>
		/// Conectarse al servidor y sincronizar animacion.
		/// </summary>
		public void Connect(){
			m_connect = true;
		}
		
		/// <summary>
		/// Desconectarse del servidor.
		/// </summary>
		public void Disconnect(){
			m_connect = false;
		}
		#endregion


		#region PRIVATE_METHODS
		void Start(){
			//Inicializar variables
			m_animator = gameObject.GetComponent<Animator>();
			m_savedRenderStates = GetRenderStates ();
			m_lastTransitionHash = 0;
			m_lastStateHash = 0;
		}
		
		void Update(){
			//Procesar operacion pendiente.
			UpdatePush ();

			//Monitorear cambio en m_connect
			if(m_latchConnect != m_connect){
				m_latchConnect = m_connect;
				if(m_connect){
					if(m_syncInterval > 0)
						StartCoroutine(BeginPolling(m_syncInterval));
					else
						SyncData();
				}
			}//if connect
			
			//Monitorear cambio en setTime
			if(m_latchSetTime != m_setTime){
				m_latchSetTime = m_setTime;
				
				//Saltar a tiempo de reproduction
				AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
				int hash = info.nameHash;
				m_animator.Play (hash,-1,m_setTime/info.length);
			}//if settime

			//Monitorear el cambio de estado y transiciones del master
			//Durante una transicion
			if(m_master){
				/*if(m_animator.IsInTransition(0) && 
				   m_lastTransitionHash != m_animator.GetAnimatorTransitionInfo(0).nameHash){

					m_lastTransitionHash = m_animator.GetAnimatorTransitionInfo(0).nameHash;

					Debug.Log (gameObject.name+": Transitioning..."+(!m_waitingForResponse ? " syncing." : "pushing."));
					PushSync();
				//Al terminar la transicion
				}else*/ if(m_lastStateHash != m_animator.GetCurrentAnimatorStateInfo(0).nameHash){
					m_lastStateHash = m_animator.GetCurrentAnimatorStateInfo(0).nameHash;
					Debug.Log (gameObject.name+": Ended transition."+(!m_waitingForResponse ? " Syncing." : " Pushing."));
					PushSync();
				}
			}
		}
		
		
		/// <summary>
		/// Rutina periodica para leer informacion desde el servidor.
		/// </summary>
		private IEnumerator BeginPolling(float waitTime){
			while(m_connect){
				if(!m_waitingForResponse){
					SyncData ();
					yield return new WaitForSeconds(waitTime);
				}else{
					yield return null;
				}
			}
		}
		
		
		/// <summary>
		/// Sincroniza los datos de animacion con el servidor.
		/// </summary>
		private void SyncData(){
			if(m_master) StartCoroutine (SendAnimationStateToServer());
			else StartCoroutine(GetAnimationStateFromServer());
		}


		/// <summary>Pone en cola una sincronizacion para cuando termine la proxima.</summary>
		private void PushSync(){
			m_pushPendiente = true;
		}

		/// <summary>Procesa la operacion push pendiente.</summary>
		private void UpdatePush(){
			if(!m_pushPendiente) return;

			if(!m_waitingForResponse){
				SyncData ();
				m_pushPendiente = false;
			}
		}
		
		/// <summary>
		/// Envia la informacion del objeto a la base de datos del servidor. 
		/// Si existe una operacion previa la cancela.
		/// </summary>
		private IEnumerator GetAnimationStateFromServer(){
			
			//Bandera de ocupado
			m_waitingForResponse = true;
			
			//Construir URL
			string url = m_serverURL + "?id=" + id;
			
			//Enviar peticion al servidor
			if(m_www != null) m_www.Dispose ();	//Cancelar la operacion previa
			WWW w = new WWW(url);
			yield return w;	//Salir y regresar cuando este listo el objeto w
			
			//Tomar accion sobre la respuesta
			StartCoroutine(UpdateAnimationStateFromResponse(w));
		}
		
		/// <summary>
		/// Envia la informacion del objeto a la base de datos del servidor.
		/// Si existe una operacion previa la cancela.
		/// </summary>
		private IEnumerator SendAnimationStateToServer(){

			//Indicar transaccion pendiente
			m_waitingForResponse = true;

			//informacion del estado actual en reproduccion
			AnimatorStateInfo info = m_animator.GetCurrentAnimatorStateInfo(0);
			int db_hash = info.nameHash;
			float db_normalizedTime = info.normalizedTime;

			//Determinar si hay transicion
			int db_nextStateHash = 0;
			if(m_animator.IsInTransition(0)){
				AnimatorStateInfo next = m_animator.GetNextAnimatorStateInfo(0);
				db_nextStateHash = next.nameHash;
				//AnimatorTransitionInfo trInfo = m_animator.GetAnimatorTransitionInfo (0);
				//db_triggerHash = trInfo.nameHash;
			}
			
			//Obtener fecha en la que empezo la animacion
			DateTime snapDateTime = DateTime.UtcNow;
			int db_dateMilliseconds = snapDateTime.Millisecond;
			
			//Construir URL
			string url = m_serverURL;
			url += "?id=" + id;
			url += "&object_name=" + WWW.EscapeURL(gameObject.name);
			url += "&pos_x=" + transform.position.x;
			url += "&pos_y=" + transform.position.y;
			url += "&pos_z=" + transform.position.z;
			url += "&rot_x=" + transform.eulerAngles.x;
			url += "&rot_y=" + transform.eulerAngles.y;
			url += "&rot_z=" + transform.eulerAngles.z;
			url += "&current_state_hash=" + db_hash;
			url += "&normalized_time=" + db_normalizedTime;
			url += "&snap_date=" + WWW.EscapeURL(snapDateTime.ToString ("s"));
			url += "&date_milliseconds=" + db_dateMilliseconds;
			url += "&trigger_hash=" + db_nextStateHash;
			url += "&ismaster=true";
			
			//Enviar peticion al servidor
			if(m_www != null) m_www.Dispose ();	//Cancelar la operacion previa
			m_www = new WWW(url);
			yield return m_www;	//Salir y regresar cuando este listo el objeto w
			
			string[] parametros = m_www.text.Split(',');
			
			//Comprobar primero si la operacion fue exitosa
			string status = parametros[0];
			if(status != "ok"){
				Debug.LogWarning(gameObject.name+": "+(m_trxID++)+" Error while connecting\n"+m_www.text);
			}else{
				Debug.Log (gameObject.name+": "+(m_trxID++)+" Estado enviado. "+db_hash+", "+db_normalizedTime+
				           " -> "+db_nextStateHash);
			}//if-else
			
			//Indicar proceso terminado
			m_waitingForResponse = false;
		}
		
		
		/// <summary>
		/// Envia la informacion al servidor para retirar este objeto de la base de datos y su informacion 
		/// de animacion y posicion. Si este dispositivo es el ultimo en desconectarse, la informacion de animacion en la
		/// base de datos se borra; de lo contrario, solo resta 1 a la cuenta de dispositivos conectados.
		/// </summary>
		/// <returns>The from server.</returns>
		private IEnumerator DisconnectFromServer(){
			//construir URL
			string url = m_serverURL;
			url += "?id=" + id;
			url += "&disconnect=true";
			
			//Enviar peticion al servidor
			WWW w = new WWW(url);
			yield return w;//Salir y regresar cuando este listo el objeto w
			
			string[] parametros = w.text.Split(',');
			
			//Comprobar primero si la operacion fue exitosa
			string status = parametros[0];
			if(status != "ok"){
				Debug.LogWarning(gameObject.name+": Error on disconnection\n"+w.text);
			}//if-else
		}//disconnect()
		
		/// <summary>
		/// Recibe la respuesta de texto generada por el script php del servidor y toma accion.
		/// </summary>
		/// <remarks>
		/// El script php del servidor genera un archivo de texto con el siguiente formato separado por comas:
		/// <code>
		/// echo("ok,$pos_x,$pos_y,$pos_z,$rot_x,$rot_y,$rot_z,$current_state_hash,$normalized_time,$snap_date,$date_milliseconds");
		/// </code>
		/// Si la transacicion fue exitosa el primer dato sera "ok". De lo contrario "error" y sera el fin de la cadena.
		/// <code>echo("error");</code>
		/// El segund dato indica si es necesario que se descargue y actualice la informacion de animacion. Se leera 
		/// "true" o "false" de acuerdo al caso.El resto de los parametros necesarios para reconstruir la animacion.
		/// </remarks>
		/// <param name="response">Respuesta del servidor en formato de texto simple.</param>
		private IEnumerator UpdateAnimationStateFromResponse(WWW response){
			string texto = response.text;
			string[] parametros = texto.Split(',');
			
			//Comprobar primero si la operacion fue exitosa
			string status = parametros[0];
			if(status == "ok"){
				
				//Extraer los parametros
				float posX = float.Parse(parametros[1]);
				float posY = float.Parse(parametros[2]);
				float posZ = float.Parse(parametros[3]);
				float rotX = float.Parse(parametros[4]);
				float rotY = float.Parse(parametros[5]);
				float rotZ = float.Parse(parametros[6]);
				int db_hash = int.Parse(parametros[7]);
				float db_normalizedTime = float.Parse(parametros[8]);
				DateTime db_snapDateTime = DateTime.Parse (parametros[9]);
				double db_dateMilliseconds = double.Parse(parametros[10]);
				int db_nextStateHash = int.Parse(parametros[11]);
				
				//Comprobar si se requiere actualizar el estado de la animacion
				bool actualizar = db_snapDateTime != m_lastSnapDate;
				
				if(actualizar){
					
					//Comprobar si hubo cambio
					m_lastSnapDate = db_snapDateTime;

					//Obtener datos del estado actual
					int currentState = m_animator.GetCurrentAnimatorStateInfo(0).nameHash;
					float stateLength = m_animator.GetCurrentAnimatorStateInfo(0).length;

					//Transformar la fecha de inicio en segundos
					db_snapDateTime = db_snapDateTime.AddMilliseconds (db_dateMilliseconds);	//Agregar milisegundos a la fecha original
					TimeSpan snapSpan = DateTime.UtcNow - db_snapDateTime;						//Calcular diferencia de fechas
					float deltaSeconds = Convert.ToSingle(snapSpan.TotalSeconds);				//Traducir a segundos
					
					//Aplicar cambios de posicion y orientacion
					Vector3 db_newPos = new Vector3(posX, posY, posZ);
					Vector3 db_newAngles = new Vector3(rotX, rotY, rotZ);
					transform.position = db_newPos;
					transform.eulerAngles = db_newAngles;
					
					//Aplicar nuevo tiempo a la animacion
					m_animator.enabled = enabled;
					bool play = false;
					//Si no hay transicion
					if(db_nextStateHash == 0){
						m_animator.Play(db_hash,-1,db_normalizedTime);
						play = true;

						//Esperar a que surtan efecto los cambios de Animator.Play()
						yield return null;
						yield return null;

					//Si la transicion es a un estado diferente del actual
					}else if(db_nextStateHash != currentState){
						//m_animator.Play(db_hash,-1,db_normalizedTime);
						m_animator.CrossFade(db_hash, .5f/stateLength);
						play = true;

						//Esperar a que surtan efecto los cambios de Animator.Play()
						yield return null;
						yield return null;
					}

					//Activa transicion
					bool crossfade = false;
					if(db_nextStateHash != 0 && db_nextStateHash != currentState){
						m_animator.CrossFade (db_nextStateHash, .5f/stateLength);	//Duracion 0.5s
						crossfade = true;

						//Esperar los cambios de Animator.CrossFade()
						yield return null;
						yield return null;
					}

					//Aplicar tiempo
					m_animator.Update (deltaSeconds);

					Debug.Log (gameObject.name+": "+(m_trxID++)+" Estado actualizado. "+db_hash+", "+db_normalizedTime+
					           " -> "+db_nextStateHash+"\n"+(play?"Play(), ":"")+(crossfade?"CrossFade(), ":""));
				}//if actualizar
				
			}else{	//error en la respuesta
				Debug.LogWarning((m_trxID++)+" error:\n"+response.text);
			}//if ok
			
			//Indicar terminado
			m_waitingForResponse = false;

		}//HandleResponse()
		
		/// <summary>
		/// Desactiva todos los Renderers.
		/// </summary>
		private void UnrenderChildren(){
			Renderer[] childrenRenderers = gameObject.GetComponentsInChildren<Renderer>();
			for(int i = 0; i < childrenRenderers.Length; i++){
				childrenRenderers[i].enabled = false;
			}
		}		
		
		/// <summary>
		/// Devuelve los Renderers al estado guardado.
		/// </summary>
		private void RenderChildren(){
			Renderer[] childrenRenderers = gameObject.GetComponentsInChildren<Renderer>();
			for(int i = 0; i < childrenRenderers.Length; i++){
				childrenRenderers[i].enabled = m_savedRenderStates[i];
			}
		}		
		
		/// <summary>
		/// Devuelve el estado de todos los renderers.
		/// </summary>
		/// <returns>Un arreglo de los estados de los renderers.</returns>
		private bool[] GetRenderStates(){
			Renderer[] childrenRenderers = gameObject.GetComponentsInChildren<Renderer>();
			bool[] renderStates = new bool[childrenRenderers.Length];
			
			for(int i = 0; i < childrenRenderers.Length; i++){
				renderStates[i] = childrenRenderers[i].enabled;
			}
			
			return renderStates;
		}
		#endregion

	}
}
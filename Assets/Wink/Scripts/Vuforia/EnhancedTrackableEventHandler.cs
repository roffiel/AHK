/******************************************************************
 * 28-09-14 Raschid
 * - Creado
 * 
 * ****************************************************************/


using UnityEngine;

namespace Wink{
	/// <summary>
	/// Esta clase monitorea el estado Lost/Found de un tracker.
	/// </summary>
	/// <remarks>Esta clase se debe adjuntar a un objeto que contenga el script <see cref="TrackableBehaviour"/>.</remarks>
	public class EnhancedTrackableEventHandler : MonoBehaviour,
	ITrackableEventHandler
	{
		/// <summary>Variable para forzar el estado de Lost/Found.</summary>
		public bool m_found = false;

		/// <summary>Aplicar el efecto usando el componente FadeBehaviour</summary>
		public bool m_applyFadingEffect = false;

		#region EVENTOS
		/// <summary>Declaracion del delegado para al encontrar el target.</summary>
		public delegate void OnTargetFound(EnhancedTrackableEventHandler sender);
		/// <summary>Evento al encontrar el target.</summary>
		public OnTargetFound onTargetFound;

		/// <summary>Declaracion del delegado para al perder el target</summary>
		public delegate void OnTargetLost(EnhancedTrackableEventHandler sender);
		/// <summary>Evento al perder el target.</summary>
		public OnTargetLost onTargetLost;

		/// <summary>Declaracion del delegado para al terminar el effecto fade in.</summary>
		public delegate void OnFadedInFinish(EnhancedTrackableEventHandler sender);
		/// <summary>Evento al terminar el efecto fade in.</summary>
		public OnFadedInFinish onFadeInFinish;

		/// <summary>Declaracion del delegado para al terminar el effecto fade out.</summary>
		public delegate void OnFadedOut(EnhancedTrackableEventHandler sender);
		/// <summary>Evento al terminar el efecto fade out.</summary>
		public OnFadedOut onFadeOutFinish;
		#endregion

		#region PRIVATE_MEMBER_VARIABLES
		/// <summary>Clase de Vuforia con todas las propiedades para trackear un target.</summary>
		private TrackableBehaviour mTrackableBehaviour;

		/// <summary>Latch para el estado Lost/Found.</summary>
		private bool m_foundLastState;

		/// <summary>Latch para la variable m_applyFadeingEffect.</summary>
		private bool m_afeLastState;

		/// <summary>Componente para hacer el efecto de fading.</summary>
		private FadeBehaviour m_fadeBehaviour;

		#endregion // PRIVATE_MEMBER_VARIABLES

		#region PROPERTIES
		/// <summary>
		/// Devuelve el nombre unico del target.
		/// </summary>
		/// <value>The name of the trackable.</value>
		public string TrackableName{ get { return mTrackableBehaviour.TrackableName; } }
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Wink.EnhancedTrackableEventHandler"/>
		///  apply fading effect.
		/// </summary>
		/// <remarks>Esta propiedad se basa en la clase <see cref="Wink.FadeBehaviour"/></remarks>
		/// <value><c>true</c> if apply fading effect; otherwise, <c>false</c>.</value>
		public bool ApplyFadingEffect{ 
			set { 
				m_applyFadingEffect = value;

				//Aplicar Fade Effect
				if(m_applyFadingEffect && m_fadeBehaviour == null){
					m_fadeBehaviour = gameObject.AddComponent<FadeBehaviour> ();
					m_fadeBehaviour.m_appear = m_found;
					m_fadeBehaviour.OnFadeOutEffectFinished += HandleFadeInFinished;
					m_fadeBehaviour.OnFadeOutEffectFinished += HandleFadeOutFinished;
				}

				//Quitar Fade Effect
				if(!m_applyFadingEffect && m_fadeBehaviour != null){
					m_fadeBehaviour.OnFadeInEffectFinished -= HandleFadeInFinished;
					m_fadeBehaviour.OnFadeOutEffectFinished -= HandleFadeOutFinished;
					GameObject.Destroy(m_fadeBehaviour);
				}
			}
		}
		#endregion
		
		
		#region UNTIY_MONOBEHAVIOUR_METHODS
		
		public void Start()
		{
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();

			ApplyFadingEffect = m_applyFadingEffect;
			RegisterEventos();
			Init ();
		}

		public void Update(){

			//Monitoriear la variable publica para forzar el estado Lost/Found
			if (m_found != m_foundLastState){

				//guardar estado
				m_foundLastState = m_found;

				//aplicar cambios
				if(m_found) WhenTargetFound();
				else WhenTargetLost();
			}

			if(m_applyFadingEffect != m_afeLastState){

				//guardar estado
				m_afeLastState = m_applyFadingEffect;

				//aplicar cambios
				ApplyFadingEffect = m_applyFadingEffect;

			}
		}

		
		#endregion // UNTIY_MONOBEHAVIOUR_METHODS		
		
		#region PUBLIC_METHODS

		/// <summary>
		/// Inserte aqui el contenido de la inicializacion. Este metodo sera ejecutado 
		/// inmediatamente despues del metodo Start() de la clase base <see cref=".WinkEnhancedTrackableEventHandler"/>.
		/// </summary>
		public virtual void Init(){}


		/// <summary>
		/// Registra los eventos del <see cref="TrackableBehaviour"/>. 
		/// </summary>
		public void RegisterEventos(){
			if (mTrackableBehaviour)
			{
				mTrackableBehaviour.RegisterTrackableEventHandler(this);
			}
		}

		/// <summary>
		/// Implementation of the ITrackableEventHandler function called when the
		/// tracking state changes.
		/// </summary>
		public void OnTrackableStateChanged(
			TrackableBehaviour.Status previousStatus,
			TrackableBehaviour.Status newStatus)
		{
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
			    newStatus == TrackableBehaviour.Status.TRACKED ||
			    newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				WhenTargetFound();
			}
			else
			{
				WhenTargetLost();
			}
		}
		
		#endregion // PUBLIC_METHODS
		
		
		
		#region PRIVATE_METHODS
		/// <summary>
		/// Registra el cambio de estado Lost/Found.
		/// </summary>
		private void WhenTargetFound()
		{

			Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
			m_found = true; //registrar estado

			//Lanzar evento de tracker found
			if(onTargetFound != null) onTargetFound(this);

			OnTrackingFound ();
		}
		
		/// <summary>
		/// Registra el cambio de estado Lost/Found.
		/// </summary>
		private void WhenTargetLost()
		{

			Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
			m_found = false; //registrar estado

			//Lanzar evento de tracker lost
			if(onTargetLost != null) onTargetLost(this);

			OnTrackingLost ();
		}

		/// <summary>
		/// Handles the fade in finished: Emite el evento OnFadeIn.
		/// </summary>
		public void HandleFadeInFinished(){
			if(onFadeInFinish != null) onFadeInFinish(this);
		}

		/// <summary>
		/// Handles the fade out finished: Desactiva todos los renderers y emite el evento OnFadeOut
		/// </summary>
		public void HandleFadeOutFinished(){
			UnrenderChildren ();
			if(onFadeOutFinish != null) onFadeOutFinish(this);
		}

		#endregion

		#region VIRTUAL_METHODS
		/// <summary>
		/// Acciones a realizar al encotrar el target. Por defecto, renderea todos los objetos hijos.
		/// </summary>
		public virtual void OnTrackingFound(){

			if(m_fadeBehaviour == null){
				RenderChildren ();
			}else{
				m_fadeBehaviour.m_appear = true;
			}

		}//OnTrackingFound()


		/// <summary>
		/// Acciones a realizar al perder el target. Por defecto, desrenderea todos los objetos hijos.
		/// </summary>
		public virtual void OnTrackingLost(){

			if(m_fadeBehaviour == null){
				UnrenderChildren ();
			}else{
				m_fadeBehaviour.m_appear = false;
			}
		}//trackinglost()

		#endregion

		#region PUBLIC_METHODS
		/// <summary>
		/// Muestra todos sus objetos activando los <see cref="MeshRenderer"/> y <see cref="Collider"/>.
		///  Si esta activada la opcion ApplyFadingEffect, aplicara el efecto.
		/// </summary>
		public void RenderChildren(){
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

			// Enable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = true;
			}
			
			// Enable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = true;
			}
		}
		
		
		/// <summary>
		/// Deja de mostrar todos sus objetos hijos desactivando los <see cref="MeshRenderer"/> y <see cref="Collider"/>.
		///  Si esta activada la opcion ApplyFadingEffect, aplicara el efecto.
		/// </summary>
		public void UnrenderChildren(){
			Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
			Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);
			
			// Disable rendering:
			foreach (Renderer component in rendererComponents)
			{
				component.enabled = false;
			}
			
			// Disable colliders:
			foreach (Collider component in colliderComponents)
			{
				component.enabled = false;
			}
		}
		#endregion
	}
}

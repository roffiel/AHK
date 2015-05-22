using UnityEngine;
using System.Collections;

namespace Wink{
	/// <summary>
	/// Esta clase agrega a un objeto un efecto de aparecer o desaparecer gradualmente cambiando su alpha.
	/// </summary>
	/// <remarks>Este componente se debe ligar al objeto que se desee tenga el efecto. A traves de la variable
	/// publica booleana <see cref="FadeBehaviour.m_FadeIn"/> se hace aparecer o desaparecer. Solo se modifica
	/// el alpha del material, no se afecta (activando o desactivando) el <see cref="MeshRenderer"/>.</remarks>
	public class FadeBehaviour : MonoBehaviour {

		#region PUBLIC_VARIABLES
		/// <summary>Trigger para comenzar la animacion de aparecer/desaparecer</summary>
		public bool m_appear = true;

		/// <summary>Duracion de la transicion</summary>
		public float m_transitionTime = 1f;
		
		/// <summary>Objetos hijos a manipular</summary>
		public FadingObjet[] m_objetosHijos;
		
		/// <summary>The transition has ended.</summary>
		public System.Action OnFadeInEffectFinished, OnFadeOutEffectFinished;
		#endregion

		#region PRIVATE_VARIABLES
		/// <summary>Curvas de animacion</summary>
		private AnimationCurve m_fadeInCurve, m_fadeOutCurve;

		/// <summary>Guarda el tiempo en el que se inicia la animacion.</summary>
		private float m_setTime = 0;
		
		/// <summary>Variable para hacer un latch y activar la animacion</summary>
		private bool m_lastState, m_eventSet = false;
		#endregion

		#region PRIVATE_METHODS
		void Start () {

			//guardar primer estado del latch
			m_lastState = m_appear;

			//Obtener e inicializar los hijos
			MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer> ();
			m_objetosHijos = new FadingObjet[renderers.Length];
			for(int i = 0; i < m_objetosHijos.Length; i++){
				m_objetosHijos[i] = new FadingObjet(renderers[i]);
			}

			//Preparar las curvas Fadein y Fadeout
			m_fadeInCurve = new AnimationCurve (new Keyframe[]{
				new Keyframe(0,0,0,0),
				new Keyframe(m_transitionTime,1f,0,0)
			});

			m_fadeOutCurve = new AnimationCurve (new Keyframe[]{
				new Keyframe(0,1f,0,0),
				new Keyframe(m_transitionTime,0,0,0)
			});

			m_fadeOutCurve.postWrapMode = WrapMode.ClampForever;
			m_fadeInCurve.postWrapMode = WrapMode.ClampForever;
		}
		
		// Update is called once per frame
		void Update () {

			//Registrar cuando cambia el estado
			if(m_lastState != m_appear){
				m_setTime = Time.time;
				m_eventSet = false;
			}

			//Aplicar transiciones
			float a;
			if(m_appear){
				a = m_fadeInCurve.Evaluate(Time.time - m_setTime);	

				//Levantar el evento cuando haya terminado la transicion
				if(Time.time - m_setTime >= m_transitionTime && !m_eventSet){
					if(OnFadeInEffectFinished != null) OnFadeInEffectFinished();
					m_eventSet = true;
				}
			}else{
				a = m_fadeOutCurve.Evaluate(Time.time - m_setTime);	

				//Levantar el evento cuando haya terminado la transicion
				if(Time.time - m_setTime >= m_transitionTime && !m_eventSet){
					if(OnFadeOutEffectFinished != null)	OnFadeOutEffectFinished();
					m_eventSet = true;
				}
			}

			//Aplicar a todos los hijos
			foreach(FadingObjet f in m_objetosHijos){
				f.RelativeAlpha = a;
			}

			//Guardar estado
			m_lastState = m_appear;
		}
		#endregion


		///<summary>
		/// Esta clase se utiliza para manipular el alpha de un objeto MeshRenderer
		/// </summary>
		public class FadingObjet{

			#region FADINGOBJECT_VARIABLES
			/// <summary>Shader original</summary>
			private Shader m_originalShader;

			/// <summary>Renderer a manipular</summary>
			private MeshRenderer m_meshRenderer;

			/// <summary>Valor orignial de alpha al crear la clase.</summary>
			private float m_originalAlpha;

			/// <summary>Este sera el valor maximo de alpha que se aplicara a esta clase</summary>
			private float m_maxAlpha;
			#endregion

			#region FADINGOBJECT_PROPIEDADES
			/// <summary>
			/// Gets the original alpha.
			/// </summary>
			/// <value>The original alpha.</value>
			public float OriginalAlpha{ get { return m_originalAlpha; } }

			/// <summary>
			/// Gets or sets the max alpha.
			/// </summary>
			/// <value>The max alpha.</value>
			public float MaxAlpha{ get { return m_maxAlpha; } set { m_maxAlpha = value; } }

			/// <summary>
			/// Aplica o devuelve el valor de alpha relativo al original.
			/// </summary>
			/// <value>The alpha.</value>
			public float RelativeAlpha{
				get {
					return m_meshRenderer.material.HasProperty("_Color") ? m_meshRenderer.material.color.a / m_maxAlpha : 1f;
				} 

				set{
					//Si el objeto esta activo
					if(m_meshRenderer != null){

						//Aplicar nuevo valor de alpha
						if(m_meshRenderer.material.HasProperty("_Color")){
							Color c = m_meshRenderer.material.color;
							c.a = value * m_maxAlpha; //No exceder el alpha original
							m_meshRenderer.material.color = c;

							//Desactivar el meshrenderer en caso de que alfa = 0
							if(c.a > 0) m_meshRenderer.enabled = true;
							else m_meshRenderer.enabled = false;
						}

						//Devolver al shader original cuando el alpha sea 100% del alpha original
						if(value >= m_maxAlpha) m_meshRenderer.material.shader = m_originalShader;
						else m_meshRenderer.material.shader = Shader.Find ("Transparent/Diffuse");
					}
				}
			}

			/// <summary>
			/// Devuelve el alpha real del objeto.
			/// </summary>
			/// <value>The absolute alpha.</value>
			public float AbsoluteAlpha{
				get {
					return m_meshRenderer.material.color.a;
				}
			}
			#endregion

			#region FADINGOBJECT_CONSTRUCTOR
			/// <summary>
			/// Initializes a new instance of the <see cref="Fade+FadingObjet"/> class.
			/// </summary>
			/// <param name="mr"><see cref="MeshRenderer"/> a quien manipular la transparencia.</param>
			public FadingObjet(MeshRenderer mr){
				m_meshRenderer = mr;
				m_originalShader = mr.material.shader;
				m_originalAlpha = mr.material.HasProperty("_Color") ? mr.material.color.a : 1f;
				m_maxAlpha = m_originalAlpha;
			}

			/// <summary>
			/// Inicializa una instancia de la clase <see cref="Fade+FadingObjet"/> y 
			/// aplica un valor inicial a alpha.
			/// </summary>
			/// <param name="meshRenderer">Mesh renderer.</param>
			/// <param name="alphaInicial">Alpha inicial.</param>
			public FadingObjet(MeshRenderer mr, float alphaInicial){
				m_meshRenderer = mr;
				m_originalShader = mr.material.shader;
				m_originalAlpha = mr.material.color.a;
				m_maxAlpha = m_originalAlpha;

				//Aplicar alpha inicial
				Color c = mr.material.color;
				c.a = alphaInicial;
				mr.material.color = c;
			}
			#endregion
		}
	}
}

# Herramientas para Vuforia
El prefab *WinkTarget* (en la carpeta [Prefabs](../../Prefabs)) proporciona funciones adicionales que faciliten el desarrollo de la aplicación.

## Características
El objeto incorpora las siguientes opciones en el Editor:

* `Found`: Simula el comportamiento encontrado/perdido sin tener que mostrar el target verdadero a la webcam.
* `ApplyFadingEffect`: Si se activa, hace que los objetos de realidad aumentada (*augmentation*) aparezcan y desaparezcan gradualmente, es decir, con un efecto *fade in/out*. Actualmente no funciona con todos los materiales.

### Scripting
Este objeto utiliza el script `EnhancedTrackableEventHandler : MonoBehaviour,ITrackableEventHandler` e implementa eventos para facilitar las tareas de programación:

* `public OnTargetFound onTargetFound` - Evento al encontrar el target.
* `public OnTargetLost onTargetLost` - Evento al perder el target.
* `public OnFadedInFinish onFadeInFinish` - Evento al terminar el efecto fade in.
* `public OnFadedOut onFadeOutFinish` - Evento al terminar el efecto fade out.


## Requerimientos
* [Vuforia](https://developer.vuforia.com/resources/sdk/unity?page=node%252F1810).

## Instrucciones de Uso
Arrastrar el prefab WinkTarget a la escena y utilizarlo como cualquier ImageTarget de Vuforia.


## Contenido

```
Wink
|---+Prefabs
|	|	WinkTarget.prefab
|
+---+Scripts
	|	EnhancedTrackableEventHandler.cs
	|	FadeBehaviour.cs
```

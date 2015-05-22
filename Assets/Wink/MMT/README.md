# VideoTarget

## Descripción
El Prefab VideoTarget proporciona una manera fácil de proyectar animaciones 2D en la superficie de un target para dar la ilusión de que la imagen toma vida.

## Requerimientos
* [MobileMovieTexture](https://www.assetstore.unity3d.com/en/#!/content/2449).
* [Vuforia](https://developer.vuforia.com/resources/sdk/unity?page=node%252F1810).


## Modo de uso
Para cada Target de video: 

1. Arrastrar el prefab [`VideoTarget`](/Prefabs) a la escena.
2. Asignar el target correspondiente en su componente `Image Target Behaviour` y ajustar la escala del objeto hijo `VideoPlane` para que coincidan en dimensión ambos planos.
3. Crear un material nuevo y asignarlo al objeto `VideoPlane`. Seleccionar un shader adecuado dentro de la categoría `Color Space` de acuerdo a las especificaciones del complemento MobileMovieTexture.
4. Modificar los parámetros del componente `Mobile Movie Texture` del objeto `VideoTarget`:
  1. `Path`: elegir un video de formato `.ogv` u `.ogg` que se encuentre en la carpeta `StreamingAssets`.
  2. `MovieMaterials`: Agregar el material recientemente creado.
  3. `Loop Count`: El número de veces que se repetirá el video. `-1` para infinito. Activar `Loop` en el audio también si es el caso.
5. En el componente `Audio Source` se puede asignar un audio de formato `.mp3`. 
6. Configurar los parámetros del componente `Video Target` según se requiera.

### Componente Video Target

* `Found`: Envía la señal de que el target fue detectado y causa el mismo efecto en el simulador que al mostrar el target en la cámara.
* `Apply Fading Effect`: Cuando está activado, causará que la realidad aumentada aparezca gradualmente al detectarse el target. No funciona con todos los materiales.
* `Video Manager`: Opcionalmente se puede asignar un objeto único en la escena de tipo [`VideoManager`](/MMT) para coordinar la reproducción de un único video a la vez. De todos los videos que tengan el mismo `VideoManager` asignado, sólo se reproducira uno a la vez y el resto se liberará de la memoria.
* `Pause`: Permite pausar la reproducción de video y audio durante la ejecución.
* `On Lost`: Define el comportamiento al perder el audio.

| Valor | Efecto|
| ----- | ----- |
| Stop  | La reproduccion se detiene y no se muestra. Comienza desde el inicio al detectar el target de nuevo. |
| Pause | La reproduccion se pausa y no se muestra. Se reaunda al detectar el target de nuevo. |
| ContinueAudio | La reproduccion no se muestra pero continua y se escucha el audio. |
| ContinueMute | La reproduccion no se muestra pero continua sin audio hasta detectar  el target de nuevo. |

* `Loop`: Repite la reproduccion si el valor es verdadero.
# Descripción

* [GyroCamBehaviour.cs](#GyroCamBehaviour) - Script que se agrega a una camara para ser controlada por giroscopio.
* [LookAtCameraBehaviour.cs](#LookAtCameraBehaviour) - Comportamiento para hacer un objeto plano girar de frente hacia la camara sobre su eje Y. Por ahora solo funciona cuando este objeto se encuentra dentro de un padre cuyo eje Z es perpendicular al eje Y de este objeto.
* [PointNorthBehaviour.cs](#PointNorthBehaviour) - Este script orienta un objeto hacia el norte magnetico en el eje Z.


# Modo de Uso


## <a name="GyroCamBehaviour"></a>GyroCamBehaviour.cs
### Instrucciones
Arrastrar el script sobre la cámara que se desea controlar.

### Opciones en el  editor

* `Enable` - activa o desactiva este comportamiento.


## <a name="LookAtCameraBehaviour"></a>LookAtCameraBehaviour.cs
### Instrucciones
Arrastrar el script sobre al objeto que se desea que siempre mire hacia la cámara.

### Opciones en el  editor

* `Enable` - Activa o desactiva este comportamiento.
* `RotX` - Ajusta el ángulo inicial en X.
* `RotY` - Ajusta el ángulo inicial en Y.
* `RotZ` - Ajusta el ángulo inicial en Z.

## <a name="PointNorthBehaviour"></a>PointNorthBehaviour.cs
### Instrucciones
1. Agregar primero el script de comportamiento [GyroCamBehaviour.cs](#GyroCamBehaviour) a la cámara.
2. Arrastrar el script PointNorthBehaviour.cs sobre al objeto que se desea que apunte siempre al norte magnético.
3. Seleccionar en el parámetro `GyroCam` la cámara que contiene el script GyroCamBehaviour.
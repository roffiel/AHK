# SyncAnimation
Clase para sincronizar las animaciones de Gameobjets en diferentes dispositivos a través de un servidor.

## <a name="options"></a>Opciones en el Editor
* `ServerURL`- Direcciónn del [script](#script) servidor en internet que controla la sincronización.
* `Master` - Indica si esta instancia es el objeto *maestro* o es un *esclavo*. El *maestro* es el que sincroniza a todos los demás
objetos. Los objetos *esclavos* imitan el comportamiento del *maestro*. Active para configurar como *maestro*.
* `ID` - ID unico del Gameobjet a sincronizar.
* `TriggerNames` - Nombres de los triggers de la máquina de estados del Gameobjet a sincronizar (si es que los tiene).
* `Connect` - Controla la conexion/desconexion del servidor.
* `SyncInterval` - Tiempo en segundos para estar actualizando datos desde servidor. Si se deja en -1 la sincronización 
	se hará sólo una vez y no se repetirá.
* `SetTime` - Permite llevar la animación del personaje al segundo seleccionado cuando se está ejecutando la escena (*PlayMode*).

## <a name="script"></a>El Script PHP: timesync.php
Antes de poder sincronizar los objetos en varios dispositivos, el administrador web debe crear una base de datos y asegurarse
de que exista una copia del script timesync.php o similar en el servidor. En esta carpeta se proporciona una copia del script comentado. 
El script debe generar un archivo de texto con la información de posición y animación del objeto separada por comas (sin espacios entre comas) 
en el siguiente formato:

```
echo("ok,$pos_x,$pos_y,$pos_z,$rot_x,$rot_y,$rot_z,$current_state_hash,$normalized_time,$snap_date,$date_milliseconds");
```

Si la transacicion fue exitosa el primer dato sera "ok". De lo contrario "error" y sera el fin de la cadena.

```
echo("error");
```


## Instrucciones de Uso

1. Asegurarse de que la base de datos y el script php se han puesto online.
2. Agregar el script *SyncAnimation.cs* sobre el objeto a sincronizar en la escena de Unity.
3. Configurar los parámetros en el Editor.
4. Asegurarse de que un dispositivo ejecute la escena con el objeto en modo [*master*](#options) y los demás en modo *esclavo*. Para esto se recomienda
crear dos aplicaciones, una maestra y una esclava; o desarrollar un mecanismo mediante scripts para que sólo el primer objeto en conectarse al
servidor se vuelva maestro.


## Contenido

```
Wink
+--+Scripts
	+--+Sync
		|	SyncAnimation.cs
		|	timesync.php
```
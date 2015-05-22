# Wink-Toolbox

## Contenido

```
 Wink
|   MMT - Contiene scripts para implementar VideoTargets: animaciones 2D de AR que se reproducen sobre el target.
|   Packages - Contiene los paquetes de Unity para sólo importar.
|   Prefabs - Algunos Prefabs de objetos útiles comúnmente usados.
+--+Scripts
   |   Orientate - Scripts relacionados con orientación de objetos y dispositivos.
   |   Scync - Scripts para sincronizar animaciones a través de varios dispositivos.
   |   UI - Scripts relacionados con el despliegue de la interfaz de usuario.
   |   Vuforia - Scripts complementarios de los componentes de vuforia.
```

## Prefabs
Se incluyen los seguientes prefabs:

* **Luces Direccionales**: Un objeto de 4 luces para obtener fácilmente una iluminación pareja en toda la escena.
* **Video Target**: Prefab para obtener fácilmente targets con animaciones 2D. [Más información](/MMT).
* **Wink Target**: Un objeto basado en el prefab de Vuforia con funcionalidades adicionales. [Más información](/Scripts/Vuforia).


## Scripts
Se incluyen las siguientes carpetas y scripts con clases que proporcionan funcionalidades útiles para construir aplicaciones:

* [Scripts/Sync](/Scripts/Sync) - Clases para sincronizar las animaciones de todos los dispositivos a traves de un servidor.
* [Scripts/Orientate](/Scripts/Orientate) - Scripts de comporamiento (*Behaviours*) para orientar objetos y escenas.
* [Scripts/UI](/Scripts/UI) - Scripts para desplegar objetos de GUI.
* [Scripts/Vuforia](/Scripts/Vuforia) - Scrips necesarios para el funcionamiento del prefab [WinkTarget](/Scripts/Vuforia).

## Instrucciones
Par utilizar las herramientas bassa sólo descargar e importar el paquete [Wink-Toolbox 1.1.unitypackage](/Packages/Wink-Toolbox 1.1.unitypackage?raw=true).
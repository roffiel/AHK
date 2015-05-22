using UnityEngine;
using System.Collections;

namespace Wink{

	/// <summary>
	/// Algunas herramientas utiles para determinar los atributos de la pantalla.
	/// </summary>
	public static class Tools{
	
		public enum ScreenRatios{_3x2, _4x3, _16x9}

		/// <summary>
		/// Gets the screen diagonal.
		/// </summary>
		/// <returns>The screen diagonal in inches.</returns>
		public static float GetScreenDiagonal(){
			float dpi = Mathf.Sqrt(Screen.dpi);
			if (dpi == 0) return 0;
			
			float w = ((float)Screen.width) / dpi;
			float h = ((float)Screen.height) / dpi;
			return (Mathf.Sqrt(w*w + h*h));
		}
		
		/// <summary>
		/// Gets the screen ratio.
		/// </summary>
		/// <returns>The screen ratio.</returns>
		public static float GetScreenRatio(){
			return (((float)Screen.width) / ((float)Screen.height));
		}
		
		
		/// <summary>
		/// Gets the more aproximate horizontal screen ratio: 3:2, 4:3 or 16:9
		/// </summary>
		/// <returns>The aproximate screen ratio.</returns>
		public static ScreenRatios GetMostAproximateScreenRatio(){
			
			float sr = GetScreenRatio();
			float d3 = Mathf.Abs(sr - (3f/2f));
			float d4 = Mathf.Abs(sr - (4f/3f));
			float d16 = Mathf.Abs(sr - (16f/9f));
			
			float min = Mathf.Min(Mathf.Min(d3,d4),d16);
			
			ScreenRatios appRatio = ScreenRatios._16x9;
			if(min == d3) appRatio = ScreenRatios._3x2;
			else if(min == d4) appRatio = ScreenRatios._4x3;
			else if(min == d16) appRatio = ScreenRatios._16x9;
			
			return appRatio;
		}

		/// <summary>
		/// Determina que el dispositivo es una table cuando su pantalla es mayor a 6".
		/// </summary>
		/// <returns><c>true</c> if this instance is a tablet; otherwise, <c>false</c>.</returns>
		public static bool IsTablet() {
			float inches = GetScreenDiagonal();
			if(inches == 0) return false;
			else return (inches >= 6f);
		}
	}
}

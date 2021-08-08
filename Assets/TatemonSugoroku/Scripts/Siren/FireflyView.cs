using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 蛍の描画クラス
	/// </summary>
	public class FireflyView : SMStandardMonoBehaviour {
		void Start() {
			var day = FindObjectOfType<DayView>();
			day._state.Subscribe( state => {
				switch ( state ) {
					case DayState.Night:
						gameObject.SetActive( true );
						break;
				}
			} );

			gameObject.SetActive( false );
		}
	}
}
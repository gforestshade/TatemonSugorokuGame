using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 背景の描画クラス
	/// </summary>
	public class BackgroundView : SMStandardMonoBehaviour {
		readonly Dictionary<DayState, GameObject> _images = new Dictionary<DayState, GameObject>();

		[SerializeField] Color _nightColor = new Color( 0.2f, 0.2f, 0.2f );



		void Start() {
			var rs = GetComponentsInChildren<SpriteRenderer>( true );
			rs.ForEach( r => {
				var s = r.gameObject.name.ToEnum<DayState>();
				_images[s] = r.gameObject;
			} );

			var day = FindObjectOfType<DayView>();
//			day._sunsetRate.Subscribe( r => SetBrightness( r ) );
//			SetBrightness( day._sunsetRate.Value );

			day._state.Subscribe( state => {
				_images.ForEach( pair => pair.Value.SetActive( false ) );
				_images[state].SetActive( true );
			} );
		}


/*
		void SetBrightness( float brightness ) {
			var c = Color.Lerp( _nightColor, Color.white, brightness );
			_renderers.ForEach( r => r.material.color = c );
		}
*/


/*
		/// <summary>
		/// ● 画像を変更
		/// </summary>
		public void ChangeImage() {
			_showIndex = ( _showIndex + 1 ) % _renderers.Length;
			_renderers.ForEach( r => r.gameObject.SetActive( false ) );
			_renderers[_showIndex].gameObject.SetActive( true );
		}
*/
	}
}
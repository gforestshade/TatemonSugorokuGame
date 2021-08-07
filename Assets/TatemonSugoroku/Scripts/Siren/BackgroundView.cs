using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 背景の描画クラス
	/// </summary>
	public class BackgroundView : SMStandardMonoBehaviour {
		DayView _day			{ get; set; }
		int _showIndex				{ get; set; }
		SpriteRenderer[] _renderers	{ get; set; }

		[SerializeField] Color _nightColor = new Color( 0.2f, 0.2f, 0.2f );



		void Start() {
			_day = FindObjectOfType<DayView>();

			_renderers = GetComponentsInChildren<SpriteRenderer>( true );

			_day._sunsetRate.Subscribe( r => SetBrightness( r ) );

			SetBrightness( _day._sunsetRate.Value );
		}



		void SetBrightness( float brightness ) {
			var c = Color.Lerp( _nightColor, Color.white, brightness );
			_renderers.ForEach( r => r.material.color = c );
		}



		/// <summary>
		/// ● 画像を変更
		/// </summary>
		public void ChangeImage() {
			_showIndex = ( _showIndex + 1 ) % _renderers.Length;
			_renderers.ForEach( r => r.gameObject.SetActive( false ) );
			_renderers[_showIndex].gameObject.SetActive( true );
		}
	}
}
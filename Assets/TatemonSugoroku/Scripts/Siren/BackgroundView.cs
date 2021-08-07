using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 背景の描画クラス
	/// </summary>
	public class BackgroundView : SMStandardMonoBehaviour {
		DayModel _dayModel			{ get; set; }
		int _showIndex				{ get; set; }
		SpriteRenderer[] _renderers	{ get; set; }

		[SerializeField] Color _nightColor = new Color( 0.2f, 0.2f, 0.2f );



		protected override void StartAfterInitialize() {
			_dayModel = AllModelManager.s_instance.Get<DayModel>();

			_renderers = GetComponentsInChildren<SpriteRenderer>( true );

			_dayModel._sunsetRate.Subscribe( r => SetBrightness( r ) );

			SetBrightness( _dayModel._sunsetRate.Value );
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
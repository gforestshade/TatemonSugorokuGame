#define TestBackground
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
		[SerializeField] Color _nightColor = new Color( 0.2f, 0.2f, 0.2f );

		DayModel _dayModel { get; set; }
		int _showIndex { get; set; }
		SpriteRenderer[] _renderers { get; set; }



		protected override void StartAfterInitialize() {
			_renderers = GetComponentsInChildren<SpriteRenderer>( true );

			_dayModel = SMServiceLocator.Resolve<AllModelManager>().Get<DayModel>();
			_dayModel._sunsetRate.Subscribe( r => SetBrightness( r ) );
			SetBrightness( _dayModel._sunsetRate.Value );

#if TestBackground
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
				_showIndex = ( _showIndex + 1 ) % _renderers.Length;
				_renderers.ForEach( r => r.gameObject.SetActive( false ) );
				_renderers[_showIndex].gameObject.SetActive( true );
			} );
#endif
		}



		void SetBrightness( float brightness ) {
			var c = Color.Lerp( _nightColor, Color.white, brightness );
			_renderers.ForEach( r => r.material.color = c );
		}
	}
}
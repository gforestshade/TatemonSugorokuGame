using System.Linq;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タップの描画クラス
	/// </summary>
	public class TapView : SMStandardMonoBehaviour {



		protected override void StartAfterInitialize() {
			gameObject.DontDestroyOnLoad();

			var effects = GetComponentsInChildren<ParticleSystem>();
			var effectTop = effects.First().transform;

			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
				var camera = Camera.main;
				if ( camera == null ) { return; }

				var mouse = inputManager.GetAxis( SMInputAxis.Mouse );
				var mousePosition = new Vector3( mouse.x, mouse.y, 1 );
				var position = camera.ScreenToWorldPoint( mousePosition );

				effectTop.position = position;
				effects.ForEach( e => e.Play() );
			} );
		}
	}
}
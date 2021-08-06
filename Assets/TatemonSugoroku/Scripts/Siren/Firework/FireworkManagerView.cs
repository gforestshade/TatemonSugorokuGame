//#define TestFirework
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火の描画クラス
	/// </summary>
	public class FireworkManagerView : SMStandardMonoBehaviour {
		FireworkManagerModel _model { get; set; }
		readonly List<FireworkView> _fireworks = new List<FireworkView>();

		[SerializeField] GameObject _prefab;

		bool _isActive { get; set; }



		protected override void StartAfterInitialize() {
			_model = AllModelManager.s_instance.Get<FireworkManagerModel>();

			FireworkManagerModel.MAX_COUNT.Times( i => {
				var go = _prefab.Instantiate( transform );
				var f = go.GetComponent<FireworkView>();
				f.Setup( this );
				_fireworks.Add( f );
			} );

			_model._launch.Subscribe( _ => {
				SetActive( true );
			} );

#if TestFirework
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				SetActive( !_isActive );
			} );
#endif
		}



		void SetActive( bool isActive ) {
			if ( _isActive == isActive ) { return; }
			_isActive = isActive;

			_fireworks.ForEach( f => f.SetActive( _isActive ) );
		}
	}
}
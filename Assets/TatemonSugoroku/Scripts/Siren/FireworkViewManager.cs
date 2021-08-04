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



/// <summary>
/// ■ 花火の描画クラス
/// </summary>
public class FireworkViewManager : SMStandardMonoBehaviour {
	const int MAX_COUNT = 5;
	const int LAUNCH_HOUR = 19;

	DayModel _dayModel { get; set; }
	readonly List<FireworkView> _fireworks = new List<FireworkView>();
	bool _isActive { get; set; }



	protected override void StartAfterInitialize() {
		var prefab = Resources.Load<GameObject>( "Prefabs/Firework" );
		MAX_COUNT.Times( i => {
			var go = prefab.Instantiate( transform );
			var f = go.GetComponent<FireworkView>();
			f.Setup( this );
			_fireworks.Add( f );
		} );

		_dayModel = SMServiceLocator.Resolve<AllModelManager>().Get<DayModel>();
		_dayModel._hour
			.Select( h => {
				SMLog.Debug( $"ゲーム内時刻 : {h}" );
				return h;
			} )
			.Where( h => h >= LAUNCH_HOUR )
			.Subscribe( h => {
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
		if ( _isActive == isActive )	{ return; }
		_isActive = isActive;

		_fireworks.ForEach( f => f.SetActive( _isActive ) );
	}
}
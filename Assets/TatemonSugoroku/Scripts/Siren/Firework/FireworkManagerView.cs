using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火の描画クラス
	/// </summary>
	public class FireworkManagerView : SMStandardMonoBehaviour {
		/// <summary>花火の最大数</summary>
		public const int MAX_COUNT = 5;
		/// <summary>花火の打ち上げ時刻</summary>
		public const int LAUNCH_HOUR = 19;

		[SerializeField] GameObject _prefab;
		readonly List<FireworkView> _fireworks = new List<FireworkView>();
		bool _isActive { get; set; }

		public readonly Subject<Unit> _launchEvent = new Subject<Unit>();



		void Start() {
			MAX_COUNT.Times( i => {
				var go = _prefab.Instantiate( transform );
				var f = go.GetComponent<FireworkView>();
				f.Setup( this );
				_fireworks.Add( f );
			} );

			var day = FindObjectOfType<DayView>();
			day._hour
				.Where( h => h >= LAUNCH_HOUR )
				.Subscribe( h => Launch() );
		}



		void SetActive( bool isActive ) {
			if ( _isActive == isActive ) { return; }
			_isActive = isActive;

			_fireworks.ForEach( f => f.SetActive( _isActive ) );
		}

		void Launch() {
			SetActive( true );
			_launchEvent.OnNext( Unit.Default );
		}
	}
}
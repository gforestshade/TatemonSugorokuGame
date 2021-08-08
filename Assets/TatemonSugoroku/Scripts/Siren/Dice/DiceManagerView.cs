using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ サイコロ管理の描画クラス
	/// </summary>
	public class DiceManagerView : SMStandardMonoBehaviour {
		public static readonly int MAX_COUNT = 2;

		
		Vector3 _power { get; set; }

		[SerializeField] GameObject _prefab;
		readonly List<DiceView> _views = new List<DiceView>();
		UIDiceView _uiView { get; set; }
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		public int _total { get; private set; }

		public readonly Subject<int> _totalEvent = new Subject<int>();
		public readonly ReactiveProperty<DiceState> _state = new ReactiveProperty<DiceState>();



		void Start() {
			_uiView = FindObjectOfType<UIDiceView>();

			MAX_COUNT.Times( i => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<DiceView>();
				v.Setup( i );
				_views.Add( v );
			} );

			_disposables.AddFirst( () => {
				_totalEvent.Dispose();
				_canceler.Dispose();
			} );
		}



		public List<int> GetValues()
			=> _views
				.Select( v => v._value )
				.ToList();



		public void SetPower( Vector3 power = default ) {
			_power = power;
			_views.ForEach( v => v._power = power );
		}



		public async UniTask ChangeState( DiceState state ) {
			_canceler.Cancel();

			_state.Value = state;

			//await _uiView.ChangeState( state );
			await _views.Select( v => v.ChangeState( state ) );

			if ( state == DiceState.Roll ) {
				_total = _views.Sum( v => v._value );
				_totalEvent.OnNext( _total );
			}
		}

		public async UniTask<int> Roll() {
			await UTask.DelayFrame( _canceler, 1 );

			await ChangeState( DiceState.Rotate );
			await ChangeState( DiceState.Roll );
			UTask.Void( async () => {
				await UTask.Delay( _canceler, 5000 );
				await ChangeState( DiceState.Hide );
			} );
			return _total;
		}
	}
}
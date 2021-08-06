using System.Linq;
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
		DiceModel _model { get; set; }

		[SerializeField] GameObject _prefab;
		readonly List<DiceView> _views = new List<DiceView>();

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		protected override void StartAfterInitialize() {
			_model = AllModelManager.s_instance.Get<DiceModel>();

			DiceModel.MAX_COUNT.Times( i => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<DiceView>();
				v.Setup( i );
				_views.Add( v );
			} );


			_model._state.Subscribe( s => {
				_canceler.Cancel();
				_views.ForEach( v => v.ChangeState( s ).Forget() );

				if ( s != DiceState.Roll )	{ return; }

				UTask.Void( async () => {
					await UTask.WaitWhile( _canceler, () => _views.Any( v => v._value == -1 ) );
					_model.SetValues( _views.Select( v => v._value ).ToArray() );
				} );
			} );

			_model._power.Subscribe( p => {
				_views.ForEach( v => v._power = p );
			} );


			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );
		}
	}
}
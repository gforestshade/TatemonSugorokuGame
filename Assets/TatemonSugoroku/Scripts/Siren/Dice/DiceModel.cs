using System.Linq;
using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ サイコロのモデルクラス
	/// </summary>
	public class DiceModel : SMStandardBase, IModel {
		public const int MAX_COUNT = 2;

		public int[] _values = new int[MAX_COUNT];

		public readonly ReactiveProperty<DiceState> _state = new ReactiveProperty<DiceState>();
		public readonly Subject<Vector3> _power = new Subject<Vector3>();
		public readonly Subject<int> _total = new Subject<int>();



		public DiceModel() {
			_disposables.AddLast( () => {
				_state.Dispose();
				_total.Dispose();
			} );
		}



		public int GetValue( int id )
			=> _values[id];

		public void SetValues( int[] values ) {
			_values = values;
			_total.OnNext( _values.Sum() );
		}

		public void ChangeState( DiceState state )
			=> _state.Value = state;
	}
}
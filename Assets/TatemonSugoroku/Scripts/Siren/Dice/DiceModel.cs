using System.Linq;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ サイコロのモデルクラス
	/// </summary>
	///========================================================================================================
	public class DiceModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>サイコロの最大数</summary>
		public static readonly int MAX_COUNT = EnumUtils.GetLength<PlayerType>();

		/// <summary>各サイコロの出目</summary>
		public int[] _values = new int[MAX_COUNT];

		/// <summary>サイコロの状態（非表示、回転待機、転がす）</summary>
		public readonly ReactiveProperty<DiceState> _state = new ReactiveProperty<DiceState>();
		/// <summary>サイコロを投げる力</summary>
		public readonly Subject<Vector3> _power = new Subject<Vector3>();
		/// <summary>サイコロ停止時の、出目の合計の通知</summary>
		public readonly Subject<int> _total = new Subject<int>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public DiceModel() {
			_disposables.AddLast( () => {
				_state.Dispose();
				_total.Dispose();
			} );
		}

		public void Initialize( AllModelManager manager ) {
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 取得、設定
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● サイコロ番号の、出目を取得
		/// </summary>
		public int GetValue( int id )
			=> _values[id];

		/// <summary>
		/// ● サイコロ番号の、出目を設定
		///		3D物理サイコロが、この関数を呼ぶ。
		/// </summary>
		public void SetValues( int[] values ) {
			_values = values;
			_total.OnNext( _values.Sum() );
		}

		/// <summary>
		/// ● サイコロを投げる力を設定
		///		サイコロボタンのUIが、この関数を呼ぶ。
		/// </summary>
		public void SetPower( Vector3 power )
			=> _power.OnNext( power );

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 状態遷移
		///		非表示、回転待機、転がす、に遷移させたい時、ご利用下さい。
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public void ChangeState( DiceState state )
			=> _state.Value = state;
	}
}
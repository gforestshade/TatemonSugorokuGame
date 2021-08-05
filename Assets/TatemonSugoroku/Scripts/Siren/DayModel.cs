using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ 日時のモデルクラス
	///		ターン数ではなく、1日の時間で、環境を遷移させます。
	///		太陽光、花火、たてもん回転など、様々な物に影響を与えます。
	/// </summary>
	///========================================================================================================
	public class DayModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>1プレイヤーが行える、最大手番</summary>
		const int MAX_PLAYER_TURN = 7;
		/// <summary>全プレイヤーを総合した、最大手番</summary>
		const int MAX_ALL_TURN = MAX_PLAYER_TURN * 2;
		/// <summary>ゲーム開始時の、ゲーム内時刻</summary>
		const int FIRST_HOUR = 12;
		/// <summary>ゲーム終了時の、ゲーム内時刻</summary>
		const int END_HOUR = 21;

		/// <summary>日没の開始時刻</summary>
		public const int FIRST_SUNSET_HOUR = 18;
		/// <summary>完全に日没する時刻</summary>
		public const int END_SUNSET_HOUR = 20;
		/// <summary>日没に要する時間</summary>
		public const int SUNSET_HOUR = END_SUNSET_HOUR - FIRST_SUNSET_HOUR;

		/// <summary>時間が進む速度</summary>
		float _hourVeloctiy { get; set; }

		/// <summary>現在時刻</summary>
		public readonly ReactiveProperty<float> _hour = new ReactiveProperty<float>();
		/// <summary>日の強さの割合</summary>
		public readonly ReactiveProperty<float> _sunsetRate = new ReactiveProperty<float>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public DayModel() {
			_hour.Value = FIRST_HOUR;
			var delta = END_HOUR - FIRST_HOUR;
			_hourVeloctiy = ( float )delta / MAX_PLAYER_TURN;
			_sunsetRate.Value = 1;
		}

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 時間を更新
		///		時間を手番分進めます。
		///		全プレイヤーの手番終了後、呼んで下さい。
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public void UpdateHour() {
			_hour.Value = Mathf.Clamp( _hour.Value + _hourVeloctiy, FIRST_HOUR, END_HOUR );

			var delta = END_SUNSET_HOUR - _hour.Value;
			_sunsetRate.Value = Mathf.Clamp01( delta / SUNSET_HOUR );
		}
	}
}
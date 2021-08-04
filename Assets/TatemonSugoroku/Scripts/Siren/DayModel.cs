#define TestDay
using UnityEngine;
using UniRx;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;



/// <summary>
/// ■ 日時のモデルクラス
/// </summary>
public class DayModel : SMStandardBase, IModel {
	const int MAX_PLAYER_TURN = 7;
	const int MAX_ALL_TURN = MAX_PLAYER_TURN * 2;
	const int FIRST_HOUR = 12;
	const int END_HOUR = 21;

	public const int FIRST_SUNSET_HOUR = 18;
	public const int END_SUNSET_HOUR = 20;
	public const int SUNSET_HOUR = END_SUNSET_HOUR - FIRST_SUNSET_HOUR;

	public readonly ReactiveProperty<float> _hour = new ReactiveProperty<float>();
	public readonly ReactiveProperty<float> _sunsetRate = new ReactiveProperty<float>();
	float _hourVeloctiy	{ get; set; }



	public DayModel() {
		_hour.Value = FIRST_HOUR;
		var delta = END_HOUR - FIRST_HOUR;
		_hourVeloctiy = ( float )delta / MAX_PLAYER_TURN;
		_sunsetRate.Value = 1;

#if TestDay
		UTask.Void( async () => {
			var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
			await framework.WaitInitialize();

			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				UpdateHour();
			} );
		} );
#endif
	}



	public void UpdateHour() {
		_hour.Value = Mathf.Clamp( _hour.Value + _hourVeloctiy, FIRST_HOUR, END_HOUR );

		var delta = END_SUNSET_HOUR - _hour.Value;
		_sunsetRate.Value = Mathf.Clamp01( delta / SUNSET_HOUR );
	}
}
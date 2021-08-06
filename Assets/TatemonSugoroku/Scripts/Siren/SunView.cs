//#define TestSun
using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 太陽の描画クラス
	/// </summary>
	public class SunView : SMStandardMonoBehaviour {
		const float ANGLE_TO_HOUR = 360f / 24;
		const float ANGLE_OFFSET = -90 - ANGLE_TO_HOUR * 2;

		[SerializeField] Color _sunColor;
		[SerializeField] Color _nightColor;

		Light _light;
		Vector3 _firstAngles;



		protected override void StartAfterInitialize() {
			_light = GetComponent<Light>();
			_firstAngles = transform.eulerAngles;

			var _dayModel = AllModelManager.s_instance.Get<DayModel>();
			_dayModel._hour.Subscribe( h => {
				SetAngle( h );
			} );
			SetAngle( _dayModel._hour.Value );
		}



		void SetAngle( float hour ) {
			var angle = hour * ANGLE_TO_HOUR + ANGLE_OFFSET;
			_firstAngles.x = angle;
			transform.eulerAngles = _firstAngles;

			var delta = Mathf.Abs( angle - 90 );
			var rate = Mathf.Clamp01( delta / 90 );
			_light.color = Color.Lerp( _sunColor, _nightColor, rate );
		}
	}
}
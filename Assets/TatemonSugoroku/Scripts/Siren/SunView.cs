//#define TestSun
using UnityEngine;
using UnityEngine.Rendering;
using Cysharp.Threading.Tasks;
using UniRx;
using DG.Tweening;
using SubmarineMirage.Base;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 太陽の描画クラス
	/// </summary>
	public class SunView : SMStandardMonoBehaviour {
		const float ANGLE_TO_HOUR = 360f / 24;
		const float ANGLE_OFFSET = -90 - ANGLE_TO_HOUR * 2;

		[SerializeField] Color _sunColor;
		[SerializeField] Color _eveningColor;
		[SerializeField] Color _nightColor;

		Light _light;
		Vector3 _firstAngles;

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		void Start() {
			_light = GetComponent<Light>();
			_firstAngles = transform.eulerAngles;

			var day = FindObjectOfType<DayView>();
			day._state.Subscribe( state => ChangeLight( state ).Forget() );
		}



		async UniTask ChangeLight( DayState state ) {
			var target = Color.clear;
			switch ( state ) {
				case DayState.Sun:		target = _sunColor;		break;
				case DayState.Evening:	target = _eveningColor;	break;
				case DayState.Night:	target = _nightColor;	break;
			}

			await DOTween.To(
				() => RenderSettings.ambientLight,
				c => {
					RenderSettings.ambientMode = AmbientMode.Flat;
					RenderSettings.ambientLight = c;
				},
				target,
				3
			)
			.SetEase( Ease.InQuart )
			.Play()
			.ToUniTask( _canceler );
		}
	}
}
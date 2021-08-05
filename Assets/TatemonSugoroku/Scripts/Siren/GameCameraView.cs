using UnityEngine;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ ゲームシーンのカメラの描画クラス
	/// </summary>
	public class GameCameraView : SMStandardMonoBehaviour {
		[SerializeField] Vector3 _offset = new Vector3( 0, 1, 0 );
		Vector3 _lookAtPosition;



		protected override void StartAfterInitialize() {
		}



		protected override void UpdateAfterInitialize() {
			transform.LookAt( _lookAtPosition + _offset );
		}



		public void LookAt( Vector3 position )
			=> _lookAtPosition = position;
	}
}
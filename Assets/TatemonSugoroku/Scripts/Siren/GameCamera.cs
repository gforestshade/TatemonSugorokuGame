using UnityEngine;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;



/// <summary>
/// ■ ゲームシーンのカメラクラス
/// </summary>
public class GameCamera : SMStandardMonoBehaviour {
	[SerializeField] Vector3 _offset = new Vector3( 0, 1, 0 );
	Vector3 _lookAtPosition;



	void Start() {
	}



	void Update() {
		transform.LookAt( _lookAtPosition + _offset );
	}



	public void LookAt( Vector3 position )
		=> _lookAtPosition = position;
}
//#define TestMoveArrow
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;



/// <summary>
/// ■ 移動カーソルの描画クラス
/// </summary>
public class MoveArrowView : SMStandardMonoBehaviour {
	public enum State {
		None,
		Down,
		Left,
		Right,
		Up,
	}


	[SerializeField] Vector3 _offset = new Vector3( 0, 0.6f, 0 );

	TileView _tile { get; set; }
	MeshRenderer[] _renderers { get; set; }

	State _state { get; set; }
	readonly Dictionary<State, Quaternion> _stateToRotation = new Dictionary<State, Quaternion>() {
		{ State.None,	Quaternion.identity },
		{ State.Down,	Quaternion.Euler(0, 270, 0) },
		{ State.Left,	Quaternion.Euler(0, 0, 0) },
		{ State.Right,	Quaternion.Euler(0, 180, 0) },
		{ State.Up,		Quaternion.Euler(0, 90, 0) },
	};



	void Start() {
	}

	protected override void StartAfterInitialize() {
	}

	public void Setup( TileView tile ) {
		_tile = tile;
#if TestMoveArrow
		_offset.y += 0.3f;
		_stateToRotation.Keys.ToArray().ForEach( key => {
			var rotation = _stateToRotation[key];
			rotation *= Quaternion.Euler( 90, 0, 0 );
			_stateToRotation[key] = rotation;
		} );
#endif
		transform.position = _tile.transform.position + _offset;

		_renderers = GetComponentsInChildren<MeshRenderer>();

		_state = State.Down;
		SetState( State.None );
	}



	protected override void UpdateAfterInitialize() {
		var color = _tile._manager._arrowColor;
		_renderers.ForEach( r => r.material.color = color );
	}



	public void SetState( State state ) {
		if ( _state == state )	{ return; }
		_state = state;

		transform.rotation = _stateToRotation[_state];

		gameObject.SetActive( _state != State.None );
	}
}
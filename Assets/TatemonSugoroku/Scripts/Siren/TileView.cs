using System.Collections.Generic;
using UnityEngine;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;



/// <summary>
/// ■ タイルの描画クラス
/// </summary>
public class TileView : SMStandardMonoBehaviour {
	public enum ColorType {
		None,
		Player1,
		Player2,
	}

	public static readonly Vector3 TILE_TO_METER = Vector3.one;
	public static readonly Vector3 FIRST_POSITION_OFFSET = TILE_TO_METER / 2;

	public TileViewManager _manager	{ get; private set; }
	MeshRenderer _renderer		{ get; set; }

	public int _tileID				{ get; private set; }
	public Vector2Int _tilePosition	{ get; private set; }

	public ColorType _colorType { get; private set; }
	readonly Dictionary<ColorType, Color> _colors = new Dictionary<ColorType, Color> {
		{ ColorType.None,		Color.white },
		{ ColorType.Player1,	Color.blue },
		{ ColorType.Player2,	Color.red },
	};

	MoveArrowView _moveArrow { get; set; }



	void Start() {
	}

	protected override void StartAfterInitialize() {
	}

	public void Setup( TileViewManager manager, int tileID ) {
		_manager = manager;

		_tileID = tileID;
		_tilePosition = _manager.IDToTilePosition( tileID );

		gameObject.name = $"Tile {tileID} ( {_tilePosition.x}, {_tilePosition.y} )";

		transform.position = _manager.TileToMeter( _tilePosition );

		_renderer = GetComponent<MeshRenderer>();
		_moveArrow = GetComponentInChildren<MoveArrowView>( true );
		_moveArrow.Setup( this );
	}



	public Vector3 GetPosition()
		=> transform.position;

	public void SetColor( ColorType type ) {
		if ( _colorType == type )	{ return; }

		_colorType = type;
		_renderer.material.color = _colors[_colorType];
	}

	public void SetArrowState( MoveArrowView.State state )
		=> _moveArrow.SetState( state );
}
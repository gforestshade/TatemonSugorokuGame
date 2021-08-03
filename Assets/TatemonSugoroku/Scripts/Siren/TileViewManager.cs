#define TestTile
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;



/// <summary>
/// ■ タイル描画の管理クラス
/// </summary>
public class TileViewManager : SMStandardMonoBehaviour {
	public static readonly Vector2Int MAX_TILE_COUNT = new Vector2Int( 8, 8 );
	public static readonly int MAX_TILE_ID = MAX_TILE_COUNT.x * MAX_TILE_COUNT.y;

	public static readonly Vector2Int FIRST_TILE_POSITION = new Vector2Int( -4, -4 );
	public static readonly Vector3 FIRST_POSITION =
		new Vector3( FIRST_TILE_POSITION.x, 0, FIRST_TILE_POSITION.y ).Mult( TileView.TILE_TO_METER ) +
		TileView.FIRST_POSITION_OFFSET;

	public const int NONE_TILE_ID = -1;

	readonly List<TileView> _tiles = new List<TileView>();



	protected override void StartAfterInitialize() {
		var prefab = Resources.Load<GameObject>( "Prefabs/Tile" );

		Enumerable.Range( 0, MAX_TILE_ID ).ForEach( i => {
			var go = prefab.Instantiate( transform );
			var tile = go.GetComponent<TileView>();
			tile.Setup( this, i );
			_tiles.Add( tile );
		} );

#if TestTile
		var inputManager = SMServiceLocator.Resolve<SMInputManager>();
		inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
			var tile = GetTile( 13 );
			var i = ( ( int )tile._colorType + 1 ) % EnumUtils.GetLength<TileView.ColorType>();
			tile.SetColor( ( TileView.ColorType )i );
		} );
		inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
			var tile = GetTile( new Vector2Int( 5, 1 ) );
			var i = ( ( int )tile._colorType + 1 ) % EnumUtils.GetLength<TileView.ColorType>();
			tile.SetColor( ( TileView.ColorType )i );
		} );
		inputManager._touchTileID.Subscribe( id => {
			SMLog.Debug( id );
		} );

		var color = TileView.ColorType.None;
		inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
			var i = ( ( int )color + 1 ) % EnumUtils.GetLength<TileView.ColorType>();
			color = ( TileView.ColorType )i;
		} );
		inputManager._touchTileID
			.Where( id => id != -1 )
			.Subscribe( id => {
				var tile = GetTile( id );
				tile.SetColor( color );
			} );
#endif
	}



	public TileView GetTile( int tileID )
		=> _tiles[tileID];

	public TileView GetTile( Vector2Int tilePosition ) {
		var id = TilePositionToID( tilePosition );
		return _tiles[id];
	}



	public Vector2Int IDToTilePosition( int tileID )
		=> new Vector2Int(
			tileID % 8,
			tileID / 8
		);

	public int TilePositionToID( Vector2Int tilePosition )
		=> (
			tilePosition.x +
			tilePosition.y * 8
		);

	public Vector3 TileToMeter( Vector2Int tilePosition ) {
		var realPosition = new Vector3( tilePosition.x, 0, tilePosition.y ).Mult( TileView.TILE_TO_METER );
		var position = FIRST_POSITION + realPosition;
		return position;
	}
}
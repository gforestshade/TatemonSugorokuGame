using UnityEngine;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイルの描画クラス
	/// </summary>
	public class TileView : SMStandardMonoBehaviour {
		MeshRenderer _renderer { get; set; }

		/// <summary>タイル番号</summary>
		public int _tileID { get; private set; }
		/// <summary>タイル位置</summary>
		public Vector2Int _tilePosition { get; private set; }
		/// <summary>プレイヤー占領の領域タイプ</summary>
		public TileAreaType _areaType { get; private set; }



		public void Setup( int tileID ) {
			_renderer = GetComponent<MeshRenderer>();

			_tileID = tileID;
			_tilePosition = TileManagerView.ToTilePosition( tileID );
			transform.position = TileManagerView.ToRealPosition( _tilePosition );
			transform.localScale = TileManagerView.REAL_SCALE;
			gameObject.name = $"Tile {_tileID} ( {_tilePosition.x}, {_tilePosition.y} )";

			_areaType = TileAreaType.Player1;
			ChangeArea( TileAreaType.None );
		}



		public void ChangeArea( TileAreaType areaType ) {
			if ( _areaType == areaType )	{ return; }
			_renderer.material.color = TileManagerView.AREA_TYPE_TO_COLOR[areaType];
		}
	}
}
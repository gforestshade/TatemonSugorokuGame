using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイルのモデルクラス
	/// </summary>
	public class TileModel : SMStandardBase, IModel {
		public string _name				{ get; private set; }
		public int _tileID				{ get; private set; }
		public Vector2Int _tilePosition	{ get; private set; }
		public Vector3 _realPosition	{ get; private set; }
		public Vector3 _realScale		{ get; private set; }

		public readonly ReactiveProperty<TileAreaType> _areaType = new ReactiveProperty<TileAreaType>();



		public TileModel( int tileID ) {
			_tileID = tileID;
			_tilePosition = TileManagerModel.ToTilePosition( tileID );
			_realPosition = TileManagerModel.ToRealPosition( _tilePosition );
			_realScale = TileManagerModel.REAL_SCALE;

			_name = $"Tile {_tileID} ( {_tilePosition.x}, {_tilePosition.y} )";

			_disposables.AddFirst( () => {
				_areaType.Dispose();
			} );
		}
	}
}
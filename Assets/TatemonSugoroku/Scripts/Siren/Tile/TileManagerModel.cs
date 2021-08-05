using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイル管理のモデルクラス
	/// </summary>
	public class TileManagerModel : SMStandardBase, IModel {
		public static readonly Vector2Int MIN_SIZE = new Vector2Int( 0, 0 );
		public static readonly Vector2Int MAX_SIZE = new Vector2Int( 8, 8 );
		public static readonly int MAX_ID = MAX_SIZE.x * MAX_SIZE.y;

		public static readonly float TILE_SCALE_TO_REAL_SCALE = 1;
		public static readonly Vector3 REAL_SCALE = Vector3.one * TILE_SCALE_TO_REAL_SCALE;

		public static readonly Vector3 FIRST_REAL_POSITION =
			(
				new Vector3( -MAX_SIZE.x, 0, MAX_SIZE.y )
				+ new Vector3( 1, 1, -1 )
			)
			/ 2
			* TILE_SCALE_TO_REAL_SCALE;

		public const int NONE_ID = -1;


		public readonly List<TileModel> _models;



		public TileManagerModel() {
			_models = Enumerable.Range( 0, MAX_ID )
				.Select( i => new TileModel( i ) )
				.ToList();

			_disposables.AddFirst( () => {
				_models.ForEach( m => m.Dispose() );
			} );
		}



		public TileModel GetModel( int tileID )
			=> _models[tileID];

		public TileModel GetModel( Vector2Int tilePosition ) {
			var id = ToID( tilePosition );
			return _models[id];
		}



		public static Vector2Int ToTilePosition( int tileID )
			=> new Vector2Int(
				tileID % 8,
				tileID / 8
			);

		public static int ToID( Vector2Int tilePosition )
			=> (
				tilePosition.x +
				tilePosition.y * 8
			);

		public static Vector3 ToRealPosition( Vector2Int tilePosition )
			=> FIRST_REAL_POSITION
				+ (
					new Vector3( tilePosition.x, 0, -tilePosition.y )
					* TILE_SCALE_TO_REAL_SCALE
				);

		public static Vector3 ToRealDirection( Vector2Int tilePosition )
			=> new Vector3( tilePosition.x, 0, -tilePosition.y )
				* TILE_SCALE_TO_REAL_SCALE;
	}
}
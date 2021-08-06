using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ タイル管理のモデルクラス
	/// </summary>
	///========================================================================================================
	public class TileManagerModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>最小のタイル位置</summary>
		public static readonly Vector2Int MIN_SIZE = new Vector2Int( 0, 0 );
		/// <summary>最大のタイル位置</summary>
		public static readonly Vector2Int MAX_SIZE = new Vector2Int( 8, 8 );
		/// <summary>最大のタイル番号</summary>
		public static readonly int MAX_ID = MAX_SIZE.x * MAX_SIZE.y;

		/// <summary>タイルの大きさと、実際の大きさの比率</summary>
		public static readonly float TILE_SCALE_TO_REAL_SCALE = 1;
		/// <summary>1タイルの、実際の大きさ</summary>
		public static readonly Vector3 REAL_SCALE = Vector3.one * TILE_SCALE_TO_REAL_SCALE;

		/// <summary>タイルの実際の開始位置</summary>
		public static readonly Vector3 FIRST_REAL_POSITION =
			(
				new Vector3( -MAX_SIZE.x, 0, MAX_SIZE.y )
				+ new Vector3( 1, 1, -1 )
			)
			/ 2
			* TILE_SCALE_TO_REAL_SCALE;

		/// <summary>無い場合の、タイル番号</summary>
		public const int NONE_ID = -1;

		/// <summary>タイルモデルの一覧</summary>
		public readonly List<TileModel> _models;

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public TileManagerModel() {
			_models = Enumerable.Range( 0, MAX_ID )
				.Select( i => new TileModel( i ) )
				.ToList();

			_disposables.AddFirst( () => {
				_models.ForEach( m => m.Dispose() );
			} );
		}

		public void Initialize( AllModelManager manager ) {
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 取得
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号から、タイルモデルを取得
		/// </summary>
		public TileModel GetModel( int tileID )
			=> _models[tileID];

		/// <summary>
		/// ● タイル位置から、タイルモデルを取得
		/// </summary>
		public TileModel GetModel( Vector2Int tilePosition ) {
			var id = ToID( tilePosition );
			return _models[id];
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 変換
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号から、タイル位置に変換
		/// </summary>
		public static Vector2Int ToTilePosition( int tileID )
			=> new Vector2Int(
				tileID % 8,
				tileID / 8
			);

		/// <summary>
		/// ● タイル位置から、タイル番号に変換
		/// </summary>
		public static int ToID( Vector2Int tilePosition )
			=> (
				tilePosition.x +
				tilePosition.y * 8
			);

		/// <summary>
		/// ● タイル位置から、実際のメートル位置に変換
		/// </summary>
		public static Vector3 ToRealPosition( Vector2Int tilePosition )
			=> FIRST_REAL_POSITION
				+ (
					new Vector3( tilePosition.x, 0, -tilePosition.y )
					* TILE_SCALE_TO_REAL_SCALE
				);

		/// <summary>
		/// ● タイル方向から、実際のメートル方向に変換
		/// </summary>
		public static Vector3 ToRealDirection( Vector2Int tileDirection )
			=> new Vector3( tileDirection.x, 0, -tileDirection.y )
				* TILE_SCALE_TO_REAL_SCALE;
	}
}
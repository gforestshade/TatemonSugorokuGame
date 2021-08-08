using System.Linq;
using System.Collections.Generic;
using KoganeUnityLib;
using UnityEngine;
using SubmarineMirage.Base;
using SubmarineMirage.Extension;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイル管理の描画クラス
	/// </summary>
	public class TileManagerView : SMStandardMonoBehaviour {
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

		/// <summary>領域タイプから、色への変換</summary>
		public static readonly Dictionary<TileAreaType, Color> AREA_TYPE_TO_COLOR
			= new Dictionary<TileAreaType, Color>();

		/// <summary>無い場合の、タイル番号</summary>
		public const int NONE_ID = -1;



		[SerializeField] GameObject _prefab;
		readonly List<TileView> _views = new List<TileView>();

		[SerializeField] Color _noneAreaColor;
		[SerializeField] Color _player1AreaColor;
		[SerializeField] Color _player2AreaColor;



		void Start() {
			AREA_TYPE_TO_COLOR[TileAreaType.None] = _noneAreaColor;
			AREA_TYPE_TO_COLOR[TileAreaType.Player1] = _player1AreaColor;
			AREA_TYPE_TO_COLOR[TileAreaType.Player2] = _player2AreaColor;

			Enumerable.Range( 0, MAX_ID ).ForEach( i => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<TileView>();
				v.Setup( i );
				_views.Add( v );
			} );
		}



		/// <summary>
		/// ● 領域を変更
		/// </summary>
		public void ChangeArea( int tileID, int playerID ) {
			var type = ( TileAreaType )( playerID + 1 );
			var tile = GetView( tileID );
			tile.ChangeArea( type );
		}



		///----------------------------------------------------------------------------------------------------
		/// ● 取得
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号から、タイルを取得
		/// </summary>
		public TileView GetView( int tileID )
			=> _views[tileID];

		/// <summary>
		/// ● タイル位置から、タイルを取得
		/// </summary>
		public TileView GetView( Vector2Int tilePosition ) {
			var id = ToID( tilePosition );
			return _views[id];
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 変換
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号から、タイル位置に変換
		/// </summary>
		public static Vector2Int ToTilePosition( int tileID )
			=> new Vector2Int(
				tileID % MAX_SIZE.x,
				tileID / MAX_SIZE.y
			);

		/// <summary>
		/// ● タイル位置から、タイル番号に変換
		/// </summary>
		public static int ToID( Vector2Int tilePosition )
			=> (
				tilePosition.x +
				tilePosition.y * MAX_SIZE.y
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
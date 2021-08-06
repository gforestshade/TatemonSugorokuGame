using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ コマ管理のモデルクラス
	/// </summary>
	///========================================================================================================
	public class PieceManagerModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>移動方向タイプから、タイル方向への変換</summary>
		public static readonly Dictionary<PieceMoveType, Vector2Int> MOVE_TYPE_TO_TILE_DIRECTION
			= new Dictionary<PieceMoveType, Vector2Int> {
				{ PieceMoveType.None,   new Vector2Int( 0, 0 ) },
				{ PieceMoveType.Down,   new Vector2Int( 0, 1 ) },
				{ PieceMoveType.Left,   new Vector2Int( -1, 0 ) },
				{ PieceMoveType.Right,  new Vector2Int( 1, 0 ) },
				{ PieceMoveType.Up,     new Vector2Int( 0, -1 ) },
			};
		/// <summary>プレイヤー数</summary>
		public static readonly int PLAYER_COUNT = EnumUtils.GetLength<PlayerType>();

		/// <summary>コマモデルの一覧</summary>
		public readonly List<PieceModel> _models;

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public PieceManagerModel() {
			_models = Enumerable.Range( 0, PLAYER_COUNT )
				.Select( i => new PieceModel( ( PlayerType )i ) )
				.ToList();
		}

		public void Initialize( AllModelManager manager ) {
		}

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● モデルを取得
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public PieceModel GetModel( PlayerType type )
			=> _models[( int )type];
	}
}
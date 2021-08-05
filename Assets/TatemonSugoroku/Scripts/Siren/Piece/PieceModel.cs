using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ プレイヤーのコマのモデルクラス
	/// </summary>
	///========================================================================================================
	public class PieceModel : SMStandardBase {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>タイルマップの範囲内の大きさ</summary>
		static readonly Vector2Int MAX_SIZE = TileManagerModel.MAX_SIZE - Vector2Int.one;

		/// <summary>プレイヤーのタイプ</summary>
		public PlayerType _playerType	{ get; private set; }
		/// <summary>コマが配置されている、タイル番号</summary>
		public int _tileID				{ get; private set; }

		/// <summary>コマが配置されている、タイル位置</summary>
		public readonly ReactiveProperty<Vector2Int> _tilePosition = new ReactiveProperty<Vector2Int>();
		/// <summary>移動完了の通知、1歩移動するごとに呼ばれる</summary>
		public readonly Subject<Unit> _moveFinish = new Subject<Unit>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public PieceModel( PlayerType type ) {
			_playerType = type;

			var tileID = 0;
			switch ( type ) {
				case PlayerType.Player1:	tileID = 0;								break;
				case PlayerType.Player2:	tileID = TileManagerModel.MAX_ID - 1;	break;
			}
			_tileID = tileID;
			_tilePosition.Value = TileManagerModel.ToTilePosition( tileID );
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号の場所に、コマを配置
		/// </summary>
		public void Place( int tileID )
			=> Place( TileManagerModel.ToTilePosition( tileID ) );

		/// <summary>
		/// ● タイル位置に、コマを配置
		/// </summary>
		public void Place( Vector2Int tilePosition ) {
			var p = tilePosition;
			p.Clamp( TileManagerModel.MIN_SIZE, MAX_SIZE );
			_tileID = TileManagerModel.ToID( p );

			if ( p == _tilePosition.Value ) {
				MoveFinish();
				return;
			}
			_tilePosition.Value = p;
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 移動
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 移動方向タイプにより、移動
		/// </summary>
		public void Move( PieceMoveType moveType ) {
			var addTilePosition = PieceManagerModel.MOVE_TYPE_TO_TILE_DIRECTION[moveType];
			Place( _tilePosition.Value + addTilePosition );
		}

		/// <summary>
		/// ● タイル方向により、移動
		/// </summary>
		public void Move( Vector2Int addTilePosition ) {
			Place( _tilePosition.Value + addTilePosition );
		}

		/// <summary>
		/// ● 移動完了を設定、通知
		/// </summary>
		public void MoveFinish() {
			_moveFinish.OnNext( Unit.Default );
		}
	}
}
using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ コマのモデルクラス
	/// </summary>
	public class PieceModel : SMStandardBase, IModel {
		static readonly Vector2Int MAX_SIZE = TileManagerModel.MAX_SIZE - Vector2Int.one;

		public PieceType _type { get; private set; }

		public readonly ReactiveProperty<Vector2Int> _tilePosition = new ReactiveProperty<Vector2Int>();
		public readonly Subject<Unit> _moveFinish = new Subject<Unit>();



		public PieceModel( PieceType type ) {
			_type = type;

			var tileID = 0;
			switch ( type ) {
				case PieceType.Player1:	tileID = 0;								break;
				case PieceType.Player2:	tileID = TileManagerModel.MAX_ID - 1;	break;
			}
			_tilePosition.Value = TileManagerModel.ToTilePosition( tileID );
		}



		public void SetTilePosition( Vector2Int tilePosition ) {
			var p = tilePosition;
			p.Clamp( TileManagerModel.MIN_SIZE, MAX_SIZE );
			if ( p == _tilePosition.Value ) {
				MoveFinish();
				return;
			}
			_tilePosition.Value = p;
		}

		public void Move( PieceMoveType moveType ) {
			var addTilePosition = PieceManagerModel.MOVE_TYPE_TO_TILE_DIRECTION[moveType];
			SetTilePosition( _tilePosition.Value + addTilePosition );
		}

		public void Move( Vector2Int addTilePosition ) {
			SetTilePosition( _tilePosition.Value + addTilePosition );
		}

		public void MoveFinish() {
			_moveFinish.OnNext( Unit.Default );
		}
	}
}
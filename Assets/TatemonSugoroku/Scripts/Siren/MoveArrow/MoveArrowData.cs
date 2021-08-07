using UnityEngine;
using TatemonSugoroku.Scripts.Akio;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印管理の描画クラス
	/// </summary>
	public class MoveArrowData {
		public int _tileID				{ get; private set; }
		public MoveArrowType _type		{ get; private set; }
		public MoveArrowState _state	{ get; private set; }



		public MoveArrowData( int tileID, MoveArrowType type, MoveArrowState state ) {
			_tileID = tileID;
			_type = type;
			_state = state;
		}

		public MoveArrowData( int tileID, MoveArrowType type, MotionStatus motionState ) {
			_tileID = tileID;
			_type = type;

			switch ( motionState ) {
				case MotionStatus.Unmovable:	_state = MoveArrowState.Hide;		break;
				case MotionStatus.Movable:		_state = MoveArrowState.Enable;		break;
				case MotionStatus.Return:		_state = MoveArrowState.Disable;	break;
			}
		}

		public MoveArrowData( Vector2Int tilePosition, MoveArrowType type, MotionStatus motionState ) {
			_tileID = TileManagerView.ToID( tilePosition );
			_type = type;

			switch ( motionState ) {
				case MotionStatus.Unmovable:	_state = MoveArrowState.Hide;		break;
				case MotionStatus.Movable:		_state = MoveArrowState.Enable;		break;
				case MotionStatus.Return:		_state = MoveArrowState.Disable;	break;
			}
		}
	}
}
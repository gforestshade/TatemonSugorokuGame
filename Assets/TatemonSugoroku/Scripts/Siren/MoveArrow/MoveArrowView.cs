//#define TestMoveArrow
using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印の描画クラス
	/// </summary>
	public class MoveArrowView : SMStandardMonoBehaviour {
		MeshRenderer[] _renderers { get; set; }
		Color _disableColor { get; set; }
		[SerializeField] Vector3 _offset = new Vector3( 0, 0.6f, 0 );

		/// <summary>タイル番号</summary>
		public int _tileID { get; private set; }
		/// <summary>タイル位置</summary>
		public Vector2Int _tilePosition { get; private set; }
		/// <summary>矢印の回転角度</summary>
		public Quaternion _rotation { get; private set; }
		/// <summary>矢印のタイプ</summary>
		public MoveArrowType _type { get; private set; }
		/// <summary>矢印の状態</summary>
		public MoveArrowState _state { get; private set; }

		/// <summary>非同期停止の識別子</summary>
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		public void Setup( MoveArrowType type, Color disableColor ) {
			_renderers = GetComponentsInChildren<MeshRenderer>();
			_disableColor = disableColor;

			_type = type;
			gameObject.name = $"MoveArrow {_type}";
			transform.rotation = MoveArrowManagerView.ARROW_TYPE_TO_ROTATION[_type];
			gameObject.SetActive( false );

			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );

#if TestMoveArrow
			_offset.y += 0.3f;
			transform.rotation *= Quaternion.Euler( 90, 0, 0 );
#endif
		}



		public void UpdateColor( Color enableColor ) {
			var c = _state == MoveArrowState.Enable ? enableColor : _disableColor;
			_renderers.ForEach( r => r.material.color = c );
		}



		public void SetState( MoveArrowState state ) {
			if ( _state == state ) { return; }
			_state = state;

			gameObject.SetActive( false );
			UpdateColor( _disableColor );

			switch ( _state ) {
				case MoveArrowState.Hide:
					break;

				case MoveArrowState.Disable:
					gameObject.SetActive( true );
					break;

				case MoveArrowState.Enable:
					gameObject.SetActive( true );
					break;
			}
		}



		public void Place( MoveArrowData arrowData ) {
			var tilePosition = arrowData._tilePosition;
			transform.position = TileManagerView.ToRealPosition( tilePosition ) + _offset;
			transform.localScale = TileManagerView.REAL_SCALE;

			SetState( arrowData._state );
		}

		public void Hide()
			=> SetState( MoveArrowState.Hide );
	}
}
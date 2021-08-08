#define TestTatemon
using UnityEngine;
using UniRx;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ たてもんの描画クラス
	/// </summary>
	public class TatemonView : SMStandardMonoBehaviour {
		Renderer[] _renderers { get; set; }

		TatemonState _state { get; set; }
		/// <summary>プレイヤーのタイプ</summary>
		public PlayerType _playerType { get; private set; }

		/// <summary>手番の回数</summary>
		public int _turnID { get; private set; }
		/// <summary>タイル番号</summary>
		public int _tileID { get; private set; }
		/// <summary>タイル位置</summary>
		public Vector2Int _tilePosition { get; private set; }

		public float _speedRate		{ get; set; }
		int _rotatePower	{ get; set; }

		/// <summary>タイルが配置済みか？</summary>
		public bool _isPlaced => _state != TatemonState.None;

		Tween _rotateTween { get; set; }



		public void Setup( PlayerType playerType, int turnID, float speedRate ) {
			_playerType = playerType;
			_turnID = turnID;
			_speedRate = speedRate;

			gameObject.name = $"Tatemon {_playerType} {_turnID}";

#if TestTatemon
			_renderers = GetComponentsInChildren<Renderer>();
			if ( _playerType == PlayerType.Player2 ) {
				_renderers.ForEach( r => r.material.color = Color.red );
			}
//			_renderers[2].gameObject.SetActive( false );
//			_renderers[3].gameObject.SetActive( false );
#endif

			_state = TatemonState.Place;
			ChangeState( TatemonState.None );

			_disposables.AddLast( () => {
				_rotateTween?.Kill();
			} );
		}



		public void ChangeState( TatemonState state ) {
			if ( _state == state )	{ return; }
			_state = state;

			transform.position = TileManagerView.ToRealPosition( _tilePosition );
			transform.localScale = TileManagerView.REAL_SCALE;

			switch ( _state ) {
				case TatemonState.None:
					_rotateTween?.Kill();
					gameObject.SetActive( false );
					break;

				case TatemonState.Place:
					gameObject.SetActive( true );
					Rotate();
					break;
			}
		}



		public void Rotate() {
			_rotateTween?.Kill();
			var duration = 1f / ( _rotatePower * _rotatePower ) * _speedRate;
			var rate = 0f;
			_rotateTween = DOTween.To(
					() => rate,
					r => {
						rate = r;
						transform.rotation = Quaternion.Euler( 0, 360 * r, 0 );
					},
					1,
					duration
				)
				.SetEase( Ease.Linear )
				.SetLoops( -1, LoopType.Restart )
				.Play();
		}



		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号に、たてもんを配置
		/// </summary>
		public void Place( int tileID, int rotatePower )
			=> Place( TileManagerView.ToTilePosition( tileID ), rotatePower );

		/// <summary>
		/// ● タイル位置に、たてもんを配置
		/// </summary>
		public void Place( Vector2Int tilePosition, int rotatePower ) {
			_tilePosition = tilePosition;
			_tileID = TileManagerView.ToID( _tilePosition );
			_rotatePower = rotatePower;
			ChangeState( TatemonState.Place );
		}
	}
}
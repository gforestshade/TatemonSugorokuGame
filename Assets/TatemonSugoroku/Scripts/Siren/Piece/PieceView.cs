#define TestPiece
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ コマの描画クラス
	/// </summary>
	public class PieceView : SMStandardMonoBehaviour {
		/// <summary>タイルマップの範囲内の大きさ</summary>
		static readonly Vector2Int MAX_SIZE = TileManagerView.MAX_SIZE - Vector2Int.one;

		Renderer[] _renderers { get; set; }

		[SerializeField] Vector3 _offset = new Vector3( 0, 0.5f, 0 );
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		PieceType _type { get; set; }
		/// <summary>プレイヤーのタイプ</summary>
		public PlayerType _playerType { get; private set; }

		/// <summary>コマが配置されている、タイル番号</summary>
		public int _tileID { get; private set; }
		/// <summary>コマが配置されている、タイル位置</summary>
		public Vector2Int _tilePosition { get; private set; }

		public bool _isMoving { get; private set; }



		public void Setup( PieceType type, PlayerType playerType ) {
			_renderers = GetComponentsInChildren<Renderer>( true );

			_type = type;
			_playerType = playerType;

			var tileID = 0;
			switch ( playerType ) {
				case PlayerType.Player1:	tileID = 0;								break;
				case PlayerType.Player2:	tileID = TileManagerView.MAX_ID - 1;	break;
			}
			_tileID = tileID;
			_tilePosition = TileManagerView.ToTilePosition( tileID );
			transform.position = TileManagerView.ToRealPosition( _tilePosition ) + _offset;

#if TestPiece
			if ( _playerType == PlayerType.Player2 ) {
				_renderers.ForEach( r => r.material.color = Color.red );
			}
#endif
			if ( _type == PieceType.Dummy ) {
				_renderers.ForEach( r => {
					var c = r.material.color;
					c.a = 0.5f;
					r.material.color = c;
				} );
				Hide();
			}

			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );
		}



		protected override void Update() {
			base.Update();

			var p = Camera.main.transform.position;
			p.y = 0;
			transform.rotation = Quaternion.LookRotation( p );
		}



		public async UniTask Move( int tileID ) {
			var tilePosition = TileManagerView.ToTilePosition( tileID );
			await Move( tilePosition );
		}

		public async UniTask Move( Vector2Int tilePosition ) {
			_canceler.Cancel();

			_tilePosition = tilePosition;
/*
// tileIDで、値をClampできないので、コメント
			_tilePosition = new Vector2Int(
				Mathf.Clamp( _tilePosition.x, TileManagerView.MIN_SIZE.x, MAX_SIZE.y ),
				Mathf.Clamp( _tilePosition.y, TileManagerView.MIN_SIZE.y, MAX_SIZE.y )
			);
*/
			_tileID = TileManagerView.ToID( _tilePosition );

			var targetRealPosition = TileManagerView.ToRealPosition( _tilePosition ) + _offset;
			try {
				_isMoving = true;
				await transform.DOMove( targetRealPosition, 1 ).Play()
					.ToUniTask( TweenCancelBehaviour.Kill, _canceler.ToToken() );
			} finally {
				_isMoving = false;
			}
			transform.position = targetRealPosition;
		}



		public void Place( int tileID )
			=> Place( TileManagerView.ToTilePosition( tileID ) );

		public void Place( Vector2Int tilePosition ) {
			_canceler.Cancel();

			_tilePosition = tilePosition;
/*
// tileIDで、値をClampできないので、コメント
			_tilePosition = new Vector2Int(
				Mathf.Clamp( _tilePosition.x, TileManagerView.MIN_SIZE.x, MAX_SIZE.y ),
				Mathf.Clamp( _tilePosition.y, TileManagerView.MIN_SIZE.y, MAX_SIZE.y )
			);
*/
			_tileID = TileManagerView.ToID( _tilePosition );

			transform.position = TileManagerView.ToRealPosition( _tilePosition ) + _offset;
		}



		public void PlaceArrowPosition( int tileID ) {
			Place( tileID );
			gameObject.SetActive( true );
		}

		public void Hide() {
			gameObject.SetActive( false );
		}
	}
}
#define TestPiece
using UnityEngine;
using UniRx;
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
		PieceModel _model { get; set; }
		[SerializeField] Vector3 _offset = new Vector3( 0, 0.5f, 0 );
		Vector2Int _tilePosition { get; set; }
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



		public void Setup( PieceModel model ) {
			_model = model;
			_model._tilePosition.Subscribe( v => Move( v ).Forget() );

			_tilePosition = _model._tilePosition.Value;
			transform.position = TileManagerModel.ToRealPosition( _tilePosition ) + _offset;

#if TestPiece
			if ( _model._type == PieceType.Player2 ) {
				var renderers = GetComponentsInChildren<Renderer>( true );
				renderers.ForEach( r => r.material.color = Color.red );
			}
#endif

			_disposables.AddFirst( () => {
				_canceler.Dispose();
			} );
		}



		protected override void UpdateAfterInitialize() {
			var p = Camera.main.transform.position;
			p.y = 0;
			transform.rotation = Quaternion.LookRotation( p );
		}



		async UniTask Move( Vector2Int tilePosition ) {
			_canceler.Cancel();

			_tilePosition = tilePosition;
			var targetPosition = TileManagerModel.ToRealPosition( _tilePosition ) + _offset;

			await transform.DOMove( targetPosition, 1 ).Play()
				.ToUniTask( TweenCancelBehaviour.Kill, _canceler.ToToken() );
			transform.position = targetPosition;

			_model.MoveFinish();
		}
	}
}
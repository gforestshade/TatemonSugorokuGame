#define TestTatemon
using UnityEngine;
using UniRx;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ たてもんの描画クラス
	/// </summary>
	public class TatemonView : SMStandardMonoBehaviour {
		TatemonModel _model { get; set; }
		TatemonState _state { get; set; }



		public void Setup( TatemonModel model ) {
			_model = model;

			gameObject.name = _model._name;

			_model._state.Subscribe( s => {
				ChangeState( s );
			} );

#if TestTatemon
			var renderers = GetComponentsInChildren<SpriteRenderer>();
			if ( _model._playerType == PlayerType.Player2 ) {
				renderers.ForEach( r => r.material.color = Color.red );
			}
//			renderers[2].gameObject.SetActive( false );
//			renderers[3].gameObject.SetActive( false );
#endif
		}



		protected override void UpdateAfterInitialize() {
			switch ( _state ) {
				case TatemonState.Place:
					var targetPosition = Camera.main.transform.position;
					targetPosition.y = 0;
					transform.rotation = Quaternion.LookRotation( targetPosition );
					break;
			}
		}



		void ChangeState( TatemonState state ) {
			_state = state;

			transform.position = TileManagerModel.ToRealPosition( _model._tilePosition );
			transform.localScale = TileManagerModel.REAL_SCALE;

			switch ( _state ) {
				case TatemonState.None:
					gameObject.SetActive( false );
					break;

				case TatemonState.Place:
					gameObject.SetActive( true );
					break;

				case TatemonState.FireworkRotate:
					gameObject.SetActive( true );
					break;
			}
		}
	}
}
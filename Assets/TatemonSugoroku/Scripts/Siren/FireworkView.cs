//#define TestFirework
using System.Linq;
using UnityEngine;
using DG.Tweening;
using SubmarineMirage.Base;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火の描画クラス
	/// </summary>
	public class FireworkView : SMStandardMonoBehaviour {
		FireworkManagerView _manager { get; set; }
		SpriteRenderer _renderer { get; set; }
		Vector3 _maxScale { get; set; }
		Tween _rateTween { get; set; }
		bool _isActive { get; set; }



		public void Setup( FireworkManagerView manager ) {
			_manager = manager;

			_renderer = GetComponent<SpriteRenderer>();
			_maxScale = transform.localScale;

			transform.localScale = Vector3.zero;
			_renderer.material.color = Color.clear;
			_isActive = true;
			SetActive( false );

			_disposables.AddLast( () => {
				_rateTween?.Kill();
			} );

#if TestFirework
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				SetActive( !_isActive );
			} );
#endif
		}



		public void SetActive( bool isActive ) {
			if ( _isActive == isActive ) { return; }
			_isActive = isActive;

			_rateTween?.Kill();
			gameObject.SetActive( _isActive );

			if ( !_isActive ) { return; }

			_rateTween = DOTween.Sequence()
				.AppendCallback( () => {
					transform.localPosition = new Vector3(
						Random.Range( -6, 6 ),
						Random.Range( 3.3f, 1.3f ),
						15
					);
					transform.localScale = Vector3.zero;
					_renderer.material.color = Color.clear;
				} )
				.AppendInterval( Random.Range( 0, 2 ) )
				.Append( transform.DOScale( _maxScale, 1 ) )
				.Join( _renderer.material.DOColor( Color.white, 1 ) )
				.AppendInterval( 1 )
				.Append( _renderer.material.DOColor( Color.clear, 2 ) )
				.AppendInterval( Random.Range( 2, 5 ) )
				.SetEase( Ease.InOutQuad )
				.SetLoops( -1, LoopType.Restart )
				.Play();
		}
	}
}
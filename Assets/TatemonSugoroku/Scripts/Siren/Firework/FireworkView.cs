using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火の描画クラス
	/// </summary>
	public class FireworkView : SMStandardMonoBehaviour {
		SpriteRenderer _renderer { get; set; }
		Vector3 _maxScale { get; set; }
		bool _isActive { get; set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();
		SMAudioManager _audioManager { get; set; }



		public void Setup( FireworkManagerView manager ) {
			_renderer = GetComponent<SpriteRenderer>();
			_maxScale = transform.localScale;

			transform.localScale = Vector3.zero;
			_renderer.material.color = Color.clear;
			_isActive = true;
			SetActive( false );

			UTask.Void( async () => {
				_audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
			} );

			_disposables.AddLast( () => {
				_canceler.Dispose();
			} );
		}



		protected override void Update() {
			base.Update();

			var target = Camera.main.transform.position;
			target.y = transform.position.y;
//			transform.LookAt( target );
		}



		public void SetActive( bool isActive ) {
			if ( _isActive == isActive ) { return; }
			_isActive = isActive;

			_canceler.Cancel();
			gameObject.SetActive( _isActive );

			if ( !_isActive ) { return; }

			Launch().Forget();
		}

		async UniTask Launch() {
			_canceler.Cancel();

			while ( true ) {
				if ( _isDispose ) { return; }

				var camera = Camera.main?.transform;
				if ( camera == null ) { return; }

				var position = camera.position + camera.forward * 20;
				position += camera.rotation * new Vector3(
					Random.Range( -10, 10 ),
					Random.Range( 2, 6 ),
					Random.Range( -1, 1 )
				);
				transform.position = position;
				transform.rotation = camera.rotation;
				transform.localScale = Vector3.zero;
				_renderer.material.color = Color.white;

				await UTask.Delay( _canceler, Random.Range( 1000, 5000 ) );

				_audioManager.Play( SMSE.Firework ).Forget();

				await transform
					.DOScale( _maxScale, 0.2f )
					.SetEase( Ease.OutCirc )
					.Play()
					.ToUniTask( _canceler );

				_renderer.material
					.DOColor( Color.clear, 4 )
					.SetEase( Ease.OutCirc )
					.Play()
					.ToUniTask( _canceler )
					.Forget();

				await transform
					.DOScale( _maxScale + Vector3.one * 0.3f, 4 )
					.SetEase( Ease.OutCirc )
					.Play()
					.ToUniTask( _canceler );
			}
		}
	}
}
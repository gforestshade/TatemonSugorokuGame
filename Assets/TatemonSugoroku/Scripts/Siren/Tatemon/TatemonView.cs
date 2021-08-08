using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Setting;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ たてもんの描画クラス
	/// </summary>
	public class TatemonView : SMStandardMonoBehaviour {
		readonly Dictionary<string, SpriteRenderer> _renderers = new Dictionary<string, SpriteRenderer>();
		readonly List<ParticleSystem> _particles = new List<ParticleSystem>();
		PieceManagerView _pieceManager { get; set; }

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
		bool _isNearPiece { get; set; }
		float _placeEndSecond { get; set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();
		SMAudioManager _audioManager { get; set; }
		float _windSESeconds { get; set; }



		public void Setup( PlayerType playerType, int turnID, float speedRate ) {
			_playerType = playerType;
			_turnID = turnID;
			_speedRate = speedRate;

			gameObject.name = $"Tatemon {_playerType} {_turnID}";

			_pieceManager = FindObjectOfType<PieceManagerView>();

			var rs = GetComponentsInChildren<SpriteRenderer>();
			rs.ForEach( r => {
				_renderers[r.gameObject.name] = r;
			} );
			GetComponentsInChildren<ParticleSystem>().ForEach( p => {
				_particles.Add( p );
			} );

			_state = TatemonState.Place;
			ChangeState( TatemonState.None );

			_disposables.AddLast( () => {
				_canceler.Dispose();
			} );

			UTask.Void( async () => {
				_audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
			} );
		}



		protected override void Update() {
			base.Update();

			if ( _placeEndSecond > Time.time ) { return; }

			var isNear = _pieceManager._views.Any( v => {
				var delta = v.transform.position - transform.position;
				var distance = delta.magnitude;
				return distance < 1.2;
			} );

			if ( _isNearPiece == isNear ) { return; }
			_isNearPiece = isNear;
			_renderers.ForEach( pair => {
				var c = pair.Value.color;
				c.a = _isNearPiece ? 0.4f : 1;
				pair.Value.color = c;
			} );
		}



		public void ChangeState( TatemonState state ) {
			if ( _state == state )	{ return; }
			_state = state;

			transform.position = TileManagerView.ToRealPosition( _tilePosition ) + new Vector3( 0, 0.5f, 0 );
			transform.localScale = TileManagerView.REAL_SCALE;

			switch ( _state ) {
				case TatemonState.None:
					_canceler.Cancel();
					gameObject.SetActive( false );
					break;

				case TatemonState.Place:
					gameObject.SetActive( true );
					var scale = transform.localScale;
					transform.localScale = Vector3.zero;
					transform
						.DOScale( scale, 1 )
						.SetEase( Ease.OutBounce )
						.Play();
					_audioManager?.Play( SMSE.Tatemon ).Forget();
					var i = Random.Range( 0, 6 );
					switch ( i ) {
						case 0:	_audioManager?.Play( SMVoice.Wasshoi1 ).Forget();	break;
						case 1:	_audioManager?.Play( SMVoice.Wasshoi2 ).Forget();	break;
						case 2:	_audioManager?.Play( SMVoice.Wasshoi3 ).Forget();	break;
						case 3:	_audioManager?.Play( SMVoice.Wasshoi4 ).Forget();	break;
						case 4:	_audioManager?.Play( SMVoice.Wasshoi5 ).Forget();	break;
						case 5:	_audioManager?.Play( SMVoice.Wasshoi6 ).Forget();	break;
					}
					Rotate().Forget();
					_placeEndSecond = Time.time + 3;
					break;
			}
		}



		async UniTask Rotate() {
			_canceler.Cancel();

			while ( true ) {
				if ( _isDispose || _canceler._isCancel ) { return; }

				var duration = 1f / ( _rotatePower * _rotatePower ) * _speedRate;
				var rate = 0f;
				await DOTween.To(
						() => rate,
						r => {
							rate = r;
							transform.rotation = Quaternion.Euler( 0, 360 * r, 0 );
						},
						1,
						duration
					)
					.SetEase( Ease.Linear )
					.Play()
					.ToUniTask( TweenCancelBehaviour.Kill, _canceler.ToToken() );
				if ( _isDispose || _canceler._isCancel ) { return; }
/*
				if ( _rotatePower > 6 ) {
					if ( _windSESeconds < Time.time ) {
						_windSESeconds = Time.time + 0.5f;
						_audioManager?.Play( SMSE.TatemonWind ).Forget();
					}
				}
*/
			}
		}



		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号に、たてもんを配置
		/// </summary>
		public void Place( int tileID, int rotatePower, Sprite tatemon, Sprite aura )
			=> Place( TileManagerView.ToTilePosition( tileID ), rotatePower, tatemon, aura );

		/// <summary>
		/// ● タイル位置に、たてもんを配置
		/// </summary>
		public void Place( Vector2Int tilePosition, int rotatePower, Sprite tatemon, Sprite aura ) {
			_renderers["Front"].sprite = tatemon;
			_renderers["Aura"].sprite = aura;
			_renderers["Back"].sprite = tatemon;

//			_particles.ForEach( p => p.Play() );

			_tilePosition = tilePosition;
			_tileID = TileManagerView.ToID( _tilePosition );
			_rotatePower = rotatePower;
			ChangeState( TatemonState.Place );
		}
	}
}
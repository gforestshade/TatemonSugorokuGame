using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ たてもんのモデルクラス
	/// </summary>
	public class TatemonModel : SMStandardBase {
		TatemonManagerModel _manager { get; set; }

		public string _name				{ get; private set; }
		public PlayerType _playerType	{ get; private set; }
		public int _turnID				{ get; private set; }
		public int _tileID				{ get; private set; }
		public Vector2Int _tilePosition	{ get; private set; }

		public bool _isPlaced => _state.Value != TatemonState.None;
		public bool _isFireworkRotated => _state.Value == TatemonState.FireworkRotate;

		public ReactiveProperty<TatemonState> _state = new ReactiveProperty<TatemonState>();



		public TatemonModel( TatemonManagerModel manager, PlayerType playerType, int turnID ) {
			_manager = manager;
			_playerType = playerType;
			_turnID = turnID;

			_name = $"Tatemon {_playerType} {_turnID}";

			_disposables.AddFirst( () => {
				_state.Dispose();
			} );
		}



		public void Place( int tileID )
			=> Place( TileManagerModel.ToTilePosition( tileID ) );

		public void Place( Vector2Int tilePosition ) {
			_tilePosition = tilePosition;
			_tileID = TileManagerModel.ToID( _tilePosition );
			if ( _manager._isFireworkRotated ) {
				ChangeState( TatemonState.FireworkRotate );
			} else {
				ChangeState( TatemonState.Place );
			}
		}

		public void ChangeState( TatemonState state ) {
			_state.Value = state;
		}
	}
}
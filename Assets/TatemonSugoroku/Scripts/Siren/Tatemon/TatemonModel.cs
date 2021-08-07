using UnityEngine;
using UniRx;
using SubmarineMirage.Base;
using SubmarineMirage.Debug;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ たてもんのモデルクラス
	/// </summary>
	///========================================================================================================
	public class TatemonModel : SMStandardBase {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>たてもんの管理者</summary>
		TatemonManagerModel _manager { get; set; }

		/// <summary>ゲームオブジェクトに付ける名前</summary>
		public string _name				{ get; private set; }
		/// <summary>プレイヤーのタイプ</summary>
		public PlayerType _playerType	{ get; private set; }
		/// <summary>手番の回数</summary>
		public int _turnID				{ get; private set; }
		/// <summary>タイル番号</summary>
		public int _tileID				{ get; private set; }
		/// <summary>タイル位置</summary>
		public Vector2Int _tilePosition	{ get; private set; }

		/// <summary>タイルが配置済みか？</summary>
		public bool _isPlaced => _state.Value != TatemonState.None;
		/// <summary>タイルが花火で回転済みか？</summary>
		public bool _isFireworkRotated => _state.Value == TatemonState.FireworkRotate;

		/// <summary>たてもんの状態、状態遷移の代わり</summary>
		public ReactiveProperty<TatemonState> _state = new ReactiveProperty<TatemonState>();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public TatemonModel( TatemonManagerModel manager, PlayerType playerType, int turnID ) {
			_manager = manager;
			_playerType = playerType;
			_turnID = turnID;

			_name = $"Tatemon {_playerType} {_turnID}";

			_disposables.AddFirst( () => {
				_state.Dispose();
			} );
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● タイル番号に、たてもんを配置
		/// </summary>
		public void Place( int tileID )
			=> Place( TileManagerView.ToTilePosition( tileID ) );

		/// <summary>
		/// ● タイル位置に、たてもんを配置
		/// </summary>
		public void Place( Vector2Int tilePosition ) {
			_tilePosition = tilePosition;
			_tileID = TileManagerView.ToID( _tilePosition );
			if ( _manager._isFireworkRotated ) {
				ChangeState( TatemonState.FireworkRotate );
			} else {
				ChangeState( TatemonState.Place );
			}
		}

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● 状態遷移
		///		上手くマネージャーで、制御しています。
		///		特に触る必要はありません。
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public void ChangeState( TatemonState state ) {
			_state.Value = state;
		}
	}
}
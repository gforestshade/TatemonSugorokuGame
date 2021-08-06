using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Utility;
using SubmarineMirage.Debug;
using TatemonSugoroku.Scripts.Akio;
namespace TatemonSugoroku.Scripts {
	///========================================================================================================
	/// <summary>
	/// ■ たてもん管理のモデルクラス
	/// </summary>
	///========================================================================================================
	public class TatemonManagerModel : SMStandardBase, IModel {
		///----------------------------------------------------------------------------------------------------
		/// ● 要素
		///----------------------------------------------------------------------------------------------------
		/// <summary>最大の1プレイヤーの手番</summary>
		const int MAX_PLAYER_TURN = 7;

		/// <summary>プレイヤーごとの、現在の手番回数</summary>
		readonly Dictionary<PlayerType, int> _turnCounts = new Dictionary<PlayerType, int>();
		/// <summary>たてもんのモデル一覧</summary>
		public readonly Dictionary< PlayerType, List<TatemonModel> > _models;
		/// <summary>花火で回転したか？</summary>
		public bool _isFireworkRotated { get; private set; }

		/// <summary>非同期停止の識別子</summary>
		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● コンストラクタ
		/// </summary>
		///----------------------------------------------------------------------------------------------------
		public TatemonManagerModel() {
			_models = EnumUtils.GetValues<PlayerType>().ToDictionary(
				type => type,
				type => Enumerable.Range( 0, MAX_PLAYER_TURN )
					.Select( i => new TatemonModel( this, type, i ) )
					.ToList()
			);
			_turnCounts = EnumUtils.GetValues<PlayerType>().ToDictionary(
				type => type,
				type => 0
			);

			UTask.Void( async () => {
				var allModelManager = await SMServiceLocator.WaitResolve<AllModelManager>( _canceler );
				await UTask.NextFrame( _canceler );
				var gameManager = allModelManager.Get<MainGameManagementModel>();
				var firework = allModelManager.Get<FireworkManagerModel>();
				var field = allModelManager.Get<FieldModel>();

				gameManager.GamePhase.Subscribe( phase => {
					switch ( phase ) {
						case MainGamePhase.WaitingPuttingTatemon:
							var playerID = gameManager.CurrentPlayerId;
							var tileID = field.GetCurrentPositionByPlayerId( playerID.Value );
							Place( ( PlayerType )playerID.Value, tileID );
							field.PutTatemonAtCurrentPosition( playerID.Value, 0 );
							break;
					}
				} );

				firework._launch.Subscribe( _ => {
					_isFireworkRotated = true;
					GetModels()
						.Where( m => m._isPlaced )
						.ForEach( m => m.ChangeState( TatemonState.FireworkRotate ) );
				} );
			} );

			_disposables.AddFirst( () => {
				_canceler.Dispose();
				_models
					.SelectMany( pair => pair.Value )
					.ForEach( m => m.Dispose() );
				_models.Clear();
			} );
		}

		///----------------------------------------------------------------------------------------------------
		/// ● 取得
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● プレイヤー番号の、たてもんモデルの一覧を取得
		/// </summary>
		public List<TatemonModel> GetModels( PlayerType playerType )
			=> _models.GetOrDefault( playerType );

		/// <summary>
		/// ● 全たてもんモデルの一覧を取得
		/// </summary>
		public IEnumerable<TatemonModel> GetModels()
			=> _models.SelectMany( pair => pair.Value );

		///----------------------------------------------------------------------------------------------------
		/// ● 配置
		///----------------------------------------------------------------------------------------------------
		/// <summary>
		/// ● プレイヤー番号と、タイル番号の場所に、たてもんを配置
		///		何個目を配置するかは、自動計算。
		/// </summary>
		public TatemonModel Place( PlayerType playerType, int tileID )
			=> Place( playerType, TileManagerModel.ToTilePosition( tileID ) );

		/// <summary>
		/// ● プレイヤー番号と、タイル位置に、たてもんを配置
		///		何個目を配置するかは、自動計算。
		/// </summary>
		public TatemonModel Place( PlayerType playerType, Vector2Int tilePosition ) {
			var i = _turnCounts[playerType];

			if ( i >= MAX_PLAYER_TURN ) {
				throw new InvalidOperationException( string.Join( "\n",
					$"",
					$"最大ターン数以上に、配置が要求されました。",
					$"これ以上、たてもんを配置できません。",
					$"{nameof( i )} : {i}",
					$"{nameof( MAX_PLAYER_TURN )} : {MAX_PLAYER_TURN}",
					$"{this}"
				) );
			}

			var m = _models[playerType][i];
			_turnCounts[playerType] += 1;

			m.Place( tilePosition );
			return m;
		}
	}
}
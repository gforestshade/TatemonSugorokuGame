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
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ たてもん管理のモデルクラス
	/// </summary>
	public class TatemonManagerModel : SMStandardBase, IModel {
		const int MAX_PLAYER_TURN = 7;

		readonly Dictionary<PlayerType, int> _turnCounts = new Dictionary<PlayerType, int>();
		public readonly Dictionary< PlayerType, List<TatemonModel> > _models;
		public bool _isFireworkRotated { get; private set; }

		readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();



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
				var firework = allModelManager.Get<FireworkManagerModel>();
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



		public List<TatemonModel> GetModels( PlayerType playerType )
			=> _models.GetOrDefault( playerType );

		public IEnumerable<TatemonModel> GetModels()
			=> _models.SelectMany( pair => pair.Value );



		public TatemonModel Place( PlayerType playerType, int tileID )
			=> Place( playerType, TileManagerModel.ToTilePosition( tileID ) );

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
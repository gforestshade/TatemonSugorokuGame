using UnityEngine;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
using TatemonSugoroku.Scripts;
namespace TatemonSugoroku.Siren {



	/// <summary>
	/// ■ Siren用、デバッグの描画クラス
	///		このスクリプトをOFFにするだけで、デバッグ挙動は無くなります。
	/// </summary>
	public class SirenDebugView : SMStandardMonoBehaviour {
		protected override void StartAfterInitialize() {
			// 各種モデルを取得
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			var allModelManager = SMServiceLocator.Resolve<AllModelManager>();
			var tileManager = allModelManager.Get<TileManagerModel>();
			var moveArrowManager = allModelManager.Get<MoveArrowManagerModel>();
			var dice = allModelManager.Get<DiceModel>();
			var pieces = allModelManager.Get<PieceManagerModel>();
			var day = allModelManager.Get<DayModel>();
			var tatemons = allModelManager.Get<TatemonManagerModel>();


			// 13番のタイル領域変更
			inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
				var tile = tileManager.GetModel( 13 );
				var i = ( ( int )tile._areaType.Value + 1 ) % EnumUtils.GetLength<TileAreaType>();
				tile._areaType.Value = ( TileAreaType )i;
			} );
			// 座標から、13番のタイル領域変更
			inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				var tile = tileManager.GetModel( new Vector2Int( 5, 1 ) );
				var i = ( ( int )tile._areaType.Value + 1 ) % EnumUtils.GetLength<TileAreaType>();
				tile._areaType.Value = ( TileAreaType )i;
			} );

			// タイル領域変更
			var areaType = TileAreaType.None;
			inputManager.GetKey( SMInputKey.Finger2 )._enabledEvent.AddLast().Subscribe( _ => {
				var i = ( ( int )areaType + 1 ) % EnumUtils.GetLength<TileAreaType>();
				areaType = ( TileAreaType )i;
			} );
			// 矢印変更
			var arrowType = MoveArrowType.Down;
			inputManager.GetKey( SMInputKey.Finger2 )._enabledEvent.AddLast().Subscribe( _ => {
				var i = ( ( int )arrowType + 1 ) % EnumUtils.GetLength<MoveArrowType>();
				arrowType = ( MoveArrowType )i;
				SMLog.Debug( $"矢印 : {arrowType}" );
			} );

			// タッチしたタイル番号を表示
			inputManager._touchTileID.Subscribe( id => {
				SMLog.Debug( id );
			} );
			// タッチしたタイルを変更
			inputManager._touchTileID
				.Where( id => id != -1 )
				.Subscribe( id => {
					var tile = tileManager.GetModel( id );
					tile._areaType.Value = areaType;

					var arrow = moveArrowManager.GetModel( arrowType );
					arrow._placeTile.Value = tile;
				} );


			// サイコロを投げる
			var diceState = DiceState.Hide;
			inputManager.GetKey( SMInputKey.Quit )._enabledEvent.AddLast().Subscribe( _ => {
				var i = ( ( int )diceState + 1 ) % EnumUtils.GetLength<DiceState>();
				diceState = ( DiceState )i;
				dice.SetPower(
					new Vector3(
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 )
					).normalized * 10
				);
				dice.ChangeState( diceState );
			} );
			// サイコロの目を表示
			dice._total.Subscribe( i => SMLog.Debug( $"出目 : {i}" ) );

			// コマ1の移動
			var isMoveFinish1 = true;
			var piece1 = pieces.GetModel( PlayerType.Player1 );
			piece1._moveFinish.Subscribe( _ => isMoveFinish1 = true );
			inputManager._updateEvent.AddLast()
				.Where( _ => isMoveFinish1 )
				.Select( _ => inputManager.GetAxis( SMInputAxis.Move ) )
				.Where( move => move != Vector2Int.zero )
				.Subscribe( move => {
					var tileMove = new Vector2Int(
						move.x < 0 ? 1 : 0 < move.x ? -1 : 0,
						move.y < 0 ? -1 : 0 < move.y ? 1 : 0
					);
					if ( tileMove.x != 0 ) {
						tileMove.y = 0;
					}
					isMoveFinish1 = false;
					piece1.Move( tileMove );
				} );

			// コマ2の移動
			var isMoveFinish2 = true;
			var piece2 = pieces.GetModel( PlayerType.Player2 );
			piece2._moveFinish.Subscribe( _ => isMoveFinish2 = true );
			inputManager._updateEvent.AddLast()
				.Where( _ => isMoveFinish2 )
				.Select( _ => inputManager.GetAxis( SMInputAxis.Debug ) )
				.Where( move => move != Vector2Int.zero )
				.Subscribe( move => {
					var tileMove = new Vector2Int(
						move.x < 0 ? 1 : 0 < move.x ? -1 : 0,
						move.y < 0 ? -1 : 0 < move.y ? 1 : 0
					);
					if ( tileMove.x != 0 ) {
						tileMove.y = 0;
					}
					isMoveFinish2 = false;
					piece2.Move( tileMove );
				} );

			// 日時を更新
			inputManager.GetKey( SMInputKey.Reset )._enabledEvent.AddLast().Subscribe( _ => {
				day.UpdateHour();
			} );

			// タッチしたタイルにたてもんを配置
			var tatemonPlayer = PlayerType.Player1;
			inputManager._touchTileID
				.Where( id => id != -1 )
				.Subscribe( id => {
					var m = tatemons.Place( tatemonPlayer, id );
					SMLog.Debug( $"たてもん : {tatemonPlayer} {m._turnID}" );

					var i = ( ( int )tatemonPlayer + 1 ) % EnumUtils.GetLength<PlayerType>();
					tatemonPlayer = ( PlayerType )i;
				} );
		}
	}
}
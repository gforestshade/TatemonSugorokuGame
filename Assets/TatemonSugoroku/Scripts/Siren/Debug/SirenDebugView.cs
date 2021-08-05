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
	/// </summary>
	public class SirenDebugView : SMStandardMonoBehaviour {
		protected override void StartAfterInitialize() {
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			var allModelManager = SMServiceLocator.Resolve<AllModelManager>();
			var tileManager = allModelManager.Get<TileManagerModel>();
			var moveArrowManager = allModelManager.Get<MoveArrowManagerModel>();
			var dice = allModelManager.Get<DiceModel>();



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
				dice._power.OnNext(
					new Vector3(
						Random.Range( -10, 10 ),
						Random.Range( -10, 10 ),
						Random.Range( -10, 10 )
					)
				);
				dice.ChangeState( diceState );
			} );
			// サイコロの目を表示
			dice._total.Subscribe( i => SMLog.Debug( $"出目 : {i}" ) );
		}
	}
}
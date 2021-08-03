using UnityEngine;
using UniRx;
using SubmarineMirage.Service;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;



/// <summary>
/// ■ 入力の設定クラス
/// </summary>
public class SMInputSetting : BaseSMInputSetting {

	/// <summary>
	/// ● 設定
	/// </summary>
	public override void Setup( SMInputManager inputManager ) {
		base.Setup( inputManager );

		SetTouchTile();
	}

	/// <summary>
	/// ● タイル接触を設定
	/// </summary>
	void SetTouchTile() {
		var layerManager = SMServiceLocator.Resolve<SMUnityLayerManager>();

		_inputManager.GetKey( SMInputKey.Finger1 )._enablingEvent.AddLast().Subscribe( _ => {
			if ( Camera.main == null )	{ return; }

			var ray = Camera.main.ScreenPointToRay( _inputManager.GetAxis( SMInputAxis.Mouse ) );
			var hit = new RaycastHit();
			var layer = layerManager.GetMask( SMUnityLayer.Tile );
			var isHit = Physics.Raycast( ray, out hit, 100, layer );

			if ( isHit ) {
				var go = hit.collider.gameObject;
				var tile = go.GetComponent<TileView>();
				_inputManager._touchTileID.Value = tile._tileID;
				return;
			}

			_inputManager._touchTileID.Value = TileViewManager.NONE_TILE_ID;
		} );
	}
}
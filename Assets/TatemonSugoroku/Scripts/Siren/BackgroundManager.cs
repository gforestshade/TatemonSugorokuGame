#define TestBackground
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;



/// <summary>
/// ■ 背景の管理クラス
/// </summary>
public class BackgroundManager : SMStandardMonoBehaviour {
	int _showIndex;
	List<GameObject> _sprites;



	void Start() {
		_sprites = GetComponentsInChildren<SpriteRenderer>( true )
			.Select( r => r.gameObject )
			.ToList();

#if TestBackground
		UTask.Void( async () => {
			var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
			await framework.WaitInitialize();

			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
				_showIndex = ( _showIndex + 1 ) % _sprites.Count;
				_sprites.ForEach( go => go.SetActive( false ) );
				_sprites[_showIndex].gameObject.SetActive( true );
			} );
		} );
#endif
	}



	void Update() {
	}
}
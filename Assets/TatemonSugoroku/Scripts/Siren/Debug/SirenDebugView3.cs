using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Audio;
using SubmarineMirage.Scene;
using SubmarineMirage.Setting;
using SubmarineMirage.Debug;
using TatemonSugoroku.Scripts;
namespace TatemonSugoroku.Siren {



	/// <summary>
	/// ■ Siren用、デバッグの描画クラス3
	///		シーン遷移のテスト用。
	/// </summary>
	public class SirenDebugView3 : MonoBehaviour {



		async void Start() {
			var framework = SMServiceLocator.Resolve<SubmarineMirageFramework>();
			await framework.WaitInitialize();

			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			var inputManager = SMServiceLocator.Resolve<SMInputManager>();
			inputManager.GetKey( SMInputKey.Quit )._enabledEvent.AddLast().Subscribe( _ => {
				sceneManager.GetFSM<MainSMScene>().ChangeState<SirenSMScene>().Forget();
			} );

		}
	}
}
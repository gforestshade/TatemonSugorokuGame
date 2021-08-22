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
				sceneManager.GetFSM<MainSMScene>().ChangeState<TitleSMScene>().Forget();
			} );


			return;


			var effects = GameObject.Find( "EffectTap1" ).GetComponentsInChildren<ParticleSystem>();
			var effectTop = effects.First().transform;
			effectTop.SetParent( null );

			inputManager.GetKey( SMInputKey.Decide )._enabledEvent.AddLast().Subscribe( _ => {
				var mouse = inputManager.GetAxis( SMInputAxis.Mouse );
				var mousePosition = new Vector3( mouse.x, mouse.y, 1 );
				var position = Camera.main.ScreenToWorldPoint( mousePosition );
				effectTop.position = position;
				effects.ForEach( e => e.Play() );
			} );
		}
	}
}
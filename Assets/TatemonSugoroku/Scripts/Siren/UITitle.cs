using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Task;
using SubmarineMirage.Audio;
using SubmarineMirage.Scene;
using SubmarineMirage.Utility;
using SubmarineMirage.Setting;
namespace TatemonSugoroku.Scripts {



	public class UITitle : SMStandardMonoBehaviour {
		protected override void StartAfterInitialize() {
			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			var taskManager = SMServiceLocator.Resolve<SMTaskManager>();
			var audioManager = SMServiceLocator.Resolve<SMAudioManager>();

			var buttons = GetComponentsInChildren<Button>( true );
			buttons.ForEach( b => {
				b.onClick.AddListener( () => {

					switch ( b.name ) {
						case "Hajimeru":
							UTask.Void( async () => {
								await audioManager.Play( SMSE.Decide );
								sceneManager.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
							} );
							break;

						case "Asobikara":
							audioManager.Play( SMSE.Decide ).Forget();
							sceneManager.GetFSM<UISMScene>().ChangeState<UIHelpSMScene>().Forget();
							break;

						case "Yameru":
							UTask.Void( async () => {
								await audioManager.Play( SMSE.Decide );
								taskManager.Finalize().Forget();
							} );
							break;
					}
				} );
			} );
		}
	}
}
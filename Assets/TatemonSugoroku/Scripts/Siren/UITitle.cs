using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Scene;
using SubmarineMirage.Task;
namespace TatemonSugoroku.Scripts {



	public class UITitle : SMStandardMonoBehaviour {
		protected override void StartAfterInitialize() {
			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
			var taskManager = SMServiceLocator.Resolve<SMTaskManager>();

			var buttons = GetComponentsInChildren<Button>( true );
			buttons.ForEach( b => {
				b.onClick.AddListener( () => {
					switch ( b.name ) {
						case "Hajimeru":
							sceneManager.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
							break;

						case "Asobikara":
							sceneManager.GetFSM<UISMScene>().ChangeState<UIHelpSMScene>().Forget();
							break;

						case "Yameru":
							taskManager.Finalize().Forget();
							break;
					}
				} );
			} );
		}
	}
}
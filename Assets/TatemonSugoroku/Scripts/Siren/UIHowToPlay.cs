using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Scene;
namespace TatemonSugoroku.Scripts {



	public class UIHowToPlay : SMStandardMonoBehaviour {
		protected override void StartAfterInitialize() {
			var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();

			var button = GetComponentInChildren<Button>();
			button.onClick.AddListener( () => {
				sceneManager.GetFSM<UISMScene>().ChangeState<UIHelpSMScene>().Forget();
			} );
		}
	}
}
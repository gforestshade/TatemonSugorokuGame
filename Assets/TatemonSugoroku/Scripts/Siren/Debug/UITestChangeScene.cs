using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using SubmarineMirage.Service;
using SubmarineMirage.Scene;
using TatemonSugoroku.Scripts;
namespace TatemonSugoroku.Siren {



	/// <summary>
	/// ■ テスト用、シーン遷移のUIクラス
	/// </summary>
	public class UITestChangeScene : MonoBehaviour {



		async void Start() {
			var sceneManager = await SMServiceLocator.WaitResolve<SMSceneManager>();
			var button = GetComponentInChildren<Button>();

			button.onClick.AddListener( () => {
				sceneManager.GetFSM<MainSMScene>().ChangeState<GameSMScene>().Forget();
			} );
		}
	}
}
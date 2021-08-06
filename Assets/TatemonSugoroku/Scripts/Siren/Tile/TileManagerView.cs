using System.Collections.Generic;
using UnityEngine;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using TatemonSugoroku.Scripts.Akio;

namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ タイル管理の描画クラス
	/// </summary>
	public class TileManagerView : SMStandardMonoBehaviour {
		public static readonly Dictionary<TileAreaType, Color> AREA_TYPE_TO_COLOR =
			new Dictionary<TileAreaType, Color> {
				{ TileAreaType.None,       Color.white },
				{ TileAreaType.Player1,    Color.blue },
				{ TileAreaType.Player2,    Color.red },
			};

		TileManagerModel _model { get; set; }
		readonly List<TileView> _views = new List<TileView>();

		[SerializeField] GameObject _prefab;



		protected override void StartAfterInitialize() {
			/* こちらも誠に勝手ながら変更させていただきます。(by Akio)
			_model = SMServiceLocator.Resolve<AllModelManager>().Get<TileManagerModel>();
			_model._models.ForEach( m => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<TileView>();
				v.Setup( m );
				_views.Add( v );
			} );
			*/

			AllModelManager allModelManager = SMServiceLocator.Resolve<AllModelManager>();
			MainGameManagementModel mainGameManagementModel = allModelManager.Get<MainGameManagementModel>();
			TileManagerModel tileManagerModel = allModelManager.Get<TileManagerModel>();
			FieldModel fieldModel = allModelManager.Get<FieldModel>();
			
			tileManagerModel._models.ForEach(model =>
			{
				TileView tileView = _prefab.Instantiate(transform).GetComponent<TileView>();
				tileView.Setup(model);
				_views.Add(tileView);
			});
		}
	}
}
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Utility;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ コマ管理の描画クラス
	/// </summary>
	public class PieceManagerView : SMStandardMonoBehaviour {
		PieceManagerModel _model { get; set; }
		[SerializeField] GameObject _prefab;
		readonly List<PieceView> _views = new List<PieceView>();



		protected override void StartAfterInitialize() {
			_model = SMServiceLocator.Resolve<AllModelManager>().Get<PieceManagerModel>();

			_model._models.ForEach( m => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<PieceView>();
				v.Setup( m );
				_views.Add( v );
			} );
		}
	}
}
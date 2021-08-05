using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 移動矢印管理の描画クラス
	/// </summary>
	public class MoveArrowManagerView : SMStandardMonoBehaviour {
		MoveArrowManagerModel _model { get; set; }
		readonly List<MoveArrowView> _views = new List<MoveArrowView>();

		[SerializeField] GameObject _prefab;

		[SerializeField] Color _normalColor = Color.gray;
		[SerializeField] Color _flashColor = Color.yellow;
		Tween _colorTween { get; set; }



		protected override void StartAfterInitialize() {
			_model = SMServiceLocator.Resolve<AllModelManager>().Get<MoveArrowManagerModel>();
			_model._models.ForEach( pair => {
				var go = _prefab.Instantiate( transform );
				var v = go.GetComponent<MoveArrowView>();
				v.Setup( pair.Value );
				_views.Add( v );
			} );

			var color = _normalColor;
			_colorTween = DOTween.To(
				() => color,
				c => {
					color = c;
					_views.ForEach( v => v.SetColor( c ) );
				},
				_flashColor,
				1
			)
			.SetEase( Ease.InOutQuart )
			.SetLoops( -1, LoopType.Yoyo )
			.Play();


			_disposables.AddLast( () => {
				_colorTween?.Kill();
			} );
		}
	}
}
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using SubmarineMirage.Debug;

namespace TatemonSugoroku.Scripts
{



	/// <summary>
	/// ■ たてもん管理の描画クラス
	/// </summary>
	public class TatemonManagerView : SMStandardMonoBehaviour
	{
		TatemonManagerModel _model { get; set; }
		readonly List<TatemonView> _views = new List<TatemonView>();

		[SerializeField] GameObject _prefab;
		[SerializeField] float _speed = 5;
		Tween _rotateTween { get; set; }



		protected override void StartAfterInitialize()
		{
			_model = SMServiceLocator.Resolve<AllModelManager>().Get<TatemonManagerModel>();
			_model.GetModels().ForEach(m =>
			{
				var go = _prefab.Instantiate(transform);
				var v = go.GetComponent<TatemonView>();
				v.Setup(m);
				_views.Add(v);
			});


			var firework = SMServiceLocator.Resolve<AllModelManager>().Get<FireworkManagerModel>();
			firework._launch.Subscribe(_ =>
			{
				_rotateTween?.Kill();
				var rate = 0f;
				_rotateTween = DOTween.To(
						() => rate,
						r =>
						{
							rate = r;
							_views.ForEach(v => v.transform.rotation = Quaternion.Euler(0, 360 * r, 0));
						},
						1,
						_speed
					)
					.SetEase(Ease.Linear)
					.SetLoops(-1, LoopType.Restart)
					.Play();
			});


			_disposables.AddLast(() => { _rotateTween?.Kill(); });
		}
	}
}
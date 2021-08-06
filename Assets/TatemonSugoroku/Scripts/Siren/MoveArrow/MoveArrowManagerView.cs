using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;
using KoganeUnityLib;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using SubmarineMirage.Extension;
using TatemonSugoroku.Scripts.Akio;
using UnityEditor;

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

		private readonly MotionStatus[] _motionStatuses =
		{
			MotionStatus.Unmovable,
			MotionStatus.Unmovable,
			MotionStatus.Unmovable,
			MotionStatus.Unmovable
		};

		private int _peekPosition;
		protected override void StartAfterInitialize() {
			/* すみませんね。毎回変えさせてもらって。 (by Akio)
			_model = AllModelManager.s_instance.Get<MoveArrowManagerModel>();
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
			*/

			AllModelManager allModelManager = AllModelManager.s_instance;
			MoveArrowManagerModel moveArrowManagerModel = allModelManager.Get<MoveArrowManagerModel>();
			MotionModel motionModel = allModelManager.Get<MotionModel>();

			moveArrowManagerModel.Models.ForEach(model =>
			{
				MoveArrowView moveArrowView = _prefab.Instantiate(transform).GetComponent<MoveArrowView>();
				moveArrowView.Setup(model);
				_views.Add(moveArrowView);
			});

			motionModel.MotionStatusUp.Subscribe(status =>
			{
				_motionStatuses[0] = status;
				UpdateView();
			});

			motionModel.MotionStatusRight.Subscribe(status =>
			{
				_motionStatuses[1] = status;
				UpdateView();
			});

			motionModel.MotionStatusDown.Subscribe(status =>
			{
				_motionStatuses[2] = status;
				UpdateView();
			});

			motionModel.MotionStatusLeft.Subscribe(status =>
			{
				_motionStatuses[3] = status;
				UpdateView();
			});

			motionModel.PeekPosition.Subscribe(position =>
			{
				_peekPosition = position;
				UpdateView();
			});
		}

		void UpdateView()
		{
			int[] offsets = {-8, 1, 8, -1};
			for (int i = 0; i < _views.Count; i++)
			{
				MotionStatus status = _motionStatuses[i];
				if (status == MotionStatus.Unmovable)
				{
					_views[i].HideArrow();
				}
				else
				{
					int position = _peekPosition + offsets[i];
					Vector3 actualPosition = TileManagerModel.ToRealPosition(TileManagerModel.ToTilePosition(position));
					if (status == MotionStatus.Movable)
					{
						_views[i].ShowArrow(actualPosition, _flashColor);
					}
					else
					{
						_views[i].ShowArrow(actualPosition, _normalColor);
					}
				}
			}
		}
	}
}
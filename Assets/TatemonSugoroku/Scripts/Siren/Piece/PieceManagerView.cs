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
using TatemonSugoroku.Scripts.Akio;

namespace TatemonSugoroku.Scripts
{
	/// <summary>
	/// ■ コマ管理の描画クラス
	/// </summary>
	public class PieceManagerView : SMStandardMonoBehaviour
	{
		PieceManagerModel _model { get; set; }
		[SerializeField] GameObject _prefab;
		readonly List<PieceView> _views = new List<PieceView>();
		
		protected override void StartAfterInitialize()
		{
			/* 毎回すみません、せっかく作っていただいたのに残念です。(by Akio）
			_model = AllModelManager.s_instance.Get<PieceManagerModel>();

			_model._models.ForEach(m =>
			{
				var go = _prefab.Instantiate(transform);
				var v = go.GetComponent<PieceView>();
				v.Setup(m);
				_views.Add(v);
			});
			*/

			AllModelManager allModelManager = AllModelManager.s_instance;
			PieceManagerModel pieceManagerModel = allModelManager.Get<PieceManagerModel>();
			FieldModel fieldModel = allModelManager.Get<FieldModel>();
			
			pieceManagerModel._models.ForEach(model =>
			{
				PieceView pieceView = _prefab.Instantiate(transform).GetComponent<PieceView>();
				pieceView.Setup(model);
				_views.Add(pieceView);
			});
			


			fieldModel.PlayerPositions.Subscribe(positions =>
			{
				for (int i = 0; i < positions.Length; i++)
				{
					_views[i].Move(positions[i]);
				}
			});
		}
	}
}
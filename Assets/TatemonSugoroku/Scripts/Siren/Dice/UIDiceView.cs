using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using SubmarineMirage.Base;
using SubmarineMirage.Service;
using TatemonSugoroku.Scripts.Akio;

namespace TatemonSugoroku.Scripts {



	/// <summary>
	/// ■ 花火画面の描画クラス
	/// </summary>
	public class UIDiceView : SMStandardMonoBehaviour {
		// DiceModel _diceModel { get; set; }
		CanvasGroup _group { get; set; }
		Text _text { get; set; }



		void Start() {
			_group = GetComponent<CanvasGroup>();
			_group.alpha = 0;
			_group.blocksRaycasts = false;
			_group.interactable = false;

			_text = GetComponentInChildren<Text>();
		}

		protected override void StartAfterInitialize() {
			/* 誠に勝手ながら、こちらの方で変えさせてもらいます。（Akioより）
			_diceModel = AllModelManager.s_instance.Get<DiceModel>();

			_diceModel._state.Subscribe( s => {
				switch ( s ) {
					case DiceState.Hide:
					case DiceState.Roll:
						_group.alpha = 0;
						_group.blocksRaycasts = false;
						_group.interactable = false;
						break;

					case DiceState.Rotate:
						_group.alpha = 1;
						_group.blocksRaycasts = true;
						_group.interactable = true;
						break;
				}
			} );

			var button = GetComponentInChildren<Button>();
			button.onClick.AddListener( () => {
				_diceModel.SetPower(
					new Vector3(
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 ),
						Random.Range( -1, 1 )
					).normalized * 10
				);
				_diceModel.ChangeState( DiceState.Roll );
			} );
			*/

			AllModelManager allModelManager = AllModelManager.s_instance;
			MainGameManagementModel mainGameManagementModel = allModelManager.Get<MainGameManagementModel>();
			DiceModel diceModel = allModelManager.Get<DiceModel>();

			mainGameManagementModel.GamePhase.Subscribe(phase =>
			{
				switch (phase)
				{
					case MainGamePhase.WaitingRollingDice:
						_group.alpha = 1.0f;
						_group.blocksRaycasts = true;
						_group.interactable = true;
						break;
					case MainGamePhase.WhileRollingDice:
						_group.alpha = 1.0f;
						_group.blocksRaycasts = true;
						_group.interactable = false;
						break;
					default:
						_group.alpha = 0.0f;
						_group.blocksRaycasts = false;
						_group.interactable = false;
						break;
				}

				_text.text = $"サイコロ";
			} );

			Button button = GetComponentInChildren<Button>();
			
			button.onClick.AddListener(() =>
			{
				mainGameManagementModel.InputDicesRoll();
			});

			diceModel._total.Subscribe(numberOfDice =>
			{
				_text.text = $"{numberOfDice}";
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(_ =>
				{
					mainGameManagementModel.NotifyDiceRollingFinished(numberOfDice);
				});
			});
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SubmarineMirage.Utility;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage.Scene;
using SubmarineMirage.Audio;
using SubmarineMirage.Setting;
using SubmarineMirage;
using UnityEngine.UI;
using KoganeUnityLib;
using UniRx;
using Cysharp.Threading.Tasks;

namespace TatemonSugoroku.Scripts
{
    public class UIHelpCanvas : MonoBehaviour
    {
        [SerializeField]
        Transform _PageTop;

        [SerializeField]
        Button _Prev;

        [SerializeField]
        Button _Next;

        [SerializeField]
        Button _Close;

        [SerializeField]
        Button _End;


        readonly List<GameObject> _pages = new List<GameObject>();
        readonly SMAsyncCanceler _canceler = new SMAsyncCanceler();

        private void SetExplanations()
        {
            foreach ( Transform t in _PageTop ) {
                _pages.Add( t.gameObject );
			}
            SetPage(0);
        }

        private void Awake()
        {
            SetExplanations();

            var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
            SMAudioManager audioManager = null;
            UTask.Void(async () => {
                audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
            });

            // ページが変わったよイベント
            var page = new ReactiveProperty<int>(0);

            // ページが変わったら見た目も切り替える
            page.Pairwise().Subscribe(pair => UpdatePage(pair.Previous, pair.Current));

            // ページの端に来たらボタンが押せなくなる
            page.Select(p => p > 0).Subscribe(prev => _Prev.interactable = prev);
            page.Select(p => p < _pages.Count - 1).Subscribe(next => _Next.interactable = next);

            // ボタンでページを操作する
            _Prev.OnClickAsObservable().Subscribe(_ => page.Value--);
            _Next.OnClickAsObservable().Subscribe(_ => page.Value++);

            // ページが変わったら音を鳴らす
            // 増えても減っても同じ音
            page.Pairwise().Subscribe(_ => audioManager?.Play(SMSE.Decide).Forget());

            // 閉じるボタンで閉じる
            _Close.OnClickAsObservable().Subscribe( _ => {
                // 音
                audioManager?.Play( SMSE.Decide ).Forget();
                
                // シーン爆破
                sceneManager.GetFSM<UISMScene>().ChangeState<UINoneSMScene>().Forget();
            } );

            // タイトルに戻る
            _End.OnClickAsObservable().Subscribe( _ => {
                UTask.Void( async () => {
                    audioManager?.Play( SMSE.Decide ).Forget();
                    await UTask.Delay( _canceler, 500 );
                    sceneManager.GetFSM<UISMScene>().ChangeState<UINoneSMScene>().Forget();
                    sceneManager.GetFSM<MainSMScene>().ChangeState<TitleSMScene>().Forget();
                } );
            } );
            
            // タイトル画面でなければタイトルに戻れる
            _End.interactable = !( sceneManager.GetFSM<MainSMScene>()._state is TitleSMScene );
        }

        /// <summary>
        /// 強制的に即座にそのページ数に切り替える
        /// </summary>
        /// <param name="i"></param>
        private void SetPage(int i)
        {
            for (int j = 0; j < _pages.Count; j++)
            {
                _pages[j].SetActive(i == j);
            }
        }

        /// <summary>
        /// 前のページを消して次のページに切り替える
        /// </summary>
        /// <param name="i"></param>
        private void UpdatePage(int previous, int current)
        {
            _pages[previous].SetActive(false);
            _pages[current].SetActive(true);
        }

		private void OnDestroy() {
            _canceler.Dispose();
		}
	}
}

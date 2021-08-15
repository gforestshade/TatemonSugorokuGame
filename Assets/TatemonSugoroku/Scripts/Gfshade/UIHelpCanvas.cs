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


        readonly List<GameObject> _pages = new List<GameObject>();
        SMAudioManager audioManager = null;
        SMSceneManager sceneManager = null;

        /// <summary>
        /// フレームワーク初期化前に独自の初期化
        /// </summary>
        private void Init()
        {
            SetExplanations();
        }

        private void SetExplanations()
        {
            foreach ( Transform t in _PageTop ) {
                _pages.Add( t.gameObject );
			}
            SetPage(0);
        }

        /// <summary>
        /// フレームワーク初期化後に独自の初期化
        /// </summary>
        private void InitAfterFrameWork()
        {
            audioManager = SMServiceLocator.Resolve<SMAudioManager>();
            sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
        }

        private void Awake()
        {
            Init();

            var f = SMServiceLocator.Resolve<SubmarineMirageFramework>();

            // Frameworkの初期化終了イベント
            var initFrameworkAsObservable = f.ObserveEveryValueChanged(f => f._isInitialized).Where(b => b).First().Publish();
            System.IDisposable initFrameworkConnectDisposable = null;

            // Frameworkと独自の初期化が両方終了したよイベント
            AsyncSubject<Unit> init = new AsyncSubject<Unit>();

            // ページが変わったよイベント
            // 初期化が終わらない限り発火しない
            var pageRP = new ReactiveProperty<int>(0);
            var page = init.SelectMany(pageRP);

            // Frameworkの初期化終了をつかまえて独自の初期化を挟み込む
            initFrameworkAsObservable.Subscribe(_ => {
                InitAfterFrameWork();
                init.OnNext(Unit.Default);
                init.OnCompleted();
            });

            // ページが変わったら見た目も切り替える
            page.Pairwise().Subscribe(pair => UpdatePage(pair.Previous, pair.Current));

            // ページの端に来たらボタンが押せなくなる
            page.Select(p => p > 0).Subscribe(prev => _Prev.interactable = prev);
            page.Select(p => p < _pages.Count - 1).Subscribe(next => _Next.interactable = next);

            // ボタンでページを操作する
            _Prev.OnClickAsObservable().Subscribe(_ => pageRP.Value--);
            _Next.OnClickAsObservable().Subscribe(_ => pageRP.Value++);

            // ページが変わったら音を鳴らす
            // 増えても減っても同じ音
            page.Pairwise().Subscribe(_ => audioManager?.Play(SMSE.Decide).Forget());

            // 閉じるボタンで閉じる
            _Close.OnClickAsObservable().Subscribe( _ => {
                // 音
                audioManager?.Play( SMSE.Decide ).Forget();

                // 初期化イベントを開放するのを忘れない
                initFrameworkConnectDisposable.DisposeIfNotNull();
                
                // シーン爆破
                sceneManager.GetFSM<UISMScene>().ChangeState<UINoneSMScene>().Forget();
            } );

            initFrameworkConnectDisposable = initFrameworkAsObservable.Connect();
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
    }
}

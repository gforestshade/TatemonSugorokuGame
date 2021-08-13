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

        private void SetExplanations()
        {
            foreach ( Transform t in _PageTop ) {
                _pages.Add( t.gameObject );
			}
            _pages.ForEach( go => go.SetActive( false ) );
        }

        private void Awake()
        {
            var f = SMServiceLocator.Resolve<SubmarineMirageFramework>();
            var initf = f.ObserveEveryValueChanged(f => f._isInitialized).Where(b => b).First().Publish();
            AsyncSubject<Unit> init = new AsyncSubject<Unit>();

            var pageRP = new ReactiveProperty<int>(0);
            var page = init.SelectMany(pageRP);

            initf.Subscribe(_ => {
                var n = f._isInitialized;
                SetExplanations();
                init.OnNext(Unit.Default);
                init.OnCompleted();
            });

            SMAudioManager audioManager = null;
            UTask.Void( async () => {
                audioManager = await SMServiceLocator.WaitResolve<SMAudioManager>();
            } );

            page.Subscribe(UpdateText);
            page.Select(p => p > 0).Subscribe(prev => _Prev.interactable = prev);
            page.Select(p => p < _pages.Count - 1).Subscribe(next => _Next.interactable = next);
            _Prev.OnClickAsObservable().Subscribe(_ => {
                pageRP.Value--;
                audioManager?.Play( SMSE.Decide ).Forget();
            } );
            _Next.OnClickAsObservable().Subscribe(_ => {
                pageRP.Value++;
                audioManager?.Play( SMSE.Decide ).Forget();
            } );

            var sceneManager = SMServiceLocator.Resolve<SMSceneManager>();
            _Close.OnClickAsObservable().Subscribe( _ => {
                audioManager?.Play( SMSE.Decide ).Forget();
                sceneManager.GetFSM<UISMScene>().ChangeState<UINoneSMScene>().Forget();
            } );

            initf.Connect();
        }

        private void UpdateText(int i)
        {
            _pages.ForEach( go => go.SetActive( false ) );
            _pages[i].SetActive( true );
        }
    }
}

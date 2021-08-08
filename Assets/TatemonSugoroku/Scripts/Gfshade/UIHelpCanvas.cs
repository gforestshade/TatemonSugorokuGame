using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SubmarineMirage.Utility;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage;
using UnityEngine.UI;
using UniRx;

namespace TatemonSugoroku.Scripts
{
    public class UIHelpCanvas : MonoBehaviour
    {
        [SerializeField]
        Text _Explain;

        [SerializeField]
        Button _Prev;

        [SerializeField]
        Button _Next;

        [SerializeField]
        Button _Close;


        private List<string> explanations = null;
        int exMax;

        private void SetExplanations()
        {
            var data = SMServiceLocator
                .Resolve<SMAllDataManager>()
                .Get<int, Sample.SetsumeiItemData>()
                .GetAlls()
                .ToList();

            exMax = data.Max(i => i.Id);

            explanations = new List<string>(exMax+1);
            for (int i = 0; i < exMax+1; i++) explanations.Add(string.Empty);

            foreach (var i in data)
            {
                explanations[i.Id] = i.Explanation;
            }
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

            page.Subscribe(UpdateText);
            page.Select(p => p > 0).Subscribe(prev => _Prev.interactable = prev);
            page.Select(p => p < exMax).Subscribe(next => _Next.interactable = next);
            _Prev.OnClickAsObservable().Subscribe(_ => pageRP.Value--);
            _Next.OnClickAsObservable().Subscribe(_ => pageRP.Value++);

            initf.Connect();
        }

        private void UpdateText(int i)
        {
            _Explain.text = explanations[i];
        }
    }
}

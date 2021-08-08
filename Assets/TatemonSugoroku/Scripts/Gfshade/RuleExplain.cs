using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using SubmarineMirage;
using SubmarineMirage.Service;
using SubmarineMirage.Data;
using SubmarineMirage.Utility;
using System.Linq;

namespace TatemonSugoroku.Scripts
{
    public class RuleExplain : MonoBehaviour
    {
        private Text text;
        private List<string> explanations;



        public void Switch(int iPage)
        {
            text.text = explanations[iPage];
        }
    }
}

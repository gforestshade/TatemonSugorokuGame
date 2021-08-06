using UnityEngine;

namespace TatemonSugoroku.Scripts.Akio
{
    public class ScoreModel : IModel
    {
        public void Initialize( AllModelManager manager ) {
        }

        public void InitializeGame(int numberOfPlayers)
        {
            Debug.Log("Number of Players is" + numberOfPlayers);
        }
        
        public void Dispose()
        {
            
        }
    }
}

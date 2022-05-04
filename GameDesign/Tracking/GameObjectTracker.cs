using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWDargatzUtility.GameDesign.GameObjects;

namespace GameDesign.Tracking
{
    /// <summary>
    /// Used for tracking objects created from Designs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GameObjectTracker<T> where T : GameObject
    {
        private int currentID;
        protected Dictionary<int, T> IdDict;

        public GameObjectTracker()
        {
            IdDict = new Dictionary<int, T>();

            currentID = 0;
        }

        public virtual void AddTrackable(T toTrack)
        {
            toTrack.SetGameObjectId(currentID);
            IdDict.Add(toTrack.Id, toTrack);
            currentID++;
        }

        public IEnumerable<GameObject> GetAll
        {
            get
            {
                for (int i = 0; i < currentID; i++)
                {
                    yield return IdDict[i];
                }
            }
        }

        public T Get(int id) { return IdDict[id]; }

        public int TrackedItems { get { return IdDict.Count; } }
    }
}

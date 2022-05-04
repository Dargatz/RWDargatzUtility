using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWDargatzUtility.GameDesign.GameObjects;

namespace RWDargatzUtility.GameDesign.Tracking
{
    [Serializable]
    public class DesignTracker<T> where T : GameObjectDesign
    {
        private Dictionary<string, T> StringRefDict;
        private Dictionary<string, Dictionary<string, bool>> TagDict;

        public DesignTracker() : base()
        {
            StringRefDict = new Dictionary<string, T>();
            TagDict = new Dictionary<string, Dictionary<string, bool>>();
        }

        public void AddDesign(T toTrack)
        {
            StringRefDict.Add(toTrack.Ref, toTrack);

            if (toTrack.Tags != null) foreach (string tag in toTrack.Tags) AddToTagTracker(toTrack, tag);
        }

        private void AddToTagTracker(T toAdd, string tag)
        {
            if (!TagDict.ContainsKey(tag))
            {
                TagDict.Add(tag, new Dictionary<string, bool>());
            }

            TagDict[tag].Add(toAdd.Ref, true);
        }

        public T Get(string Ref)
        {
            if (!StringRefDict.ContainsKey(Ref)) throw new Exception("The following Ref was not found in the tracker: " + Ref);
            return StringRefDict[Ref];
        }

        public bool HasTag(string refString, string tag)
        {
            if (!TagDict.ContainsKey(tag)) return false;
            if (TagDict[tag].ContainsKey(refString)) return true;
            return false;
        }

        public T[] GetAllDesigns() => StringRefDict.Values.ToArray();
    }
}

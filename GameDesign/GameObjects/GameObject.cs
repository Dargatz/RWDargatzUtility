using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWDargatzUtility.GameDesign.GameObjects
{
    public class GameObject
    {
        public int Id { get; private set; }
        public string DesignRef { get; private set; }

        public GameObject(GameObjectDesign design)
        {
            DesignRef = design.Ref;
        }

        public void SetGameObjectId(int id) => Id = id; 
    }
}

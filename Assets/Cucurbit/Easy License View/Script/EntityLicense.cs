using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Cucurbit
{
    public class EntityLicense : ScriptableObject
    {
        public List<Param> param = new List<Param>();

        [System.SerializableAttribute]
        public class Param
        {
            public string Title;
            public string Provision;
        }
    }
}


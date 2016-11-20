namespace ECS.Controllers
{
    using System;
    using System.Collections.Generic;
    using ECS.Utilities;
    using UnityEngine;

    public class ECSController : MonoBehaviour
    {
        public static ECSController Instance { get; protected set; }

        public bool active { get; protected set; }

        public event Action OnPlay;
        public event Action OnPause;

        public Dictionary<string, EntityPrototype> prototypes;
        
        public void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There should only be one instance of ECSController in the scene.");

                Destroy(this);

                return;
            }

            Instance = this;
            this.active = true;
            this.prototypes = new Dictionary<string, EntityPrototype>();

            TextAsset jsonDoc = Resources.Load<TextAsset>("Entities");

            if (jsonDoc == null)
            {
                Debug.LogError("Could not find the Entities.JSON file");
                return;
            }

            this.GetPrototypesFromJSON(new JSONObject(jsonDoc.text));
        }

        public void Play()
        {
            this.active = true;

            if (OnPlay != null)
            {
                OnPlay();
            }
        }

        public void Pause()
        {
            active = false;

            if (OnPause != null)
            {
                OnPause();
            }
        }

        public void AssignPlayPause(Action OnPlay, Action OnPause)
        {
            this.OnPlay += OnPlay;
            this.OnPause += OnPause;
        }

        public void GetPrototypesFromJSON(JSONObject jsonObj)
        {
            if (jsonObj.type != JSONObject.Type.OBJECT)
            {
                Debug.Log(jsonObj.type.ToString());
                Debug.LogError("The Entities.JSON file did not contain a valid JSONObject for conversion to a Prototype");
                return;
            }

            for (int i = 0; i < jsonObj.keys.Count; i++)
            {
                this.prototypes.Add(jsonObj.keys[i].ToSlug(), new EntityPrototype(jsonObj.keys[i].ToSlug(), jsonObj.list[i]));
            }
        }

        public EntityPrototype GetPrototypeByName(string name)
        {
            name = name.ToSlug();

            if (this.prototypes.ContainsKey(name))
            {
                return this.prototypes[name];
            }

            return null;
        }
    }
}
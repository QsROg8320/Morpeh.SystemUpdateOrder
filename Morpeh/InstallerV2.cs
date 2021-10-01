namespace Morpeh {
#if UNITY_EDITOR
    using UnityEditor;
#endif
    using System.Linq;
    using UnityEngine;
    using Utils;
#if UNITY_EDITOR && ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
   
    using global::Unity.IL2CPP.CompilerServices;

    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.ArrayBoundsChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    [AddComponentMenu("ECS/" + nameof(InstallerV2))]
    public sealed class InstallerV2 : BaseInstaller {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [Required]
        [InfoBox("Order collision with other installer!", InfoMessageType.Error, nameof(IsCollisionWithOtherInstaller))]
        [PropertyOrder(-5)]
#endif
        public int order;
        
#if UNITY_EDITOR && ODIN_INSPECTOR
        private bool IsCollisionWithOtherInstaller 
            => this.IsPrefab() == false && FindObjectsOfType<Installer>().Where(i => i != this).Any(i => i.order == this.order);
        
        private bool IsPrefab() => this.gameObject.scene.name == null;
#endif
        
        [Space]
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(-5)]
#endif
        public Initializer[] initializers;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(-4)]
        [OnValueChanged(nameof(OnValueChangedUpdate))]
#endif
        public UpdateSystemPairV2[] updateSystems;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(-3)]
        [OnValueChanged(nameof(OnValueChangedFixedUpdate))]
#endif
        public FixedSystemPairV2[] fixedUpdateSystems;
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(-2)]
        [OnValueChanged(nameof(OnValueChangedLateUpdate))]
#endif
        public LateSystemPairV2[] lateUpdateSystems;

        private SystemsGroup group;

        private void OnValueChangedUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.updateSystems);
                this.AddSystems(this.updateSystems);
            }
        }
        
        private void OnValueChangedFixedUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.fixedUpdateSystems);
                this.AddSystems(this.fixedUpdateSystems);
            }
        }
        
        private void OnValueChangedLateUpdate() {
            if (Application.isPlaying) {
                this.RemoveSystems(this.lateUpdateSystems);
                this.AddSystems(this.lateUpdateSystems);
            }
        }

        [SerializeField]
        private bool _includeAllSystemsByDefault=true;

        [SerializeField]
#if  ODIN_INSPECTOR
        [Sirenix.OdinInspector.FolderPath(ParentFolder = "")]

#endif

        private string _scriptableObjectPath;


#if UNITY_EDITOR && ODIN_INSPECTOR
        [Button(Name ="Sort Systems")]
#endif
        private void SortSystems()
        {
        

            this.updateSystems = SystemHelper
                .GetSortedUpdateSystems(_scriptableObjectPath, _includeAllSystemsByDefault)
                .OfType<UpdateSystem>()
                .Select(x => new UpdateSystemPairV2(x))
                .ToArray();

            this.lateUpdateSystems = SystemHelper
                .GetSortedLateUpdateSystems(_scriptableObjectPath, _includeAllSystemsByDefault)
                .OfType<LateUpdateSystem>()
                .Select(x => new LateSystemPairV2(x))
                .ToArray();

            this.fixedUpdateSystems = SystemHelper
                .GetSortedFixedUpdateSystems(_scriptableObjectPath, _includeAllSystemsByDefault)
                .OfType<FixedUpdateSystem>()
                .Select(x => new FixedSystemPairV2(x))
                .ToArray();

     



        }

        protected override void OnEnable() {
            this.group = World.Default.CreateSystemsGroup();
            
            for (int i = 0, length = this.initializers.Length; i < length; i++) {
                var initializer = this.initializers[i];
                this.group.AddInitializer(initializer);
            }

            this.AddSystems(this.updateSystems);
            this.AddSystems(this.fixedUpdateSystems);
            this.AddSystems(this.lateUpdateSystems);
            
            World.Default.AddSystemsGroup(this.order, this.group);
        }

        protected override void OnDisable() {
            this.RemoveSystems(this.updateSystems);
            this.RemoveSystems(this.fixedUpdateSystems);
            this.RemoveSystems(this.lateUpdateSystems);
            
            World.Default.RemoveSystemsGroup(this.group);
        }

        private void AddSystems<T>(BasePairV2<T>[] pairs) where T : class, ISystem {
            for (int i = 0, length = pairs.Length; i < length; i++) {
                var pair   = pairs[i];
                var system = pair.System;
                pair.group = this.group;
                if (system != null) {
                    this.group.AddSystem(system, pair.Enabled);
                }
                else {
                    this.SystemNullError();
                }
            }
        }

        private void SystemNullError() {
            var go = this.gameObject;
            Debug.LogError($"[MORPEH] System null in installer {go.name} on scene {go.scene.name}", go);
        }

        private void RemoveSystems<T>(BasePairV2<T>[] pairs) where T : class, ISystem {
            for (int i = 0, length = pairs.Length; i < length; i++) {
                var system = pairs[i].System;
                if (system != null) {
                    this.group.RemoveSystem(system);
                }
            }
        }
        
#if UNITY_EDITOR
        [MenuItem("GameObject/ECS/", true, 10)]
        private static bool OrderECS() => true;

        [MenuItem("GameObject/ECS/InstallerV2", false, 1)]
        private static void CreateInstaller(MenuCommand menuCommand) {
            var go = new GameObject("[Installer]");
            go.AddComponent<InstallerV2>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif
    }

    namespace Utils {
        using System;
        using JetBrains.Annotations; 
#if UNITY_EDITOR && ODIN_INSPECTOR
        using Sirenix.OdinInspector;
#endif
        [Serializable]
        public abstract class BasePairV2<T> where T : class, ISystem {
            internal SystemsGroup group;

            [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
            [HorizontalGroup("Pair", 10)]
            [HideLabel]
            [OnValueChanged(nameof(OnChange))]
#endif
            private bool enabled;

#pragma warning disable CS0649
            [SerializeField]
#if UNITY_EDITOR && ODIN_INSPECTOR
            [HorizontalGroup("Pair")]
            [HideLabel]
            [Required]
#endif
            [CanBeNull]
            private T system;
#pragma warning restore CS0649

            public bool Enabled {
                get => this.enabled;
                set => this.enabled = value;
            }

            [CanBeNull]
            public T System => this.system;

            public BasePairV2() => this.enabled = true;
            public BasePairV2(T system) 
            {
                this.system = system;
                this.enabled = true;
            }
            private void OnChange() {
#if UNITY_EDITOR

                if (Application.isPlaying) {
                    if (this.enabled) {
                        this.group.EnableSystem(this.system);
                    }
                    else {
                        this.group.DisableSystem(this.system);
                    }
                }
#endif
            }
        }

        [Serializable]
        public class UpdateSystemPairV2 : BasePairV2<UpdateSystem> {
            public UpdateSystemPairV2(UpdateSystem system) : base(system) { }
           
        }

        [Serializable]
        public class FixedSystemPairV2 : BasePairV2<FixedUpdateSystem> {
            public FixedSystemPairV2(FixedUpdateSystem system) : base(system) { }
        }

        [Serializable]
        public class LateSystemPairV2 : BasePairV2<LateUpdateSystem> {
            public LateSystemPairV2(LateUpdateSystem system) : base(system) { }
        }
    }
}

using Scellecs.Morpeh.Systems;
using Scellecs.Morpeh.Utils;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Scellecs.Morpeh
{
    public sealed class InstallerWithOrdering : Installer
    {
        [SerializeField]
        private bool _includeAllSystemsByDefault = true;
        [SerializeField]
#if ODIN_INSPECTOR
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
                .Select(x => new UpdateSystemPair() { System = x })
                .ToArray();

            this.lateUpdateSystems = SystemHelper
                .GetSortedLateUpdateSystems(_scriptableObjectPath, _includeAllSystemsByDefault)
                .OfType<LateUpdateSystem>()
                .Select(x => new LateSystemPair() { System=x})
                .ToArray();

            this.fixedUpdateSystems = SystemHelper
                .GetSortedFixedUpdateSystems(_scriptableObjectPath, _includeAllSystemsByDefault)
                .OfType<FixedUpdateSystem>()
                .Select(x => new FixedSystemPair() { System = x })
                .ToArray();
        }
        #if UNITY_EDITOR

            [MenuItem("GameObject/ECS/InstallerWithOrdering", false, 1)]
            private static void CreateInstaller(MenuCommand menuCommand)
            {
                var go = new GameObject("[Installer]");
                go.AddComponent<InstallerWithOrdering>();
                GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
                Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
                Selection.activeObject = go;
            }
        #endif
    }

}

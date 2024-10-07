using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelroshanTools.BuildSettingsPresets.Editor
{
    [InitializeOnLoad]
    public class BuildSettingsIjector : ScriptableSingleton<BuildSettingsIjector>
    {
        static BuildSettingsIjector()
        {
            // When instance is created, inspectorWindow may exist, but is empty.
            void InjectAfterFewFrames()
            {
                EditorApplication.update -= InjectAfterFewFrames;
                instance.Inject();
            }
            EditorApplication.delayCall += InjectAfterFewFrames;
        }

        private bool assemblyReloading;

        private void OnEnable()
        {
            EditorApplication.update += Inject;
            AssemblyReloadEvents.beforeAssemblyReload += () => assemblyReloading = true;
            AssemblyReloadEvents.afterAssemblyReload += () => assemblyReloading = false;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Inject;
        }

        public BuildSettingsPreset[] presets;

        private BuildSettingsPreset FindMatchingPreset(BuildSettingsPreset[] presets)
        {

            BuildSettingsPreset bestPreset = null;
            int bestPresetFactor = 3;

            foreach (var preset in presets)
            {
                var diff = preset.GetDiffFactor();
                if (diff == 0)
                {
                    return preset;
                }
                if (diff < bestPresetFactor)
                {
                    bestPreset = preset;
                    bestPresetFactor = diff;
                }
            }

            return bestPreset;
        }

        private BuildSettingsPreset[] GetPresets()
        {
            return AssetDatabase.FindAssets("t:" + nameof(BuildSettingsPreset))
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<BuildSettingsPreset>(p))
                .ToArray();
        }

        private BuildSettingsPreset currentPreset;

        private void Inject()
        {
            // TODO: scenes list scroll does not work
            foreach (var build in Resources.FindObjectsOfTypeAll<EditorWindow>().OfType<BuildPlayerWindow>())
            {
                EditorApplication.update -= Inject;
                if (build.rootVisualElement.Children().Any(c => c.name == "build-presets"))
                {
                    continue;
                }
                var existingPresets = GetPresets();
                currentPreset = FindMatchingPreset(existingPresets);
                var presets = new VisualElement() { name = "build-presets", style = { marginLeft = 278, flexDirection = FlexDirection.ColumnReverse, marginBottom = 100, flexGrow = 1, marginRight = 10 }, pickingMode = PickingMode.Ignore };
                var container = new VisualElement();

                container.Add(new Label("Builds presets [injected]") { style = { unityFontStyleAndWeight = FontStyle.Bold } });
                // TODO: update dropdown when assets are created/removed
                var dropdown = new DropdownField("Selected", choices: existingPresets.Select(p => p.name).ToList(), ArrayUtility.IndexOf(existingPresets, currentPreset));
                container.Add(dropdown);
                var preset = new ObjectField("Asset") { objectType = typeof(BuildSettingsPreset), value = currentPreset };
                preset.RegisterValueChangedCallback(ev => (ev.newValue as BuildSettingsPreset)?.Apply());
                preset.RegisterValueChangedCallback(ev => dropdown.SetValueWithoutNotify(ev.newValue?.name));
                preset.RegisterValueChangedCallback(ev => currentPreset = (BuildSettingsPreset)ev.newValue);
                container.Add(preset);
                dropdown.RegisterValueChangedCallback(ev => preset.value = currentPreset = existingPresets.FirstOrDefault(p => p.name == ev.newValue));
                var buttons = new VisualElement() { style = { flexDirection = FlexDirection.Row } };
                buttons.Add(new Button(() => (preset.value as BuildSettingsPreset).Apply()) { text = "Reload" });
                buttons.Add(new Button(() => (preset.value as BuildSettingsPreset).OverwriteWithCurrentBuildSettings()) { text = "Save" });
                buttons.Add(new Button(() => preset.value = Create()) { text = "Create new..." });
                container.Add(buttons);
                presets.Add(container);
                build.rootVisualElement.Add(presets);

                build.rootVisualElement.RegisterCallback<DetachFromPanelEvent>(ev => OnClose());
                break;
            }
        }

        private void OnClose()
        {
            if (currentPreset != null && currentPreset.GetDiffFactor() > 0 && !assemblyReloading)
            {
                var ask = EditorUtility.DisplayDialogComplex($"Save pending changes for {currentPreset.name}?", "There are unsaved editor build settings", "Save", "Ignore", "Discard");
                if (ask == 0)
                {
                    currentPreset.OverwriteWithCurrentBuildSettings();
                }
                else if (ask == 2)
                {
                    currentPreset.Apply();
                }
            }
            EditorApplication.update += Inject;
        }

        private BuildSettingsPreset Create()
        {
            BuildSettingsPreset preset = BuildSettingsPreset.FromCurrentSettings();
            var path = "Assets/preset.asset";
            var counter = 1;
            while (AssetDatabase.GUIDFromAssetPath(path) != default)
            {
                path = $"Assets/preset ({counter++}).asset";
            }
            AssetDatabase.CreateAsset(preset, path);

            Selection.activeObject = preset;
            return preset;
        }
    }
}
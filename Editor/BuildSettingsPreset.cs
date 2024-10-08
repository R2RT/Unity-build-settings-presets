﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace TelroshanTools.BuildSettingsPresets.Editor
{
    public class BuildSettingsPreset : ScriptableObject
    {
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum PresetVersion
        {
            [InspectorName("2019.2")] V2019_2,
            [InspectorName("2022.3")] V2022_3__0,
        }

        [SerializeField] public PresetVersion presetVersion;

        private const PresetVersion LatestVersion = PresetVersion.V2022_3__0;

        [Serializable]
        public class BuildScene
        {
            public SceneAsset scene;
            public string guid;
            public string path;
            public bool enabled;
        }

        [Header("Build settings")]
        [SerializeField]
        private BuildScene[] scenes;

        [SerializeField] private BuildTarget activeBuildTarget;
        [SerializeField] private List<string> activeScriptCompilationDefines = new();
        [SerializeField] private bool allowDebugging;
        [SerializeField] private MobileTextureSubtarget androidBuildSubtarget;
        [SerializeField] private AndroidETC2Fallback androidEtc2Fallback;
        [SerializeField] private bool buildAppBundle;
        [SerializeField] private bool buildScriptsOnly;
        [SerializeField] private bool compressFilesInPackage;
        [SerializeField] private bool connectProfiler;
        [SerializeField] private bool development;
        [SerializeField] private bool enableHeadlessMode;
        [SerializeField] private bool explicitArrayBoundsChecks;
        [SerializeField] private bool explicitDivideByZeroChecks;
        [SerializeField] private bool explicitNullChecks;
        [SerializeField] private bool exportAsGoogleAndroidProject;
        [SerializeField] private bool forceInstallation;
        [SerializeField] private bool installInBuildFolder;
        [SerializeField] private XcodeBuildConfig iOsBuildConfigType;
        [SerializeField] private bool movePackageToDiscOuterEdge;
        [SerializeField] private bool needSubmissionMaterials;
        [SerializeField] private PS4BuildSubtarget ps4BuildSubtarget;
        [SerializeField] private PS4HardwareTarget ps4HardwareTarget;
        [SerializeField] private BuildTargetGroup selectedBuildTargetGroup;
        [SerializeField] private BuildTarget selectedStandaloneTarget;
        [SerializeField] private int streamingInstallLaunchRange;
        [SerializeField] private bool symlinkLibraries;
        [SerializeField] private bool waitForPlayerConnection;
        [SerializeField] private string windowsDevicePortalAddress;
        [SerializeField] private string windowsDevicePortalPassword;
        [SerializeField] private string windowsDevicePortalUsername;
        [SerializeField] private WSABuildAndRunDeployTarget wsaBuildAndRunDeployTarget;
        [SerializeField] private string wsaUwpsdk;
        [SerializeField] private string wsaUwpVisualStudioVersion;
        [SerializeField] private XboxBuildSubtarget xboxBuildSubtarget;
        [SerializeField] private XboxOneDeployDrive xboxOneDeployDrive;
        [SerializeField] private XboxOneDeployMethod xboxOneDeployMethod;
        [SerializeField] private bool xboxOneRebootIfDeployFailsAndRetry;

        #region Properties added in V2022_3__0

        [SerializeField] private QNXOsVersion selectedQnxOsVersion;
        [SerializeField] private QNXArchitecture selectedQnxArchitecture;
        [SerializeField] private EmbeddedLinuxArchitecture selectedEmbeddedLinuxArchitecture;
        [SerializeField] private bool remoteDeviceInfo;
        [SerializeField] private string remoteDeviceAddress;
        [SerializeField] private string remoteDeviceUsername;
        [SerializeField] private string remoteDeviceExports;
        [SerializeField] private string pathOnRemoteDevice;
        [SerializeField] private StandaloneBuildSubtarget standaloneBuildSubtarget;
        [SerializeField] private WebGLTextureSubtarget webGLBuildSubtarget;
        [SerializeField] private AndroidETC2Fallback androidETC2Fallback;
        [SerializeField] private AndroidBuildSystem androidBuildSystem;
        [SerializeField] private AndroidBuildType androidBuildType;
        [SerializeField] private AndroidCreateSymbols androidCreateSymbols;
        [SerializeField] private WSAUWPBuildType wsaUWPBuildType;
        [SerializeField] private string wsaUWPSDK;
        [SerializeField] private string wsaMinUWPSDK;
        [SerializeField] private string wsaArchitecture;
        [SerializeField] private string wsaUWPVisualStudioVersion;
        [SerializeField] private int overrideMaxTextureSize;
        [SerializeField] private OverrideTextureCompression overrideTextureCompression;
        [SerializeField] private bool buildWithDeepProfilingSupport;
        [SerializeField] private bool symlinkSources;
        [SerializeField] private XcodeBuildConfig iOSXcodeBuildConfig;
        [SerializeField] private XcodeBuildConfig macOSXcodeBuildConfig;
        [SerializeField] private bool switchCreateRomFile;
        [SerializeField] private bool switchEnableRomCompression;
        [SerializeField] private bool switchSaveAdf;
        [SerializeField] private SwitchRomCompressionType switchRomCompressionType;
        [SerializeField] private int switchRomCompressionLevel;
        [SerializeField] private string switchRomCompressionConfig;
        [SerializeField] private bool switchNVNGraphicsDebugger;
        [SerializeField] private bool generateNintendoSwitchShaderInfo;
        [SerializeField] private bool switchNVNShaderDebugging;
        [SerializeField] private bool switchNVNDrawValidationLight;
        [SerializeField] private bool switchNVNDrawValidationHeavy;
        [SerializeField] private bool switchEnableMemoryTracker;
        [SerializeField] private bool switchWaitForMemoryTrackerOnStartup;
        [SerializeField] private bool switchEnableDebugPad;
        [SerializeField] private bool switchRedirectWritesToHostMount;
        [SerializeField] private bool switchHtcsScriptDebugging;
        [SerializeField] private bool switchUseLegacyNvnPoolAllocator;

        #endregion

        public void OverwriteWithCurrentBuildSettings()
        {
            scenes = EditorBuildSettings.scenes.Select(x => new BuildScene
            {
                scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(x.path),
                path = x.path,
                guid = x.guid.ToString(),
                enabled = x.enabled,
            })
                .ToArray();

            activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            activeScriptCompilationDefines =
                PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup)
                    .Split(';').ToList();
            allowDebugging = EditorUserBuildSettings.allowDebugging;
            androidBuildSubtarget = EditorUserBuildSettings.androidBuildSubtarget;
            androidEtc2Fallback = EditorUserBuildSettings.androidETC2Fallback;
            buildAppBundle = EditorUserBuildSettings.buildAppBundle;
            buildScriptsOnly = EditorUserBuildSettings.buildScriptsOnly;
            compressFilesInPackage = EditorUserBuildSettings.compressFilesInPackage;
            connectProfiler = EditorUserBuildSettings.connectProfiler;
            development = EditorUserBuildSettings.development;
            enableHeadlessMode = EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server;
            explicitArrayBoundsChecks = EditorUserBuildSettings.explicitArrayBoundsChecks;
            explicitDivideByZeroChecks = EditorUserBuildSettings.explicitDivideByZeroChecks;
            explicitNullChecks = EditorUserBuildSettings.explicitNullChecks;
            exportAsGoogleAndroidProject = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            forceInstallation = EditorUserBuildSettings.forceInstallation;
            installInBuildFolder = EditorUserBuildSettings.installInBuildFolder;
            iOsBuildConfigType = EditorUserBuildSettings.iOSXcodeBuildConfig;
            movePackageToDiscOuterEdge = EditorUserBuildSettings.movePackageToDiscOuterEdge;
            needSubmissionMaterials = EditorUserBuildSettings.needSubmissionMaterials;
            ps4BuildSubtarget = EditorUserBuildSettings.ps4BuildSubtarget;
            ps4HardwareTarget = EditorUserBuildSettings.ps4HardwareTarget;
            selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            selectedStandaloneTarget = EditorUserBuildSettings.selectedStandaloneTarget;
            streamingInstallLaunchRange = EditorUserBuildSettings.streamingInstallLaunchRange;
            symlinkLibraries = EditorUserBuildSettings.symlinkSources;
            waitForPlayerConnection = EditorUserBuildSettings.waitForPlayerConnection;
            windowsDevicePortalAddress = EditorUserBuildSettings.windowsDevicePortalAddress;
            windowsDevicePortalPassword = EditorUserBuildSettings.windowsDevicePortalPassword;
            windowsDevicePortalUsername = EditorUserBuildSettings.windowsDevicePortalUsername;
            wsaBuildAndRunDeployTarget = EditorUserBuildSettings.wsaBuildAndRunDeployTarget;
            wsaUwpsdk = EditorUserBuildSettings.wsaUWPSDK;
            wsaUwpVisualStudioVersion = EditorUserBuildSettings.wsaUWPVisualStudioVersion;
            xboxBuildSubtarget = EditorUserBuildSettings.xboxBuildSubtarget;
            xboxOneDeployDrive = EditorUserBuildSettings.xboxOneDeployDrive;
            xboxOneDeployMethod = EditorUserBuildSettings.xboxOneDeployMethod;
            xboxOneRebootIfDeployFailsAndRetry = EditorUserBuildSettings.xboxOneRebootIfDeployFailsAndRetry;

            selectedQnxOsVersion = EditorUserBuildSettings.selectedQnxOsVersion;
            selectedQnxArchitecture = EditorUserBuildSettings.selectedQnxArchitecture;
            selectedEmbeddedLinuxArchitecture = EditorUserBuildSettings.selectedEmbeddedLinuxArchitecture;
            remoteDeviceInfo = EditorUserBuildSettings.remoteDeviceInfo;
            remoteDeviceAddress = EditorUserBuildSettings.remoteDeviceAddress;
            remoteDeviceUsername = EditorUserBuildSettings.remoteDeviceUsername;
            remoteDeviceExports = EditorUserBuildSettings.remoteDeviceExports;
            pathOnRemoteDevice = EditorUserBuildSettings.pathOnRemoteDevice;
            standaloneBuildSubtarget = EditorUserBuildSettings.standaloneBuildSubtarget;
            webGLBuildSubtarget = EditorUserBuildSettings.webGLBuildSubtarget;
            androidETC2Fallback = EditorUserBuildSettings.androidETC2Fallback;
            androidBuildSystem = EditorUserBuildSettings.androidBuildSystem;
            androidBuildType = EditorUserBuildSettings.androidBuildType;
            androidCreateSymbols = EditorUserBuildSettings.androidCreateSymbols;
            wsaUWPBuildType = EditorUserBuildSettings.wsaUWPBuildType;
            wsaUWPSDK = EditorUserBuildSettings.wsaUWPSDK;
            wsaMinUWPSDK = EditorUserBuildSettings.wsaMinUWPSDK;
            wsaArchitecture = EditorUserBuildSettings.wsaArchitecture;
            wsaUWPVisualStudioVersion = EditorUserBuildSettings.wsaUWPVisualStudioVersion;
            overrideMaxTextureSize = EditorUserBuildSettings.overrideMaxTextureSize;
            overrideTextureCompression = EditorUserBuildSettings.overrideTextureCompression;
            buildWithDeepProfilingSupport = EditorUserBuildSettings.buildWithDeepProfilingSupport;
            symlinkSources = EditorUserBuildSettings.symlinkSources;
            iOSXcodeBuildConfig = EditorUserBuildSettings.iOSXcodeBuildConfig;
            macOSXcodeBuildConfig = EditorUserBuildSettings.macOSXcodeBuildConfig;
            switchCreateRomFile = EditorUserBuildSettings.switchCreateRomFile;
            switchEnableRomCompression = EditorUserBuildSettings.switchEnableRomCompression;
            switchSaveAdf = EditorUserBuildSettings.switchSaveADF;
            switchRomCompressionType = EditorUserBuildSettings.switchRomCompressionType;
            switchRomCompressionLevel = EditorUserBuildSettings.switchRomCompressionLevel;
            switchRomCompressionConfig = EditorUserBuildSettings.switchRomCompressionConfig;
            switchNVNGraphicsDebugger = EditorUserBuildSettings.switchNVNGraphicsDebugger;
            generateNintendoSwitchShaderInfo = EditorUserBuildSettings.generateNintendoSwitchShaderInfo;
            switchNVNShaderDebugging = EditorUserBuildSettings.switchNVNShaderDebugging;
            switchNVNDrawValidationLight = EditorUserBuildSettings.switchNVNDrawValidation_Light;
            switchNVNDrawValidationHeavy = EditorUserBuildSettings.switchNVNDrawValidation_Heavy;
            switchEnableMemoryTracker = EditorUserBuildSettings.switchEnableMemoryTracker;
            switchWaitForMemoryTrackerOnStartup = EditorUserBuildSettings.switchWaitForMemoryTrackerOnStartup;
            switchEnableDebugPad = EditorUserBuildSettings.switchEnableDebugPad;
            switchRedirectWritesToHostMount = EditorUserBuildSettings.switchRedirectWritesToHostMount;
            switchHtcsScriptDebugging = EditorUserBuildSettings.switchHTCSScriptDebugging;
            switchUseLegacyNvnPoolAllocator = EditorUserBuildSettings.switchUseLegacyNvnPoolAllocator;

            if (presetVersion < LatestVersion)
            {
                var previousVersion = presetVersion;
                presetVersion = LatestVersion;
                Debug.Log("Updated preset from version " + previousVersion + " to " + presetVersion +
                          ", used current build settings values for properties that were added since the last version");
            }

            EditorUtility.SetDirty(this);
        }

        public static BuildSettingsPreset FromCurrentSettings()
        {
            BuildSettingsPreset preset = CreateInstance<BuildSettingsPreset>();
            preset.presetVersion = LatestVersion;
            preset.OverwriteWithCurrentBuildSettings();

            return preset;
        }

        public void Apply()
        {
            EditorBuildSettings.scenes = scenes.Select(x => new EditorBuildSettingsScene()
            {
                guid = new GUID(x.guid),
                path = x.path,
                enabled = x.enabled,
            }).ToArray();

            EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup,
                string.Join(";", activeScriptCompilationDefines));
            EditorUserBuildSettings.allowDebugging = allowDebugging;
            EditorUserBuildSettings.androidBuildSubtarget = androidBuildSubtarget;
            EditorUserBuildSettings.androidETC2Fallback = androidEtc2Fallback;
            EditorUserBuildSettings.buildAppBundle = buildAppBundle;
            EditorUserBuildSettings.buildScriptsOnly = buildScriptsOnly;
            EditorUserBuildSettings.compressFilesInPackage = compressFilesInPackage;
            EditorUserBuildSettings.connectProfiler = connectProfiler;
            EditorUserBuildSettings.development = development;
            EditorUserBuildSettings.standaloneBuildSubtarget = enableHeadlessMode
                ? StandaloneBuildSubtarget.Server
                : StandaloneBuildSubtarget.Player;
            EditorUserBuildSettings.explicitArrayBoundsChecks = explicitArrayBoundsChecks;
            EditorUserBuildSettings.explicitDivideByZeroChecks = explicitDivideByZeroChecks;
            EditorUserBuildSettings.explicitNullChecks = explicitNullChecks;
            EditorUserBuildSettings.exportAsGoogleAndroidProject = exportAsGoogleAndroidProject;
            EditorUserBuildSettings.forceInstallation = forceInstallation;
            EditorUserBuildSettings.installInBuildFolder = installInBuildFolder;
            EditorUserBuildSettings.iOSXcodeBuildConfig = iOsBuildConfigType;
            EditorUserBuildSettings.movePackageToDiscOuterEdge = movePackageToDiscOuterEdge;
            EditorUserBuildSettings.needSubmissionMaterials = needSubmissionMaterials;
            EditorUserBuildSettings.ps4BuildSubtarget = ps4BuildSubtarget;
            EditorUserBuildSettings.ps4HardwareTarget = ps4HardwareTarget;
            EditorUserBuildSettings.selectedBuildTargetGroup = selectedBuildTargetGroup;
            EditorUserBuildSettings.selectedStandaloneTarget = selectedStandaloneTarget;
            EditorUserBuildSettings.streamingInstallLaunchRange = streamingInstallLaunchRange;
            EditorUserBuildSettings.symlinkSources = symlinkLibraries;
            EditorUserBuildSettings.waitForPlayerConnection = waitForPlayerConnection;
            EditorUserBuildSettings.windowsDevicePortalAddress = windowsDevicePortalAddress;
            EditorUserBuildSettings.windowsDevicePortalPassword = windowsDevicePortalPassword;
            EditorUserBuildSettings.windowsDevicePortalUsername = windowsDevicePortalUsername;
            EditorUserBuildSettings.wsaBuildAndRunDeployTarget = wsaBuildAndRunDeployTarget;
            EditorUserBuildSettings.wsaUWPSDK = wsaUwpsdk;
            EditorUserBuildSettings.wsaUWPVisualStudioVersion = wsaUwpVisualStudioVersion;
            EditorUserBuildSettings.xboxBuildSubtarget = xboxBuildSubtarget;
            EditorUserBuildSettings.xboxOneDeployDrive = xboxOneDeployDrive;
            EditorUserBuildSettings.xboxOneDeployMethod = xboxOneDeployMethod;
            EditorUserBuildSettings.xboxOneRebootIfDeployFailsAndRetry = xboxOneRebootIfDeployFailsAndRetry;

            if (presetVersion > PresetVersion.V2019_2)
            {
                EditorUserBuildSettings.selectedQnxOsVersion = selectedQnxOsVersion;
                EditorUserBuildSettings.selectedQnxArchitecture = selectedQnxArchitecture;
                EditorUserBuildSettings.selectedEmbeddedLinuxArchitecture = selectedEmbeddedLinuxArchitecture;
                EditorUserBuildSettings.remoteDeviceInfo = remoteDeviceInfo;
                EditorUserBuildSettings.remoteDeviceAddress = remoteDeviceAddress;
                EditorUserBuildSettings.remoteDeviceUsername = remoteDeviceUsername;
                EditorUserBuildSettings.remoteDeviceExports = remoteDeviceExports;
                EditorUserBuildSettings.pathOnRemoteDevice = pathOnRemoteDevice;
                EditorUserBuildSettings.standaloneBuildSubtarget = standaloneBuildSubtarget;
                EditorUserBuildSettings.webGLBuildSubtarget = webGLBuildSubtarget;
                EditorUserBuildSettings.androidETC2Fallback = androidETC2Fallback;
                EditorUserBuildSettings.androidBuildSystem = androidBuildSystem;
                EditorUserBuildSettings.androidBuildType = androidBuildType;
                EditorUserBuildSettings.androidCreateSymbols = androidCreateSymbols;
                EditorUserBuildSettings.wsaUWPBuildType = wsaUWPBuildType;
                EditorUserBuildSettings.wsaUWPSDK = wsaUWPSDK;
                EditorUserBuildSettings.wsaMinUWPSDK = wsaMinUWPSDK;
                EditorUserBuildSettings.wsaArchitecture = wsaArchitecture;
                EditorUserBuildSettings.wsaUWPVisualStudioVersion = wsaUWPVisualStudioVersion;
                EditorUserBuildSettings.overrideMaxTextureSize = overrideMaxTextureSize;
                EditorUserBuildSettings.overrideTextureCompression = overrideTextureCompression;
                EditorUserBuildSettings.buildWithDeepProfilingSupport = buildWithDeepProfilingSupport;
                EditorUserBuildSettings.symlinkSources = symlinkSources;
                EditorUserBuildSettings.iOSXcodeBuildConfig = iOSXcodeBuildConfig;
                EditorUserBuildSettings.macOSXcodeBuildConfig = macOSXcodeBuildConfig;
                EditorUserBuildSettings.switchCreateRomFile = switchCreateRomFile;
                EditorUserBuildSettings.switchEnableRomCompression = switchEnableRomCompression;
                EditorUserBuildSettings.switchSaveADF = switchSaveAdf;
                EditorUserBuildSettings.switchRomCompressionType = switchRomCompressionType;
                EditorUserBuildSettings.switchRomCompressionLevel = switchRomCompressionLevel;
                EditorUserBuildSettings.switchRomCompressionConfig = switchRomCompressionConfig;
                EditorUserBuildSettings.switchNVNGraphicsDebugger = switchNVNGraphicsDebugger;
                EditorUserBuildSettings.generateNintendoSwitchShaderInfo = generateNintendoSwitchShaderInfo;
                EditorUserBuildSettings.switchNVNShaderDebugging = switchNVNShaderDebugging;
                EditorUserBuildSettings.switchNVNDrawValidation_Light = switchNVNDrawValidationLight;
                EditorUserBuildSettings.switchNVNDrawValidation_Heavy = switchNVNDrawValidationHeavy;
                EditorUserBuildSettings.switchEnableMemoryTracker = switchEnableMemoryTracker;
                EditorUserBuildSettings.switchWaitForMemoryTrackerOnStartup = switchWaitForMemoryTrackerOnStartup;
                EditorUserBuildSettings.switchEnableDebugPad = switchEnableDebugPad;
                EditorUserBuildSettings.switchRedirectWritesToHostMount = switchRedirectWritesToHostMount;
                EditorUserBuildSettings.switchHTCSScriptDebugging = switchHtcsScriptDebugging;
                EditorUserBuildSettings.switchUseLegacyNvnPoolAllocator = switchUseLegacyNvnPoolAllocator;
            }

            if (presetVersion < LatestVersion)
            {
                // As we already applied the supported properties' values, this will just retrieve the new ones
                // from the current build settings values
                OverwriteWithCurrentBuildSettings();
            }
        }

        public int GetDiffFactor()
        {
            var factor = 0;
            foreach (var (a, b) in Enumerable.Zip(EditorBuildSettings.scenes.OrderBy(s => s.path), scenes.OrderBy(s => s.path), (a, b) => (a, b)))
            {
                factor += (a.path, a.enabled) == (b.path, b.enabled) ? 0 : 1;
            }

            factor += EditorUserBuildSettings.activeBuildTarget == activeBuildTarget ? 0 : 1;
            factor += PlayerSettings.GetScriptingDefineSymbolsForGroup(selectedBuildTargetGroup).Split(',').SequenceEqual(activeScriptCompilationDefines) ? 0 : 1;
            factor += EditorUserBuildSettings.allowDebugging == allowDebugging ? 0 : 1;
            factor += EditorUserBuildSettings.androidBuildSubtarget == androidBuildSubtarget ? 0 : 1;
            factor += EditorUserBuildSettings.androidETC2Fallback == androidEtc2Fallback ? 0 : 1;
            factor += EditorUserBuildSettings.buildAppBundle == buildAppBundle ? 0 : 1;
            factor += EditorUserBuildSettings.buildScriptsOnly == buildScriptsOnly ? 0 : 1;
            factor += EditorUserBuildSettings.compressFilesInPackage == compressFilesInPackage ? 0 : 1;
            factor += EditorUserBuildSettings.connectProfiler == connectProfiler ? 0 : 1;
            factor += EditorUserBuildSettings.development == development ? 0 : 1;
            factor += EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server ^ enableHeadlessMode ? 1 : 0;
            factor += EditorUserBuildSettings.explicitArrayBoundsChecks == explicitArrayBoundsChecks ? 0 : 1;
            factor += EditorUserBuildSettings.explicitDivideByZeroChecks == explicitDivideByZeroChecks ? 0 : 1;
            factor += EditorUserBuildSettings.explicitNullChecks == explicitNullChecks ? 0 : 1;
            factor += EditorUserBuildSettings.exportAsGoogleAndroidProject == exportAsGoogleAndroidProject ? 0 : 1;
            factor += EditorUserBuildSettings.forceInstallation == forceInstallation ? 0 : 1;
            factor += EditorUserBuildSettings.installInBuildFolder == installInBuildFolder ? 0 : 1;
            factor += EditorUserBuildSettings.iOSXcodeBuildConfig == iOsBuildConfigType ? 0 : 1;
            factor += EditorUserBuildSettings.movePackageToDiscOuterEdge == movePackageToDiscOuterEdge ? 0 : 1;
            factor += EditorUserBuildSettings.needSubmissionMaterials == needSubmissionMaterials ? 0 : 1;
            factor += EditorUserBuildSettings.ps4BuildSubtarget == ps4BuildSubtarget ? 0 : 1;
            factor += EditorUserBuildSettings.ps4HardwareTarget == ps4HardwareTarget ? 0 : 1;
            factor += EditorUserBuildSettings.selectedBuildTargetGroup == selectedBuildTargetGroup ? 0 : 1;
            factor += EditorUserBuildSettings.selectedStandaloneTarget == selectedStandaloneTarget ? 0 : 1;
            factor += EditorUserBuildSettings.streamingInstallLaunchRange == streamingInstallLaunchRange ? 0 : 1;
            factor += EditorUserBuildSettings.symlinkSources == symlinkLibraries ? 0 : 1;
            factor += EditorUserBuildSettings.waitForPlayerConnection == waitForPlayerConnection ? 0 : 1;
            factor += EditorUserBuildSettings.windowsDevicePortalAddress == windowsDevicePortalAddress ? 0 : 1;
            factor += EditorUserBuildSettings.windowsDevicePortalPassword == windowsDevicePortalPassword ? 0 : 1;
            factor += EditorUserBuildSettings.windowsDevicePortalUsername == windowsDevicePortalUsername ? 0 : 1;
            factor += EditorUserBuildSettings.wsaBuildAndRunDeployTarget == wsaBuildAndRunDeployTarget ? 0 : 1;
            factor += EditorUserBuildSettings.wsaUWPSDK == wsaUwpsdk ? 0 : 1;
            factor += EditorUserBuildSettings.wsaUWPVisualStudioVersion == wsaUwpVisualStudioVersion ? 0 : 1;
            factor += EditorUserBuildSettings.xboxBuildSubtarget == xboxBuildSubtarget ? 0 : 1;
            factor += EditorUserBuildSettings.xboxOneDeployDrive == xboxOneDeployDrive ? 0 : 1;
            factor += EditorUserBuildSettings.xboxOneDeployMethod == xboxOneDeployMethod ? 0 : 1;
            factor += EditorUserBuildSettings.xboxOneRebootIfDeployFailsAndRetry == xboxOneRebootIfDeployFailsAndRetry ? 0 : 1;

            if (presetVersion > PresetVersion.V2019_2)
            {
                factor += EditorUserBuildSettings.selectedQnxOsVersion == selectedQnxOsVersion ? 0 : 1;
                factor += EditorUserBuildSettings.selectedQnxArchitecture == selectedQnxArchitecture ? 0 : 1;
                factor += EditorUserBuildSettings.selectedEmbeddedLinuxArchitecture == selectedEmbeddedLinuxArchitecture ? 0 : 1;
                factor += EditorUserBuildSettings.remoteDeviceInfo == remoteDeviceInfo ? 0 : 1;
                factor += EditorUserBuildSettings.remoteDeviceAddress == remoteDeviceAddress ? 0 : 1;
                factor += EditorUserBuildSettings.remoteDeviceUsername == remoteDeviceUsername ? 0 : 1;
                factor += EditorUserBuildSettings.remoteDeviceExports == remoteDeviceExports ? 0 : 1;
                factor += EditorUserBuildSettings.pathOnRemoteDevice == pathOnRemoteDevice ? 0 : 1;
                factor += EditorUserBuildSettings.standaloneBuildSubtarget == standaloneBuildSubtarget ? 0 : 1;
                factor += EditorUserBuildSettings.webGLBuildSubtarget == webGLBuildSubtarget ? 0 : 1;
                factor += EditorUserBuildSettings.androidETC2Fallback == androidETC2Fallback ? 0 : 1;
                factor += EditorUserBuildSettings.androidBuildSystem == androidBuildSystem ? 0 : 1;
                factor += EditorUserBuildSettings.androidBuildType == androidBuildType ? 0 : 1;
                factor += EditorUserBuildSettings.androidCreateSymbols == androidCreateSymbols ? 0 : 1;
                factor += EditorUserBuildSettings.wsaUWPBuildType == wsaUWPBuildType ? 0 : 1;
                factor += EditorUserBuildSettings.wsaUWPSDK == wsaUWPSDK ? 0 : 1;
                factor += EditorUserBuildSettings.wsaMinUWPSDK == wsaMinUWPSDK ? 0 : 1;
                factor += EditorUserBuildSettings.wsaArchitecture == wsaArchitecture ? 0 : 1;
                factor += EditorUserBuildSettings.wsaUWPVisualStudioVersion == wsaUWPVisualStudioVersion ? 0 : 1;
                factor += EditorUserBuildSettings.overrideMaxTextureSize == overrideMaxTextureSize ? 0 : 1;
                factor += EditorUserBuildSettings.overrideTextureCompression == overrideTextureCompression ? 0 : 1;
                factor += EditorUserBuildSettings.buildWithDeepProfilingSupport == buildWithDeepProfilingSupport ? 0 : 1;
                factor += EditorUserBuildSettings.symlinkSources == symlinkSources ? 0 : 1;
                factor += EditorUserBuildSettings.iOSXcodeBuildConfig == iOSXcodeBuildConfig ? 0 : 1;
                factor += EditorUserBuildSettings.macOSXcodeBuildConfig == macOSXcodeBuildConfig ? 0 : 1;
                factor += EditorUserBuildSettings.switchCreateRomFile == switchCreateRomFile ? 0 : 1;
                factor += EditorUserBuildSettings.switchEnableRomCompression == switchEnableRomCompression ? 0 : 1;
                factor += EditorUserBuildSettings.switchSaveADF == switchSaveAdf ? 0 : 1;
                factor += EditorUserBuildSettings.switchRomCompressionType == switchRomCompressionType ? 0 : 1;
                factor += EditorUserBuildSettings.switchRomCompressionLevel == switchRomCompressionLevel ? 0 : 1;
                factor += EditorUserBuildSettings.switchRomCompressionConfig == switchRomCompressionConfig ? 0 : 1;
                factor += EditorUserBuildSettings.switchNVNGraphicsDebugger == switchNVNGraphicsDebugger ? 0 : 1;
                factor += EditorUserBuildSettings.generateNintendoSwitchShaderInfo == generateNintendoSwitchShaderInfo ? 0 : 1;
                factor += EditorUserBuildSettings.switchNVNShaderDebugging == switchNVNShaderDebugging ? 0 : 1;
                factor += EditorUserBuildSettings.switchNVNDrawValidation_Light == switchNVNDrawValidationLight ? 0 : 1;
                factor += EditorUserBuildSettings.switchNVNDrawValidation_Heavy == switchNVNDrawValidationHeavy ? 0 : 1;
                factor += EditorUserBuildSettings.switchEnableMemoryTracker == switchEnableMemoryTracker ? 0 : 1;
                factor += EditorUserBuildSettings.switchWaitForMemoryTrackerOnStartup == switchWaitForMemoryTrackerOnStartup ? 0 : 1;
                factor += EditorUserBuildSettings.switchEnableDebugPad == switchEnableDebugPad ? 0 : 1;
                factor += EditorUserBuildSettings.switchRedirectWritesToHostMount == switchRedirectWritesToHostMount ? 0 : 1;
                factor += EditorUserBuildSettings.switchHTCSScriptDebugging == switchHtcsScriptDebugging ? 0 : 1;
                factor += EditorUserBuildSettings.switchUseLegacyNvnPoolAllocator == switchUseLegacyNvnPoolAllocator ? 0 : 1;
            }
            return factor;
        }
    }
}
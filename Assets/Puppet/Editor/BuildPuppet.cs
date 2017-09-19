﻿// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// This class provides methods to build the puppet app in many different configurations.
// They are meant to be invoked from the build scripts in this repository.
public class BuildPuppet
{
    private static readonly string BuildFolder = "CAKE_SCRIPT_TEMPPuppetBuilds";
    private static readonly string AppIdentifier = "com.microsoft.azure.mobile.unity.puppet";

    static BuildPuppet()
    {
#if UNITY_5_6_OR_NEWER
        PlayerSettings.applicationIdentifier = AppIdentifier;
#else
        PlayerSettings.bundleIdentifier = AppIdentifier;
#endif
    }

    public static void BuildPuppetSceneAndroidMono()
    {
        CreateGoogleServicesJsonIfNotPresent();
        BuildPuppetScene(BuildTarget.Android, BuildTargetGroup.Android, ScriptingImplementation.Mono2x, "AndroidMonoBuild.apk");
    }

    public static void BuildPuppetSceneAndroidIl2CPP()
    {
        CreateGoogleServicesJsonIfNotPresent();
        BuildPuppetScene(BuildTarget.Android, BuildTargetGroup.Android, ScriptingImplementation.IL2CPP, "AndroidIL2CPPBuild.apk");
    }

    public static void BuildPuppetSceneIosMono()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        BuildPuppetScene(BuildTarget.iOS, BuildTargetGroup.iOS, ScriptingImplementation.Mono2x, "iOSMonoBuild");
    }

    public static void BuildPuppetSceneIosIl2CPP()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;
        BuildPuppetScene(BuildTarget.iOS, BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP, "iOSIL2CPPBuild");
    }

    public static void BuildPuppetSceneIosMonoDeviceSdk()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        BuildPuppetScene(BuildTarget.iOS, BuildTargetGroup.iOS, ScriptingImplementation.Mono2x, "iOSMonoBuild");
    }

    public static void BuildPuppetSceneIosIl2CPPDeviceSdk()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        BuildPuppetScene(BuildTarget.iOS, BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP, "iOSIL2CPPBuild");
    }

    public static void BuildPuppetSceneWsaNetXaml()
    {
        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.XAML;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WSA, ApiCompatibilityLevel.NET_4_6);
        BuildPuppetScene(BuildTarget.WSAPlayer, BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET, "WSANetBuildXaml");
    }

    public static void BuildPuppetSceneWsaIl2CPPXaml()
    {
        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.XAML;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WSA, ApiCompatibilityLevel.NET_4_6);
        BuildPuppetScene(BuildTarget.WSAPlayer, BuildTargetGroup.WSA, ScriptingImplementation.IL2CPP, "WSAIL2CPPBuildXaml");
    }

    public static void BuildPuppetSceneWsaNetD3D()
    {
        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
        PlayerSettings.WSA.compilationOverrides = PlayerSettings.WSACompilationOverrides.UseNetCore;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WSA, ApiCompatibilityLevel.NET_4_6);
        BuildPuppetScene(BuildTarget.WSAPlayer, BuildTargetGroup.WSA, ScriptingImplementation.WinRTDotNET, "WSANetBuildD3D");
    }

    public static void BuildPuppetSceneWsaIl2CPPD3D()
    {
        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.D3D;
        PlayerSettings.scriptingRuntimeVersion = ScriptingRuntimeVersion.Legacy;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.WSA, ApiCompatibilityLevel.NET_4_6);
        BuildPuppetScene(BuildTarget.WSAPlayer, BuildTargetGroup.WSA, ScriptingImplementation.IL2CPP, "WSAIL2CPPBuildD3D");
    }

    private static void BuildPuppetScene(BuildTarget target, BuildTargetGroup targetGroup, ScriptingImplementation scriptingImplementation, string outputPath)
    {
        PlayerSettings.SetScriptingBackend(targetGroup, scriptingImplementation);
        string[] puppetScene = { "Assets/Puppet/PuppetScene.unity" };
        var options = new BuildPlayerOptions
        {
            scenes = puppetScene,
            options = BuildOptions.None,
            locationPathName = Path.Combine(BuildFolder, outputPath),
            target = target
        };
        BuildPipeline.BuildPlayer(options);
    }

    // Increments build version for all platforms
    public static void IncrementVersionNumber()
    {
        var currentVersion = PlayerSettings.bundleVersion;
        Debug.Log("current version: " + currentVersion);
        var minorVersion = int.Parse(currentVersion.Substring(currentVersion.LastIndexOf(".") + 1)) + 1;
        var newVersion = currentVersion.Substring(0, currentVersion.LastIndexOf(".") + 1) + minorVersion;
        Debug.Log("new version: " + newVersion);
        PlayerSettings.bundleVersion = newVersion;
        PlayerSettings.Android.bundleVersionCode++;
    }

    // Sets version number for puppet app
    public static void SetVersionNumber()
    {
        var currentVersion = PlayerSettings.bundleVersion;
        var puppetVersion = Microsoft.Azure.Mobile.Unity.WrapperSdk.WrapperSdkVersion;
        PlayerSettings.bundleVersion = puppetVersion;
        PlayerSettings.Android.bundleVersionCode++;
    }

    // Detects whether there exists a "google-services.json" file, and if not,
    // copies the "google-services-placeholder.json" and imports it as a
    // "google-services.json" file. The resulting file contains only
    // placeholders for keys, not actual keys.
    private static void CreateGoogleServicesJsonIfNotPresent()
    {
        var actualFile = "Assets/google-services.json";
        if (File.Exists(actualFile))
        {
            return;
        }
        var placeholderFile = "Assets/google-services-placeholder.json";
        if (!File.Exists(placeholderFile))
        {
            Debug.Log("Could not find google services placeholder.");
        }
        File.Copy(placeholderFile, actualFile);
        AssetDatabase.ImportAsset(actualFile, ImportAssetOptions.Default);
    }
}
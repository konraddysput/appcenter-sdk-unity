﻿﻿// Copyright (c) Microsoft Corporation. All rights reserved.
//
// Licensed under the MIT license.

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine;
using System;

namespace Microsoft.Azure.Mobile.Unity.Push.Internal
{
    class PushInternal
    {
        private static AndroidJavaClass _push = new AndroidJavaClass("com.microsoft.azure.mobile.push.Push");
        public static void Initialize()
        {
            PushDelegate.SetDelegate();
        }

        public static void PostInitialize()
        {
            var instance = _push.CallStatic<AndroidJavaObject>("getInstance");
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            instance.Call("onActivityResumed", activity);
        }

        public static MobileCenterTask SetEnabledAsync(bool isEnabled)
        {
            var future = _push.CallStatic<AndroidJavaObject>("setEnabled", isEnabled);
            return new MobileCenterTask(future);
        }

        public static MobileCenterTask<bool> IsEnabledAsync()
        {
            var future = _push.CallStatic<AndroidJavaObject>("isEnabled");
            return new MobileCenterTask<bool>(future);
        }
        
        public static IntPtr GetNativeType()
        {
            return AndroidJNI.FindClass("com/microsoft/azure/mobile/push/Push");
        }

        public static void EnableFirebaseAnalytics()
        {
           _push.CallStatic("enableFirebaseAnalytics");
        }

        internal static void ReplayUnprocessedPushNotifications()
        {
            //TODO implement me
        }
    }
}
#endif

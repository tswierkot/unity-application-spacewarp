Feed me a beer (૭ ◉༬◉)૭⁾⁾⁾⁾ [![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/wad1m)
  
# Application Spacewarp Template Project
  
Project is preconfigured with AppSW for the Oculus Quest 1 &amp; 2 using a custom render pipeline 

![screenshot](https://raw.githubusercontent.com/nukadelic/unity-application-spacewarp/master/Img/screenshot.png)

### Update (Mar-2023)

+ Updated to Unity version to 2021.3.21f1
+ Updated URP version to 12.1.10 (with AppSW support)

### Update (Jan-2023)

+ Patch Alpha Clipping Issue [42dbf6a](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/42dbf6a25b33099b1249bcd03ccffc223224818e)
+ Patch Allow Material Override in shader graph [5a42db6](https://gin.g-node.org/FloppyDisk/asw-render-pipelines-universal/commit/5a42db665706440403125170c379deb6b998aff5)

### Update (Apr-2022)

Updated to latest commit [30e14a2](https://github.com/Oculus-VR/Unity-Graphics/tree/30e14a2ca18f7c4c9903767895c1ca15d1af6c76)

Unity version 2021.2+ [URPv12](https://github.com/Oculus-VR/Unity-Graphics/commit/4f6daf0a988e86df35739c5fddbf6fe9bf9bb773)

### Update (Jul-2022)

Added sample hlsl shader , see : Assets/Shaders/UnlitTemplate.shader

### APK File 

Try the build : https://github.com/nukadelic/unity-application-spacewarp/releases/tag/apk

### Notes

* 120Hz is still in experimental stage , to get it working enable it inside the Quest headset in the settings under experimental features. 
* Making materials in shader graph will support MotionVectors by default, custom hlsl shaders needs to be edited manually othewise they will jitter when in motion.

### Config 

* The scene in the screen shot can be found in `Assets/Example/Scenes/SampleScene.unity`
* Completely **removed** Oculus SDK plugin , now using only the package manager provided dll's  
* All the project settings are allready setup to be Spacewarp ready 
* Shader graph seems to be working as expected 
* Additional Project settings 
  * Linear , Vulkan 
  * Android 8.0 'Oreo' ( API Level 26 ) 
  * IL2CPP , .NET 4.x  
  * Texture Compression : ASTC 

### Building 

For faster builds you can swap to the Mono Scripting backend in `Project Settings > Player > Other Settings > Configuration > Scripting Backend` , but it is required to have IL2CPP when publishing the application on the store 
  
### Links 

* AppSW Developer Guide : https://developer.oculus.com/documentation/unity/unity-asw
* Video Lecture [Asynchronous Timewarp & Spacewarp for VR | Bonnie Mathew 2017](https://www.youtube.com/watch?v=gqVIJtRjtr8)
* [how to diagnose and fix common issues with AppSW-enabled apps](https://github.com/oculus-samples/Unity-AppSpaceWarp)

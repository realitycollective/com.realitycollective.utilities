# Reality Toolkit - utilities Platform Module

![com.realitycollective.utilities](https://github.com/realitycollective/realitycollective.logo/blob/main/RealityCollective/RepoBanners/com.realitycollective.utilities.png?raw=true)

A collection of useful utilities for Unity Projects by the Reality Collective.  Useful for any Unity Project to accelerate and code safely within Unity.

[![openupm](https://img.shields.io/npm/v/com.realitycollective.utilities?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.realitycollective.utilities/)
[![Discord](https://img.shields.io/discord/597064584980987924.svg?label=&logo=discord&logoColor=ffffff&color=7389D8&labelColor=6A7EC2)](https://discord.gg/hF7TtRCFmB)
[![Publish development branch on Merge](https://github.com/realitycollective/com.realitycollective.utilities/actions/workflows/development-publish.yml/badge.svg)](https://github.com/realitycollective/com.realitycollective.utilities/actions/workflows/development-publish.yml)
[![Build and test UPM packages for platforms, all branches except main](https://github.com/realitycollective/com.realitycollective.utilities/actions/workflows/development-buildandtestupmrelease.yml/badge.svg)](https://github.com/realitycollective/com.realitycollective.utilities/actions/workflows/development-buildandtestupmrelease.yml)

## What's included?

### Utilities

* Async - A collection of Asynchronous and CoRoutine helpers for working with Sync -> Async code.

### Extensions

* AnimationCurveExtensions
* ArrayExtensions
* AssemblyExtensions
* BoundsExtensions
* CameraExtensions
* CollectionsExtensions
* ColliderExtensions
* CollisionExtensions
* Color32Extensions
* ComparerExtensions
* ComponentExtensions
* ConverterExtensions
* DoubleExtensions
* EnumExtensions
* EnumerableExtensions
* FloatExtensions
* GameObjectExtensions
* LayerExtensions
* MathfExtensions
* MatrixExtensions
* ProcessExtensions
* QuaternionExtensions
* RayExtensions
* SpriteExtensions
* StringExtensions
* SystemNumericsExtensions
* TextureExtensions
* TransformExtensions
* UnityObjectExtensions
* VectorExtensions

### Property Attributes

* EnumFlagsAttribute
* Il2CppSetOptionAttribute
* ImplementsAttribute
* PhysicsLayerAttribute
* PrefabAttribute
* Vector3RangeAttribute

### Functional Utilities

The Utilities package includes a well-formed logging solution that is single instanced and can optionally connect to Unity log events, useful for tracking both application and Unity logs and dramatically reduces the default Unity logging.  Also includes options to set whether debug logs should be output.

* StaticLogger - A single instance logging solution for capturing, saving and exposing logged events from both Unity and the Application.
* CaptureApplicationLog - A static router to push Unity events direct from a Unity scene.
* FilterLogType - A filtering definition to limit what logs are captured / exposed

## Requirements
<!-- Fill in list of requirements here -->

* [Unity 2022.3 and above](https://unity.com/)

### OpenUPM

The simplest way to getting started using the utilities package in your project is via OpenUPM. Visit [OpenUPM](https://openupm.com/docs/) to learn more about it. Once you have the OpenUPM CLI set up use the following command to add the package to your project:

```text
    openupm add com.realitycollective.utilities
```

## Feedback

Please feel free to provide feedback via the [Reality Toolkit dev channel here](https://github.com/realitycollective/com.realitycollective.utilities/issues), all feedback. suggestions and fixes are welcome.

## Related Articles

- tbc

---

## Raise an Information Request

If there is anything not mentioned in this document or you simply want to know more, raise an [RFI (Request for Information) request here](https://github.com/realitycollective/com.realitycollective.utilities/issues/new?assignees=&labels=question&template=request_for_information.md).

Or simply [**join us on Discord**](https://discord.gg/YjHAQD2XT8) and come chat about your questions, we would love to hear from you

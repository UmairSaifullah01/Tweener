# Tweener

## Overview
Tweener is a comprehensive Unity package that streamlines the process of creating smooth transitions and animations for game objects. It offers a diverse selection of easing functions and animation commands to modify properties like position, scale, rotation, and color.

## Key Features
- **Seamless Animations**: Effortlessly animate game object properties with customizable easing functions.
- **Versatile Commands**: Provides commands for moving, scaling, rotating, and fading UI elements.
- **Asynchronous Compatibility**: Enables asynchronous animations for enhanced performance.
- **Flexible Looping**: Supports various looping types, including linear and yoyo.

## Installation
To integrate the Tweener package into your Unity project, add the following entry to your `manifest.json` file:

```json
"com.thebaddest.tweener": "1.0.0"
```

## Quick Start Guide
Below is a simple example demonstrating how to use the Tweener package:

```csharp
using THEBADDEST.Tweening2;
using UnityEngine;

public class Example : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        // Extension methods on Transform (returns ITweener for chaining)
        target.Move(new Vector3(10, 10, 10), 2f)
            .SetEase(EaseType.OutQuad)
            .OnComplete(() => Debug.Log("Move done"));

        // Or use TweenCore for full control
        var sequence = TweenCore.Sequence()
            .Append(target.MoveTween(new Vector3(5, 0, 0), 1f))
            .AppendInterval(0.5f)
            .Append(target.ScaleTween(Vector3.one * 2f, 1f));
    }
}
```

## Available Commands
- **Move**: Transitions a game object from current (or start) position to an ending position.
- **MoveTween**: Same as Move but returns `Tween` instead of `ITweener` (use in sequences to avoid wrapper allocation).
- **Scale / ScaleTween**: Adjusts a game object from current (or start) scale to an ending scale.
- **Rotate / RotateTween**: Rotates a game object from current (or start) rotation to an ending rotation.
- **FadeImage**: Use GraphicExtensions or color tweens for UI alpha.

## Easing Functions
Tweener includes a wide range of easing functions, such as:
1. Linear
2. EaseInQuad, EaseOutQuad, EaseInOutQuad
3. InSine, OutSine, InOutSine
4. InBack, OutBack, InOutBack
5. InBounce, OutBounce, InOutBounce
6. Flash, InFlash, OutFlash, InOutFlash
7. And many more...

## Author Information
- **Name**: Umair Saifullah
- **Email**: contact@umairsaifullah.com
- **Website**: [umairsaifullah.com](https://www.umairsaifullah.com)

## License
This project is licensed under the MIT License. For more details, refer to the LICENSE file.

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
using THEBADDEST.Tweening;
using UnityEngine;

public class Example : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        ITweener tweener = new CoroutineTweener();
        tweener.Move(target, new Vector3(0, 0, 0), new Vector3(10, 10, 10), 2f);
    }
}
```

## Available Commands
- **Move**: Transitions a game object from a starting position to an ending position.
- **Scale**: Adjusts a game object from a starting scale to an ending scale.
- **Rotate**: Rotates a game object from a starting rotation to an ending rotation.
- **FadeImage**: Changes a UI image's alpha from a starting value to an ending value.

## Easing Functions
Tweener includes a wide range of easing functions, such as:
1. Linear
2. EaseInQuad
3. EaseOutQuad
4. EaseInOutQuad
5. And many more...

## Author Information
- **Name**: Umair Saifullah
- **Email**: contact@umairsaifullah.com
- **Website**: [umairsaifullah.com](https://www.umairsaifullah.com)

## License
This project is licensed under the MIT License. For more details, refer to the LICENSE file.
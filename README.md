# Tweener

## Description
Tweener is a Unity package designed to facilitate smooth transitions and animations for game objects. It provides a variety of easing functions and commands to animate properties such as position, scale, rotation, and color.

## Features
- **Smooth Animations**: Easily animate properties of game objects with customizable easing functions.
- **Multiple Commands**: Includes commands for moving, scaling, rotating, and fading UI elements.
- **Asynchronous Support**: Supports asynchronous animations for better performance.
- **Looping Options**: Offers different looping types including linear and yoyo.

## Installation
To install the Tweener package, add the following line to your `manifest.json` file in your Unity project:

```json
"com.thebaddest.tweener": "1.0.0"
``` 
## Usage
Here’s a quick example of how to use the Tweener package:
```csharp
using THEBADDEST.Tweening;
using UnityEngine;

public class Example : MonoBehaviour
{
    public Transform target;

    void Start()
    {
        ITweener tweener = new CorotineTweener();
        tweener.Move(target, new Vector3(0, 0, 0), new Vector3(10, 10, 10), 2f);
    }
}
```
## Commands
Move: Moves a game object from a start position to an end position.
Scale: Scales a game object from a start scale to an end scale.
Rotate: Rotates a game object from a start rotation to an end rotation.
FadeImage: Fades a UI image from a start alpha to an end alpha.
## Easing Functions
The Tweener package includes a variety of easing functions such as:
1. Linear
2. EaseInQuad
3. EaseOutQuad
4. EaseInOutQuad
5. And many more...

## Author
Name: Umair Saifullah
Email: contact@umairsaifullah.com
Website: umairsaifullah.com
## License
This project is licensed under the MIT License - see the LICENSE file for details.
# VR Notification System

Simple easy-to-use Notification System for your wonderful VR project.

Originally, I developed this package for [VR Boxel Editor](https://twitter.com/Volorf/status/1305406161710125056).

# How to install the package
Easy-peasy. Just 3 steps:

1. Copy Git URL
2. Open `Window/Package Manager`
3. Add VR Notifications package via `Add package from git URL`

<img src="Images/install-via-git-url.gif" width="800">


# How to add it to your project

Super simple. Find the `Notification Manager` prefab and drop it into your scene.

<img src="Images/add-to-scene.gif" width="800">


# How to send a notification

Personally, I love binding the `SendMessage(<T> m)` methods to UnityEvents to make it decoupled as mush as possible.

```csharp
using System;
using UnityEngine.Events;

// Create a custom Unity Event
[Serializable]
public class NotificationEvent: UnityEvent<string> {}
```

```csharp
using UnityEngine;

public class MyScript : MonoBehaviour
{
    // Expose the custom event in Editor
    [SerializeField] 
    private NotificationEvent MyEvent;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MyEvent.Invoke("Space has been pressed.");
        }
    }
}
```

<img src="Images/coonect-to-button.gif" width="800">

However, since `Notification Manager` object is `Singleton` and its methods are static, you can call them without having a direct reference to the object. Very straightforward:

```csharp
private void Start()
{
    NotificationManager.Instance.SendMessage("Hello World!");
}
```


# How to set up it
All properties are in a Scriptable Object Asset called "Notification Settings". 

I found it very useful especially when you tweak values during the design iterations without recomplining the script each time you made changes and you can store diffrent versions of the values while you do the design experiments.

To create `Notification Settings`, go to `Create` / `Create Notification Settings`.

<img src="Images/create-settings.gif" width="800">
 
 Then drop the asset to the `Notification Manager`.

<img src="Images/add-settings.gif" width="800">

# Links
[Portfolio](https://olegfrolov.design/) | [Linkedin](https://www.linkedin.com/in/oleg-frolov-6a6a4752/) | [Dribbble](https://dribbble.com/Volorf) | [Twitter](https://www.twitter.com/volorf) 



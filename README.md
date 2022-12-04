# VR Notification System

Simple easy-to-use Notification System for your wonderful VR project.

Originally, this pacages was developed for [VR Boxel Editor](https://twitter.com/Volorf/status/1305406161710125056).

## How to install the package

Just 2 steps:

### 1. Copy Git URL
### 2. Open ```Window/Package Manager``` and paste the URL

## How to add VR Notification to your project
Super simple. Find the ```VR Notification``` prefab and drop it into your scene.

## How to send a notification
Personally, I love binding the ```SendMessage(<T> m)``` methods to UnityEvents that exposed in Editor to make it decoupled as mush as possible. Like this:

But since VR Notification object is ```Singleton``` and its methods are static, you can call them without having a direct reference to the object. ~~Single~~ Simple like this:
```csharp
Notification.Instance.SendMessage("Hello World!");
```

## How to set up it


## Contacts



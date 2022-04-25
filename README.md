# Futurus Remote Input

A Unity Package for interacting world-space UI from a world-space remote, by Futurus

## Use

Create an `EventSystem` component somewhere in your project, ideally in the first scene so it persists. Remove the default `StandaloneInputModule` component and add the `RemoteInputModule` component to replace it. 

When you make new Canvases, remove any `GraphicsRaycasters` and replace with the `RemoteInputRaycaster`.

Last, write a script that suits your project requirements that implements `IRemoteInputProvider`. Ensure you register your implementing class with the `RemoteInputModule`, which you can get a reference to the `RemoteInputModule` with the by routing through `EventSystem.current.currentInputModule as RemoteInputModule`

## Importing into a project

The manifest may be found at `your-project/Packages/manifest.json`

`"com.futurus.remoteinput": "git@github.com:FuturusDev/FuturusRemoteInput.git"` may be added to dependencies of your project in the `manifest.json`.

Unity will automatically download the latest version of the repo into your project as a package separate from your Assets folder.

## Unit Testing

Unit Testing is provided by Unity's Test Runner system. Tests are held within the `Futurus.RemoteInput.Tests` and `Futurus.RemoteInput.Editor.Tests` namespaces. If you want to perform the Unit Tests in your project, include the following in your manifest after the dependencies.

    {
        "dependencies": {
            "com.futurus.remoteinput": "git@github.com:FuturusDev/FuturusRemoteInput.git",
            ...
        },
        "testables": ["com.futurus.remoteinput"]
    }

Refer to the [Unity Package Testing Documentation](https://docs.unity3d.com/Manual/cus-tests.html)  for more details

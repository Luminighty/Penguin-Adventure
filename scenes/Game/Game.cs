using Godot;
using System;

public class Game : Node
{

    [Export]
    public NodePath defaultScene;
    [Export(PropertyHint.File, "*.tscn,*.scn")]
    public string defaultScenePath;

    public string scenePath;
    public Node currentScene;

    public override void _Ready() {
        if (defaultScene != null) {
            currentScene = GetNode(defaultScene);
            scenePath = defaultScenePath;
        } else {
            GD.PushWarning("No default scene was defined");
        }
    }

    public void ChangeScene(string path) {
        CallDeferred(nameof(DeferredChangeScene), path);
    }

    private void DeferredChangeScene(string path) {
        RemoveChild(currentScene);
        currentScene.Free();

        scenePath = path;
        PackedScene sceneResource = ResourceLoader.Load<PackedScene>(path);
        currentScene = sceneResource.Instance();
        AddChild(currentScene);
    }

}

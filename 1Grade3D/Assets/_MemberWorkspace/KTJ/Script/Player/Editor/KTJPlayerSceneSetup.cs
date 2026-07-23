#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class KTJPlayerSceneSetup
{
    private const string ScenePath =
        "Assets/_MemberWorkspace/KTJ/Scene/01_KTJ_Start.unity";
    private const string InputAssetPath =
        "Assets/_MemberWorkspace/KTJ/Asset/Input/PlayerInput.asset";

    public static void Setup()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject player = FindPlayer(scene);

        if (player == null)
        {
            throw new System.InvalidOperationException(
                "01_KTJ_Start scene에서 Player 오브젝트를 찾지 못했습니다.");
        }

        PlayerInputSO inputAsset = CreateOrLoadInputAsset();

        Rigidbody rigidbodyComponent = GetOrAddComponent<Rigidbody>(player);
        rigidbodyComponent.mass = 1f;
        rigidbodyComponent.useGravity = true;
        rigidbodyComponent.isKinematic = false;
        rigidbodyComponent.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbodyComponent.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rigidbodyComponent.constraints = RigidbodyConstraints.FreezeRotation;

        AgentMover mover = GetOrAddComponent<AgentMover>(player);
        mover.SetPlayerInput(inputAsset);
        Transform visual = player.transform.Find("Visual");
        Transform body = player.transform.Find("Visual/Body");
        Transform particle = player.transform.Find("MoveParticle");

        SerializedObject moverObject = new(mover);
        moverObject.FindProperty("playerInput").objectReferenceValue = inputAsset;
        moverObject.FindProperty("moveSpeed").floatValue = 5f;
        moverObject.FindProperty("bodySpriteRenderer").objectReferenceValue =
            body == null ? null : body.GetComponent<SpriteRenderer>();
        moverObject.FindProperty("moveParticle").objectReferenceValue =
            particle == null ? null : particle.GetComponent<ParticleSystem>();
        moverObject.FindProperty("directionRotationTargets").arraySize = 0;
        moverObject.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(mover);

        PlayerStateMachine stateMachine = GetOrAddComponent<PlayerStateMachine>(player);
        SerializedObject stateMachineObject = new(stateMachine);
        stateMachineObject.FindProperty("agentMover").objectReferenceValue = mover;
        stateMachineObject.FindProperty("animator").objectReferenceValue =
            visual == null ? null : visual.GetComponent<Animator>();
        stateMachineObject.FindProperty("animationTransitionDuration").floatValue = 0.1f;
        stateMachineObject.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(player);
        EditorUtility.SetDirty(mover);
        EditorUtility.SetDirty(stateMachine);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();

        Debug.Log("KTJ Player FSM setup completed.");
    }

    private static PlayerInputSO CreateOrLoadInputAsset()
    {
        AssetDatabase.ImportAsset(InputAssetPath, ImportAssetOptions.ForceUpdate);
        PlayerInputSO asset = AssetDatabase.LoadMainAssetAtPath(InputAssetPath) as PlayerInputSO;

        if (asset != null)
        {
            return asset;
        }

        const string inputFolder = "Assets/_MemberWorkspace/KTJ/Asset/Input";

        if (!AssetDatabase.IsValidFolder(inputFolder))
        {
            AssetDatabase.CreateFolder("Assets/_MemberWorkspace/KTJ/Asset", "Input");
        }

        asset = ScriptableObject.CreateInstance<PlayerInputSO>();
        AssetDatabase.CreateAsset(asset, InputAssetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(InputAssetPath, ImportAssetOptions.ForceUpdate);

        asset = AssetDatabase.LoadMainAssetAtPath(InputAssetPath) as PlayerInputSO;

        if (asset == null)
        {
            throw new System.InvalidOperationException(
                "PlayerInputSO 에셋을 생성하거나 불러오지 못했습니다.");
        }

        return asset;
    }

    private static GameObject FindPlayer(Scene scene)
    {
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            if (rootObject.CompareTag("Player") || rootObject.name == "Player")
            {
                return rootObject;
            }
        }

        return null;
    }

    private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        return component == null ? gameObject.AddComponent<T>() : component;
    }
}
#endif

using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SoundSampleSetup
{
    private const string SoundFolder = "Assets/_MemberWorkspace/KTJ/Asset/Sound";
    private const string MixerPath = SoundFolder + "/GameAudioMixer.mixer";
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    [MenuItem("Tools/KTJ/Setup Sound Sample")]
    public static void Setup()
    {
        EnsureFolder("Assets/_MemberWorkspace/KTJ/Library/Sound/Editor");
        EnsureFolder(SoundFolder);

        AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
        if (mixer == null)
            mixer = CreateMixerThroughUnityMenu();

        object mixerController = mixer;
        object startSnapshot = GetProperty(mixerController, "startSnapshot");
        if (startSnapshot == null)
            throw new InvalidOperationException("Audio Mixer has no valid Default Snapshot. Scene setup was not changed.");

        object masterController = GetProperty(mixerController, "masterGroup");
        if (masterController == null)
            throw new InvalidOperationException("Audio Mixer has no valid Master group. Scene setup was not changed.");
        ((UnityEngine.Object)masterController).name = "Master";
        object sfxController = FindOrCreateGroup(mixerController, masterController, "SFX");
        object bgmController = FindOrCreateGroup(mixerController, masterController, "BGM");
        ExposeVolume(mixerController, masterController, "MasterVolume");
        ExposeVolume(mixerController, sfxController, "SFXVolume");
        ExposeVolume(mixerController, bgmController, "BGMVolume");
        AudioMixerGroup sfx = (AudioMixerGroup)sfxController;
        EditorUtility.SetDirty((UnityEngine.Object)mixerController);
        AssetDatabase.SaveAssets();

        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject oldRoot = GameObject.Find("Sound Sample");
        if (oldRoot != null)
            UnityEngine.Object.DestroyImmediate(oldRoot);

        GameObject root = new GameObject("Sound Sample", typeof(AudioSource), typeof(SoundSampleController));
        AudioSource source = root.GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = sfx;

        Canvas canvas = CreateCanvas(root.transform);
        Slider masterSlider = CreateSlider(canvas.transform, "Master", new Vector2(0f, 90f), out Text masterValue);
        Slider sfxSlider = CreateSlider(canvas.transform, "SFX", new Vector2(0f, 20f), out Text sfxValue);
        Slider bgmSlider = CreateSlider(canvas.transform, "BGM", new Vector2(0f, -50f), out Text bgmValue);
        CreateHelpText(canvas.transform);

        if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            SceneManager.MoveGameObjectToScene(eventSystem, scene);
        }

        SoundSampleController controller = root.GetComponent<SoundSampleController>();
        SerializedObject serialized = new SerializedObject(controller);
        serialized.FindProperty("mixer").objectReferenceValue = mixer;
        serialized.FindProperty("sfxGroup").objectReferenceValue = sfx;
        serialized.FindProperty("testSfx").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>(SoundFolder + "/ClickSound.mp3");
        serialized.FindProperty("masterSlider").objectReferenceValue = masterSlider;
        serialized.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
        serialized.FindProperty("bgmSlider").objectReferenceValue = bgmSlider;
        serialized.FindProperty("masterValueText").objectReferenceValue = masterValue;
        serialized.FindProperty("sfxValueText").objectReferenceValue = sfxValue;
        serialized.FindProperty("bgmValueText").objectReferenceValue = bgmValue;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("KTJ sound mixer and sample scene setup completed.");
    }

    private static AudioMixer CreateMixerThroughUnityMenu()
    {
        Type controllerType = FindEditorType("UnityEditor.Audio.AudioMixerController");
        MethodInfo factory = controllerType.GetMethod("CreateMixerControllerAtPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        if (factory == null)
            throw new MissingMethodException(controllerType.FullName, "CreateMixerControllerAtPath");
        factory.Invoke(null, new object[] { MixerPath });
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        AudioMixer mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(MixerPath);
        if (mixer == null)
            throw new InvalidOperationException("Unity Audio Mixer factory did not create an asset.");
        return mixer;
    }

    public static void RecoverBrokenSetup()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        GameObject soundSample = GameObject.Find("Sound Sample");
        if (soundSample != null)
            UnityEngine.Object.DestroyImmediate(soundSample);

        EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<EventSystem>();
        if (eventSystem != null && eventSystem.name == "EventSystem" && eventSystem.GetComponent<InputSystemUIInputModule>() != null)
            UnityEngine.Object.DestroyImmediate(eventSystem.gameObject);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("KTJ broken sound sample setup recovered.");
    }

    private static object FindOrCreateGroup(object mixer, object master, string name)
    {
        IEnumerable groups = (IEnumerable)Invoke(mixer, "GetAllAudioGroupsSlow");
        foreach (object group in groups)
            if (((UnityEngine.Object)group).name == name)
                return group;

        object created = Invoke(mixer, "CreateNewGroup", name, false);
        Invoke(mixer, "AddChildToParent", created, master);
        return created;
    }

    private static void ExposeVolume(object mixer, object group, string parameter)
    {
        object guid = Invoke(group, "GetGUIDForVolume");
        if (!(bool)Invoke(mixer, "ContainsExposedParameter", guid))
        {
            object path = CreateAudioParameterPath(group, guid);
            Invoke(mixer, "AddExposedParameter", path);
        }

        RenameExposedParameter(mixer, guid, parameter);
    }

    private static object CreateAudioParameterPath(object group, object guid)
    {
        Type pathType = FindEditorType("UnityEditor.Audio.AudioParameterPath");
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try { types = assembly.GetTypes(); }
            catch (ReflectionTypeLoadException exception) { types = exception.Types; }
            foreach (Type concreteType in types)
            {
                if (concreteType == null || concreteType.IsAbstract || !pathType.IsAssignableFrom(concreteType))
                    continue;
                foreach (ConstructorInfo constructor in concreteType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    ParameterInfo[] parameters = constructor.GetParameters();
                    object[] values = new object[parameters.Length];
                    bool compatible = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Type type = parameters[i].ParameterType;
                        if (type.IsInstanceOfType(group)) values[i] = group;
                        else if (type.IsInstanceOfType(guid)) values[i] = guid;
                        else if (!type.IsValueType) values[i] = null;
                        else { compatible = false; break; }
                    }
                    if (compatible)
                        return constructor.Invoke(values);
                }
            }
        }
        throw new MissingMethodException(pathType.FullName, ".ctor");
    }

    private static void RenameExposedParameter(object mixer, object guid, string name)
    {
        Array parameters = (Array)GetProperty(mixer, "exposedParameters");
        for (int i = 0; i < parameters.Length; i++)
        {
            object item = parameters.GetValue(i);
            object itemGuid = GetMember(item, "guid", "guid");
            if (!itemGuid.Equals(guid))
                continue;
            SetMember(item, "name", "name", name);
            parameters.SetValue(item, i);
            SetMember(mixer, "exposedParameters", "m_ExposedParameters", parameters);
            return;
        }
    }

    private static object GetMember(object target, string propertyName, string fieldName)
    {
        Type type = target.GetType();
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null) return property.GetValue(target);
        FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return field?.GetValue(target);
    }

    private static object GetProperty(object target, string name)
    {
        return target.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(target);
    }

    private static void SetMember(object target, string propertyName, string fieldName, object value)
    {
        Type type = target.GetType();
        PropertyInfo property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != null && property.SetMethod != null)
        {
            property.SetValue(target, value);
            return;
        }

        FieldInfo field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
            throw new MissingFieldException(type.FullName, fieldName);
        field.SetValue(target, value);
    }

    private static Type FindEditorType(string fullName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType(fullName, false);
            if (type != null)
                return type;
        }

        throw new TypeLoadException(fullName);
    }

    private static object Invoke(object target, string name, params object[] arguments)
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (MethodInfo method in methods)
        {
            if (method.Name == name && method.GetParameters().Length == arguments.Length)
                return method.Invoke(target, arguments);
        }

        throw new MissingMethodException(target.GetType().FullName, name);
    }

    private static bool TryInvoke(object target, string name, params object[] arguments)
    {
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (MethodInfo method in methods)
        {
            if (method.Name == name && method.GetParameters().Length == arguments.Length)
            {
                try
                {
                    method.Invoke(target, arguments);
                    return true;
                }
                catch (ArgumentException)
                {
                    // Same-arity overload with incompatible parameter types.
                }
            }
        }

        return false;
    }

    private static Canvas CreateCanvas(Transform parent)
    {
        GameObject go = new GameObject("Sound Volume UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.transform.SetParent(parent, false);
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        return canvas;
    }

    private static Slider CreateSlider(Transform parent, string label, Vector2 position, out Text valueText)
    {
        GameObject row = CreateUiObject(label + " Volume", parent, new Vector2(650f, 55f), position);
        Image background = row.AddComponent<Image>();
        background.color = new Color(0.08f, 0.1f, 0.14f, 0.92f);

        Text labelText = CreateText(row.transform, label, new Vector2(130f, 45f), new Vector2(-245f, 0f), TextAnchor.MiddleLeft);
        labelText.fontSize = 24;
        GameObject sliderGo = CreateUiObject("Slider", row.transform, new Vector2(380f, 35f), new Vector2(15f, 0f));
        Slider slider = sliderGo.AddComponent<Slider>();
        Image sliderBackground = sliderGo.AddComponent<Image>();
        sliderBackground.color = new Color(0.22f, 0.25f, 0.32f, 1f);
        slider.targetGraphic = sliderBackground;

        GameObject fill = CreateUiObject("Fill", sliderGo.transform, new Vector2(360f, 21f), Vector2.zero);
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.15f, 0.7f, 1f, 1f);
        slider.fillRect = fill.GetComponent<RectTransform>();

        GameObject handle = CreateUiObject("Handle", sliderGo.transform, new Vector2(28f, 42f), Vector2.zero);
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;

        valueText = CreateText(row.transform, "100", new Vector2(70f, 45f), new Vector2(285f, 0f), TextAnchor.MiddleCenter);
        valueText.fontSize = 24;
        return slider;
    }

    private static void CreateHelpText(Transform parent)
    {
        Text text = CreateText(parent, "Sound Test  |  Press T to play ClickSound (SFX)", new Vector2(700f, 50f), new Vector2(0f, 170f), TextAnchor.MiddleCenter);
        text.fontSize = 27;
    }

    private static GameObject CreateUiObject(string name, Transform parent, Vector2 size, Vector2 position)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return go;
    }

    private static Text CreateText(Transform parent, string content, Vector2 size, Vector2 position, TextAnchor alignment)
    {
        GameObject go = CreateUiObject(content + " Text", parent, size, position);
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.color = Color.white;
        text.alignment = alignment;
        return text;
    }

    private static void EnsureFolder(string path)
    {
        string[] parts = path.Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
}

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer), typeof(CapsuleCollider2D))]
class SlimeColliderMapper : MonoBehaviour
{
    [SerializeField] private SlimeDatabase _database;
    [SerializeField] private SlimeDatabase.SlimeType _level;

#if UNITY_EDITOR
    [ContextMenu("Collider/Load Level Preview")]
    private void LoadLevelPreview()
    {
        if (!TryGetData(out SlimeData data)) return;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        UnityEditor.Undo.RecordObjects(
            new Object[] { spriteRenderer, capsule, transform },
            "Load Slime Collider Preview");

        spriteRenderer.sprite = data.Sprite;
        transform.localScale = new Vector3(data.Scale, data.Scale, 1f);

        Bounds bounds = data.Sprite.bounds;
        Vector2 size = data.HasColliderData ? data.ColliderSize : bounds.size;
        capsule.size = size;
        capsule.offset = data.HasColliderData
            ? data.ColliderOffset
            : bounds.center;
        capsule.direction = data.HasColliderData
            ? data.ColliderDirection
            : size.x >= size.y
                ? CapsuleDirection2D.Horizontal
                : CapsuleDirection2D.Vertical;

        UnityEditor.EditorUtility.SetDirty(gameObject);
    }

    [ContextMenu("Collider/Save Collider To Database")]
    private void SaveColliderToDatabase()
    {
        if (!ResolveDatabase()) return;

        int levelIndex = (int)_level;
        UnityEditor.SerializedObject serializedDatabase =
            new UnityEditor.SerializedObject(_database);
        UnityEditor.SerializedProperty slimeDatas =
            serializedDatabase.FindProperty("_slimeDatas");

        if (slimeDatas == null || levelIndex >= slimeDatas.arraySize)
        {
            Debug.LogError($"Slime data level {levelIndex} does not exist.", this);
            return;
        }

        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        UnityEditor.Undo.RecordObject(_database, "Save Slime Collider Data");
        serializedDatabase.Update();

        UnityEditor.SerializedProperty data =
            slimeDatas.GetArrayElementAtIndex(levelIndex);
        data.FindPropertyRelative("_colliderSize").vector2Value = capsule.size;
        data.FindPropertyRelative("_colliderOffset").vector2Value = capsule.offset;
        data.FindPropertyRelative("_colliderDirection").enumValueIndex =
            (int)capsule.direction;

        serializedDatabase.ApplyModifiedProperties();
        UnityEditor.EditorUtility.SetDirty(_database);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log($"Saved collider for level {levelIndex} ({_level}).", _database);
    }

    private bool TryGetData(out SlimeData data)
    {
        data = default;
        if (!ResolveDatabase()) return false;

        int levelIndex = (int)_level;
        if (levelIndex < 0 || levelIndex >= _database.SlimeDatas.Length)
        {
            Debug.LogError($"Slime data level {levelIndex} does not exist.", this);
            return false;
        }

        data = _database.SlimeDatas[levelIndex];
        if (data.Sprite != null) return true;

        Debug.LogError($"Level {levelIndex} has no sprite.", this);
        return false;
    }

    private bool ResolveDatabase()
    {
        if (_database == null)
            _database = Resources.Load<SlimeDatabase>("Data/SlimeDatabase");

        if (_database != null) return true;

        Debug.LogError("SlimeDatabase was not found.", this);
        return false;
    }
#endif
}

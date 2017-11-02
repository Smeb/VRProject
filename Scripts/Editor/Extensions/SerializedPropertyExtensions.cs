using UnityEngine;
using UnityEditor;

public static class SerializedPropertyExtensions
{
    public static void AddToObjectArray<T> (this SerializedProperty arrayProperty, T elementToAdd)
        where T : Object
    {
        if (!arrayProperty.isArray)
            SerializedPropertyError(arrayProperty, "is not an array.");
        arrayProperty.serializedObject.Update();

        arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
        arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = elementToAdd;

        arrayProperty.serializedObject.ApplyModifiedProperties();
    }

    public static void RemoveFromObjectArrayAt(this SerializedProperty arrayProperty, int index)
    {
        if (index < 0)
            SerializedPropertyError(arrayProperty, " cannot have negative elements removed.");

        if (!arrayProperty.isArray)
            SerializedPropertyError(arrayProperty, " is not an array.");

        if (index < arrayProperty.arraySize - 1)
            SerializedPropertyError(arrayProperty, " has only " + arrayProperty.arraySize + " elements so element " + index + " cannot be removed.");

        arrayProperty.serializedObject.Update();

        if (arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue)
            arrayProperty.DeleteArrayElementAtIndex(index);

        arrayProperty.DeleteArrayElementAtIndex(index);

        arrayProperty.serializedObject.ApplyModifiedProperties();
    }

    public static void RemoveFromObjectArray<T> (this SerializedProperty arrayProperty, T elementToRemove)
        where T : Object
    {
        if (!arrayProperty.isArray)
            SerializedPropertyError(arrayProperty, " is not an array.");

        if (!elementToRemove)
            throw new UnityException("Removing a null elemnt is not supported using this method.");

        arrayProperty.serializedObject.Update();

        for(int i = 0; i < arrayProperty.arraySize; i++)
        {
            SerializedProperty elementProperty = arrayProperty.GetArrayElementAtIndex(i);

            if(elementProperty.objectReferenceValue == elementToRemove)
            {
                arrayProperty.RemoveFromObjectArrayAt(i);
                return;
            }
        }
    }

    private static void SerializedPropertyError(this SerializedProperty arrayProperty, string message)
    {
        throw new UnityException("SerializedProperty " + arrayProperty.name + message);
    }
}

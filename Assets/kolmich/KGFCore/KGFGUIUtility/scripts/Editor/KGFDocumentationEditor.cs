using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(KGFDocumentation))]
public class KGFDocumentationEditor : Editor
{
	public void OnEnable()
	{
		KGFDocumentation aDocumentation = target as KGFDocumentation;
		aDocumentation.OpenDocumentation();
	}
}

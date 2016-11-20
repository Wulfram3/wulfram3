using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ReadBitmap : MonoBehaviour {

	// Use this for initialization
	void Start () {
        byte[] data = loadBytes("loading_screen2");
        //Texture2D tex = new Texture2D(1, 1);
        Texture2D tex = new Texture2D(640, 480);
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        
        //Texture2D tex = renderer.sprite.texture;
        for (int y = 0; y < 480; y++) {
            for (int x = 0; x < 640; x++) {
                byte col = data[y * 640 + x + 9];
                float c = 1f - col / 255f;
                float r = c;
                float g = c;
                float b = c;
                tex.SetPixel(x, 480 - y, new Color(r, g, b));
            }
        }
        tex.Apply();
        renderer.sprite = Sprite.Create(tex, new Rect(0, 0, 640, 480), new Vector2(0.5f, 0.5f));

        //GetComponent<MeshRenderer>().material.mainTexture = tex;


    }

    // Update is called once per frame
    void Update () {
	
	}

    public byte[] loadBytes(string fileName) {
        TextAsset data = Resources.Load(fileName) as TextAsset;
        return data.bytes;
    }
}

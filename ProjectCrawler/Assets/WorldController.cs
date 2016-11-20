using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour
{
    public int width = 32;
    public int height = 32;

    public GameObject[,] tiles;

    Dictionary<string, Sprite> groundSprites;

    public void Start()
    {
        tiles = new GameObject[width, height];

        Sprite[] sprites = Resources.LoadAll<Sprite>("mudland_floor");

        groundSprites = new Dictionary<string, Sprite>();

        foreach (Sprite sprite in sprites)
        { 
            groundSprites.Add(sprite.name, sprite);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                tiles[x, y] = new GameObject(
                    string.Format("tile_{0}_{1}", x, y),
                    typeof(SpriteRenderer));

                tiles[x, y].GetComponent<SpriteRenderer>().sprite =
                    groundSprites["mudland_floor_" + ((8 * (7 - (y % 8))) + (x % 8)).ToString()];

                tiles[x, y].transform.position = new Vector2(x, y);
            }
        }
    }
}
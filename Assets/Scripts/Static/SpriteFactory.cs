using UnityEngine;
using UnityEngine.U2D;
public class SpriteFactory : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas spriteAtlas;
    static SpriteAtlas _spriteAtlas;
    private void Awake()
    {
        _spriteAtlas = spriteAtlas;
    }
    public static Sprite GetSprite(string SpriteName)
    {
        return _spriteAtlas.GetSprite(SpriteName);
    }
}

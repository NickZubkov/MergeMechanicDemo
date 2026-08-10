using UnityEngine;

namespace MergeMechanic.Presentation
{
    /// <summary>Белый квадрат размером в один юнит — чтобы не заводить арт-ассеты ради заглушки.</summary>
    public static class DefaultSprite
    {
        private static Sprite _square;

        public static Sprite Square
        {
            get
            {
                if (_square == null)
                {
                    Texture2D texture = Texture2D.whiteTexture;
                    _square = Sprite.Create(
                        texture,
                        new Rect(0f, 0f, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f),
                        texture.width);
                }

                return _square;
            }
        }
    }
}

using MergeMechanic.Configs;
using MergeMechanic.Domain;
using TMPro;
using UnityEngine;
using Zenject;

namespace MergeMechanic.Presentation
{
    public class BoardObjectView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private TextMeshPro _label;
        [SerializeField] private MeshRenderer _labelRenderer;
        [SerializeField] private int _normalSortingOrder = 10;
        [SerializeField] private int _draggingSortingOrder = 20;

        public int Id { get; private set; }

        public void Bind(BoardObject boardObject)
        {
            Id = boardObject.Id;

            ChainLevel level = boardObject.LevelData;
            _renderer.sprite = level.Sprite != null ? level.Sprite : DefaultSprite.Square;
            _renderer.color = level.Color;
            _label.text = level.DisplayName;

            SetDragging(false);
        }

        public void SetDragging(bool dragging)
        {
            int order = dragging ? _draggingSortingOrder : _normalSortingOrder;
            _renderer.sortingOrder = order;
            _labelRenderer.sortingOrder = order + 1;
        }

        public class Factory : PlaceholderFactory<BoardObjectView>
        {
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project
{
    public class Drawing : MonoBehaviour
    {
        [Header("Body")]
        [SerializeField] private SpriteRenderer face;
        [SerializeField] private SpriteRenderer head;
        [SerializeField] private SpriteRenderer body;
        [SerializeField] private SpriteRenderer limbs;
        [Header("Pencils")]
        public bool canDraw;
        [SerializeField] private GameObject stroke;
        [SerializeField] private Gradient currentGradient;
        [SerializeField] private LayerMask targetLayer;
        LineRenderer currentLineRenderer;
        List<GameObject> strokeHistory = new();
        Vector2 lastPos;
        Transform strokesParent;
        [Header("Frame")]
        [SerializeField] private Image colorPicker;
        [SerializeField] private Image pauseButton;
        [SerializeField] private Image undoButton;
        [SerializeField] private Image[] itemPickers;
        [SerializeField] private Image[] characterPreviews;
        [SerializeField] private Image[] confirmButtons;
        [SerializeField] private Frame[] frameThemes;

        private void Update()
        {
            if (!canDraw) return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!Physics2D.Raycast(mousePos, Vector3.forward, 100, targetLayer)) return;

            Draw();
        }

        public void EnableDrawing(bool enable)
        {
            canDraw = enable;
        }

        private void Draw()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                CreateStroke();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                PointToMousePos();
            }
            else
            {
                currentLineRenderer = null;
            }
        }

        private void CreateStroke()
        {
            if (strokesParent == null) strokesParent = new GameObject("Strokes").transform;

            GameObject strokeInstance = Instantiate(stroke, strokesParent);
            strokeHistory.Add(strokeInstance);
            currentLineRenderer = strokeInstance.GetComponent<LineRenderer>();
            currentLineRenderer.colorGradient = currentGradient;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            currentLineRenderer.SetPosition(0, mousePos);
            currentLineRenderer.SetPosition(1, mousePos);

        }

        public void Undo()
        {
            if (strokeHistory.Count == 0) return;

            Destroy(strokeHistory[strokeHistory.Count - 1]);
            strokeHistory.RemoveAt(strokeHistory.Count - 1);
        }

        private void AddAPoint(Vector2 pointPos)
        {
            currentLineRenderer.positionCount++;
            int positionIndex = currentLineRenderer.positionCount - 1;
            currentLineRenderer.SetPosition(positionIndex, pointPos);
        }

        private void PointToMousePos()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (lastPos != mousePos)
            {
                AddAPoint(mousePos);
                lastPos = mousePos;
            }
        }

        public void SetPencilColor(Image image)
        {
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(image.color, 1f);
            colorKeys[1] = new GradientColorKey(image.color, 1f);
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1f, 0f);
            alphaKeys[1] = new GradientAlphaKey(1f, 0f);
            currentGradient.SetKeys(colorKeys, alphaKeys);
        }

        public void SetHeadShape(Image buttonImage)
        {
            head.sprite = buttonImage.sprite;
        }

        public void SetFace(Image buttonImage)
        {
            face.sprite = buttonImage.sprite;
        }

        public void SetBody(Image buttonImage)
        {
            body.sprite = buttonImage.sprite;
        }

        public void SetSkinColor(Image buttonImage)
        {
            head.color = buttonImage.color;
            limbs.color = buttonImage.color;
        }

        public void SetWoodTheme()
        {
            SetFrameTheme(0);
        }

        public void SetBlueTheme()
        {
            SetFrameTheme(1);
        }

        public void SetPinkTheme()
        {
            SetFrameTheme(2);
        }

        public void SetFrameTheme(int frameIndex)
        {
            for (int i = 0; i < itemPickers.Length; i++)
            {
                itemPickers[i].sprite = frameThemes[frameIndex].itemPicker;
            }
            for (int i = 0; i < characterPreviews.Length; i++)
            {
                characterPreviews[i].sprite = frameThemes[frameIndex].characterPreview;
            }
            for (int i = 0; i < confirmButtons.Length; i++)
            {
                confirmButtons[i].sprite = frameThemes[frameIndex].confirmButton;
            }
            colorPicker.sprite = frameThemes[frameIndex].colorPicker;
            pauseButton.sprite = frameThemes[frameIndex].pauseButton;
            undoButton.sprite = frameThemes[frameIndex].undoButton;
        }

        [Serializable]
        public class Frame
        {
            public string name;
            public Sprite characterPreview;
            public Sprite colorPicker;
            public Sprite itemPicker;
            public Sprite confirmButton;
            public Sprite pauseButton;
            public Sprite undoButton;
        }
    }
}

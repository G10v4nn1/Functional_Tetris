using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace simpleUI
{
    public class MainCamera
    {
        public Camera camera = new GameObject().AddComponent<Camera>();

        public MainCamera()
        {
        }
    }

    //The job of this object is to render the UI
    public class GameRender
    {
        public Canvas canvas = new GameObject().AddComponent<Canvas>();

        public RectTransform rect => canvas.GetComponent<RectTransform>();

        public GameRender()
        {
            canvas.name = "Canvas";
            canvas.worldCamera = new MainCamera().camera;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            rect.position = new Vector3(0, 0, 8.5f);
        }
    }

    public class UIPanel
    {
        public Image panel = new GameObject().AddComponent<Image>();

        public RectTransform rect => panel.rectTransform;

        public Vector2 size => rect.sizeDelta;

        public (int cols, int rows) matrixDimentions;

        public Vector2 matrixUnit => new Vector2(size.x / matrixDimentions.cols, size.y / matrixDimentions.rows);

        public Vector2[,] locationsMatrix;

        public UIPanel(Sprite _image)
        {
            panel.sprite = _image;
        }

        public UIPanel setParent(RectTransform parent)
        {
            rect.parent = parent;
            rect.position = new Vector3(rect.position.x, rect.position.y, parent.position.z);
            return this;
        }

        public UIPanel setAnchor(Vector2 min, Vector2 max)
        {
            rect.localScale = new Vector3(1, 1, 1);
            rect.anchorMax = max;
            rect.anchorMin = min;
            return this;
        }

        public UIPanel setAnchorOffset(Vector2 min, Vector2 max)
        {
            rect.offsetMax = max;
            rect.offsetMin = min;
            return this;
        }

        public UIPanel setAnchorBorders(float border)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x - border, rect.offsetMax.y - border);
            rect.offsetMin = new Vector2(rect.offsetMin.x + border, rect.offsetMin.y + border);
            return this;
        }

        public UIPanel setAnchorBorders(float top, float border)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x - border, rect.offsetMax.y - top);
            rect.offsetMin = new Vector2(rect.offsetMin.x + border, rect.offsetMin.y + border);
            return this;
        }

        public UIPanel setAnchorBorders(float top, float left, float border)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x - border, rect.offsetMax.y - top);
            rect.offsetMin = new Vector2(rect.offsetMin.x + left, rect.offsetMin.y + border);
            return this;
        }

        public UIPanel setAnchorBorders(float top, float left, float down, float right)
        {
            rect.offsetMax = new Vector2(rect.offsetMax.x - right, rect.offsetMax.y - top);
            rect.offsetMin = new Vector2(rect.offsetMin.x + left, rect.offsetMin.y + down);
            return this;
        }

        public UIPanel setVectorSize(Vector2 _sizeDelta)
        {
            if(rect.anchorMin != rect.anchorMax)
            {
                rect.anchorMin = rect.anchorMax;
            }
            rect.localScale = new Vector3(1, 1, 1);
            rect.sizeDelta = _sizeDelta;
            return this;
        }

        public UIPanel setVectorLocation(Vector2 _location)
        {
            rect.localPosition = _location;
            return this;
        }

        public UIPanel createMatrix(int cols, int rows)
        {
            matrixDimentions = (cols, rows);
            locationsMatrix = new Vector2[matrixDimentions.cols, matrixDimentions.rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    locationsMatrix[c, r] = new Vector2(-(size.x / 2) + (matrixUnit.x * c) + (matrixUnit.x / 2), -(size.y / 2) + (matrixUnit.y * r) + (matrixUnit.y / 2));
                }
            }
            return this;
        }
    }
}

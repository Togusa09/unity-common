using UnityEngine;
using UnityEngine.UIElements;

namespace Anarks2.UnityCommon.Gameplay.HUD.Crosshair
{
    [UxmlElement]
    public partial class VectorCrosshair : VisualElement
    {
        private float _spread = 10f;
        private float _lineLength = 12f;
        private float _lineThickness = 2.5f;
        private float _outlineThickness = 1.5f;
        private float _centerDotRadius = 2f;
        private float _ringRadius = 0f;
        private Color _color = Color.white;
        private Color _outlineColor = new Color(0f, 0f, 0f, 0.75f);
        private bool _showCenterDot = true;
        private bool _showOutline = true;
        private bool _useRoundCaps = false;

        [UxmlAttribute("spread")]
        public float Spread
        {
            get => _spread;
            set { _spread = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("line-length")]
        public float LineLength
        {
            get => _lineLength;
            set { _lineLength = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("line-thickness")]
        public float LineThickness
        {
            get => _lineThickness;
            set { _lineThickness = Mathf.Max(0.5f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("outline-thickness")]
        public float OutlineThickness
        {
            get => _outlineThickness;
            set { _outlineThickness = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("center-dot-radius")]
        public float CenterDotRadius
        {
            get => _centerDotRadius;
            set { _centerDotRadius = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("ring-radius")]
        public float RingRadius
        {
            get => _ringRadius;
            set { _ringRadius = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        [UxmlAttribute("color")]
        public Color Color
        {
            get => _color;
            set { _color = value; MarkDirtyRepaint(); }
        }

        [UxmlAttribute("outline-color")]
        public Color OutlineColor
        {
            get => _outlineColor;
            set { _outlineColor = value; MarkDirtyRepaint(); }
        }

        [UxmlAttribute("show-center-dot")]
        public bool ShowCenterDot
        {
            get => _showCenterDot;
            set { _showCenterDot = value; MarkDirtyRepaint(); }
        }

        [UxmlAttribute("show-outline")]
        public bool ShowOutline
        {
            get => _showOutline;
            set { _showOutline = value; MarkDirtyRepaint(); }
        }

        [UxmlAttribute("use-round-caps")]
        public bool UseRoundCaps
        {
            get => _useRoundCaps;
            set { _useRoundCaps = value; MarkDirtyRepaint(); }
        }

        public VectorCrosshair()
        {
            // Crosshairs shouldn't block raycasts/pointer events on underlying UI
            pickingMode = PickingMode.Ignore;

            style.width = 128;
            style.height = 128;

            generateVisualContent += OnGenerateVisualContent;
        }

        /// <summary>
        /// Smoothly updates dynamic recoil/movement spread from gameplay code.
        /// </summary>
        public void SetDynamicSpread(float targetSpread)
        {
            Spread = targetSpread;
        }

        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            if (width <= 0f || height <= 0f)
                return;

            var painter = mgc.painter2D;
            Vector2 center = new Vector2(width * 0.5f, height * 0.5f);
            LineCap cap = _useRoundCaps ? LineCap.Round : LineCap.Butt;

            // 1. Optional dark outline pass for readability over bright backgrounds
            if (_showOutline && _outlineThickness > 0f)
            {
                painter.strokeColor = _outlineColor;
                painter.lineCap = cap;
                painter.lineJoin = LineJoin.Round;

                if (_lineLength > 0f)
                {
                    painter.lineWidth = _lineThickness + _outlineThickness * 2f;
                    DrawProngs(painter, center, _spread, _spread + _lineLength);
                }

                if (_ringRadius > 0f)
                {
                    painter.lineWidth = _lineThickness + _outlineThickness * 2f;
                    DrawRing(painter, center, _ringRadius);
                }

                if (_showCenterDot && _centerDotRadius > 0f)
                {
                    painter.fillColor = _outlineColor;
                    DrawFilledCircle(painter, center, _centerDotRadius + _outlineThickness);
                }
            }

            // 2. Main foreground pass
            painter.strokeColor = _color;
            painter.lineCap = cap;
            painter.lineJoin = LineJoin.Round;

            if (_lineLength > 0f)
            {
                painter.lineWidth = _lineThickness;
                DrawProngs(painter, center, _spread, _spread + _lineLength);
            }

            if (_ringRadius > 0f)
            {
                painter.lineWidth = _lineThickness;
                DrawRing(painter, center, _ringRadius);
            }

            if (_showCenterDot && _centerDotRadius > 0f)
            {
                painter.fillColor = _color;
                DrawFilledCircle(painter, center, _centerDotRadius);
            }
        }

        private static void DrawProngs(Painter2D painter, Vector2 center, float innerDist, float outerDist)
        {
            // Left
            painter.BeginPath();
            painter.MoveTo(center + Vector2.left * innerDist);
            painter.LineTo(center + Vector2.left * outerDist);
            painter.Stroke();

            // Right
            painter.BeginPath();
            painter.MoveTo(center + Vector2.right * innerDist);
            painter.LineTo(center + Vector2.right * outerDist);
            painter.Stroke();

            // Top (-Y in UI Toolkit layout coordinates)
            painter.BeginPath();
            painter.MoveTo(center + Vector2.down * innerDist);
            painter.LineTo(center + Vector2.down * outerDist);
            painter.Stroke();

            // Bottom (+Y in UI Toolkit layout coordinates)
            painter.BeginPath();
            painter.MoveTo(center + Vector2.up * innerDist);
            painter.LineTo(center + Vector2.up * outerDist);
            painter.Stroke();
        }

        private static void DrawRing(Painter2D painter, Vector2 center, float radius)
        {
            painter.BeginPath();
            painter.Arc(center, radius, 0f, 360f);
            painter.Stroke();
        }

        private static void DrawFilledCircle(Painter2D painter, Vector2 center, float radius)
        {
            painter.BeginPath();
            painter.Arc(center, radius, 0f, 360f);
            painter.ClosePath();
            painter.Fill();
        }
    }
}
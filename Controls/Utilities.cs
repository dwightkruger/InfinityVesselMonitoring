//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;

namespace InfinityGroup.VesselMonitoring.Controls
{
    public class FontMetricsHolder : ICanvasTextRenderer
    {
        public struct Metrics
        {
            public float Ascent;
            public float LineGap;
            public float Descent;
            public float CapHeight;
            public float LowercaseLetterHeight;
            public Rect Bounds;
        }

        public List<Metrics> GlyphRunMetrics = new List<Metrics>();

        CanvasDrawingSession drawingSession;

        public FontMetricsHolder(CanvasDrawingSession ds)
        {
            drawingSession = ds;
        }

        public void DrawGlyphRun(
            Vector2 position,
            CanvasFontFace fontFace,
            float fontSize,
            CanvasGlyph[] glyphs,
            bool isSideways,
            uint bidiLevel,
            object brush,
            CanvasTextMeasuringMode measuringMode,
            string locale,
            string textString,
            int[] custerMapIndices,
            uint textPosition,
            CanvasGlyphOrientation glyphOrientation)
        {

            if (null == glyphs) return;

            Metrics m = new Metrics();
            m.Ascent = fontFace.Ascent;
            m.LineGap = fontFace.LineGap;
            m.Descent = fontFace.Descent;
            m.CapHeight = fontFace.CapHeight;
            m.LowercaseLetterHeight = fontFace.LowercaseLetterHeight;
            m.Bounds = fontFace.GetGlyphRunBounds(
                drawingSession,     // Session
                position,           // Position
                fontSize,
                glyphs,
                isSideways,
                bidiLevel);

            GlyphRunMetrics.Add(m);
        }

        public void DrawStrikethrough(
            Vector2 position,
            float strikethroughWidth,
            float strikethroughThickness,
            float strikethroughOffset,
            CanvasTextDirection textDirection,
            object brush,
            CanvasTextMeasuringMode measuringMode,
            string locale,
            CanvasGlyphOrientation glyphOrientation)
        {
        }

        public void DrawUnderline(
            Vector2 position,
            float underlineWidth,
            float underlineThickness,
            float underlineOffset,
            float runHeight,
            CanvasTextDirection textDirection,
            object brush,
            CanvasTextMeasuringMode measuringMode,
            string locale,
            CanvasGlyphOrientation glyphOrientation)
        {
        }

        public void DrawInlineObject(
            Vector2 baselineOrigin,
            ICanvasTextInlineObject inlineObject,
            bool isSideways,
            bool isRightToLeft,
            object brush,
            CanvasGlyphOrientation glyphOrientation)
        {
        }

        public float Dpi { get { return 96; } }

        public bool PixelSnappingDisabled { get { return false; } }

        public Matrix3x2 Transform { get { return System.Numerics.Matrix3x2.Identity; } }
    }

    public static class Utilities
    {
        public static Rect CalculateStringBoundingRectangle(CanvasControl sender, CanvasDrawEventArgs args, string value, CanvasTextFormat textFormat)
        {
            FontMetricsHolder fmh = new FontMetricsHolder(args.DrawingSession);

            CanvasTextLayout textLayout = new CanvasTextLayout(sender, value, textFormat, (float)sender.ActualWidth, (float)sender.ActualHeight);

            textLayout.DrawToTextRenderer(fmh, new Vector2(0, 0));
            float height = 0;
            float width = 0;
            foreach (var metrics in fmh.GlyphRunMetrics)
            {
                height = height + (float)metrics.Bounds.Height;
                width = Math.Max(width, (float)metrics.Bounds.Width);
            }

            return new Rect(0, 0, width, height);
        }
    }

}

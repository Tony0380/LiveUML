using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using LiveUML.Models;

namespace LiveUML.Rendering
{
    public class UmlRenderer
    {
        private static readonly Color BoxFill = Color.FromArgb(255, 255, 230);
        private static readonly Color HeaderFill = Color.FromArgb(70, 130, 180);
        private static readonly Color BorderColor = Color.FromArgb(60, 60, 60);
        private static readonly Color LineColor = Color.FromArgb(100, 100, 100);

        private static readonly Font HeaderFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        private static readonly Font AttributeFont = new Font("Consolas", 8.5f, FontStyle.Regular);
        private static readonly Font LabelFont = new Font("Segoe UI", 7.5f, FontStyle.Regular);

        private const int HeaderHeight = 28;
        private const int AttributeLineHeight = 18;
        private const int TextPadding = 6;

        public void Render(Graphics g, DiagramLayout layout)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            foreach (var line in layout.RelationshipLines)
            {
                DrawRelationshipLine(g, line);
            }

            foreach (var box in layout.EntityBoxes)
            {
                DrawEntityBox(g, box);
            }
        }

        private void DrawEntityBox(Graphics g, EntityBox box)
        {
            var bounds = box.Bounds;

            using (var fill = new SolidBrush(BoxFill))
            {
                g.FillRectangle(fill, bounds);
            }

            var headerRect = new Rectangle(bounds.X, bounds.Y, bounds.Width, HeaderHeight);
            using (var headerBrush = new SolidBrush(HeaderFill))
            {
                g.FillRectangle(headerBrush, headerRect);
            }

            using (var pen = new Pen(BorderColor, 1.5f))
            {
                g.DrawRectangle(pen, bounds);
                g.DrawLine(pen, bounds.X, bounds.Y + HeaderHeight, bounds.Right, bounds.Y + HeaderHeight);
            }

            var headerTextRect = new RectangleF(bounds.X + TextPadding, bounds.Y + 4, bounds.Width - TextPadding * 2, HeaderHeight - 4);
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(box.DisplayName, HeaderFont, Brushes.White, headerTextRect, sf);
            }

            int y = bounds.Y + HeaderHeight + TextPadding;
            foreach (var attr in box.AttributeLines)
            {
                g.DrawString(attr, AttributeFont, Brushes.Black, bounds.X + TextPadding, y);
                y += AttributeLineHeight;
            }

            if (box.AttributeLines.Count == 0)
            {
                using (var grayBrush = new SolidBrush(Color.Gray))
                {
                    g.DrawString("(no attributes selected)", AttributeFont, grayBrush, bounds.X + TextPadding, y);
                }
            }
        }

        private void DrawRelationshipLine(Graphics g, RelationshipLine line)
        {
            using (var pen = new Pen(LineColor, 1.5f))
            {
                if (line.Type == RelationshipType.ManyToMany)
                {
                    pen.DashStyle = DashStyle.Dash;
                }

                pen.CustomEndCap = new AdjustableArrowCap(5, 5);
                g.DrawLine(pen, line.StartPoint, line.EndPoint);
            }

            if (line.Type != RelationshipType.ManyToMany)
            {
                DrawDiamond(g, line.StartPoint, line.EndPoint);
            }

            var midX = (line.StartPoint.X + line.EndPoint.X) / 2;
            var midY = (line.StartPoint.Y + line.EndPoint.Y) / 2;

            using (var bgBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
            {
                var labelSize = g.MeasureString(line.Label, LabelFont);
                var labelRect = new RectangleF(midX - labelSize.Width / 2 - 2, midY - labelSize.Height / 2 - 1, labelSize.Width + 4, labelSize.Height + 2);
                g.FillRectangle(bgBrush, labelRect);
            }

            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                g.DrawString(line.Label, LabelFont, Brushes.DarkSlateGray, midX, midY, sf);
            }
        }

        private void DrawDiamond(Graphics g, Point start, Point end)
        {
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length < 1) return;

            float ux = dx / length;
            float uy = dy / length;

            float size = 8;
            var points = new PointF[]
            {
                new PointF(start.X, start.Y),
                new PointF(start.X + ux * size + uy * size * 0.4f, start.Y + uy * size - ux * size * 0.4f),
                new PointF(start.X + ux * size * 2, start.Y + uy * size * 2),
                new PointF(start.X + ux * size - uy * size * 0.4f, start.Y + uy * size + ux * size * 0.4f)
            };

            using (var brush = new SolidBrush(Color.White))
            using (var pen = new Pen(LineColor, 1f))
            {
                g.FillPolygon(brush, points);
                g.DrawPolygon(pen, points);
            }
        }
    }
}

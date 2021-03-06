﻿using System;
using System.Collections.Generic;
using System.Linq;
using NetCoreGui.Base;
using NetCoreGui.Controls;
using NetCoreGui.Controls.Container;
using NetCoreGui.Controls.Layout;
using NetCoreGui.Drawing;
using NetCoreGui.Drivers;
using NetCoreGui.Utility;
using SFML.Graphics;

namespace NetCoreGui.Themes
{
    public abstract class Theme
    {
        #region Properties
        public IGraphicsContext GraphicsContext { get; set; }
        public Image AppIcon { get; set; }

        public Color BackColor { get; set; }
        public Color TextColor { get; set; }

        public Font Font { get; set; }
        public float FontSize { get; set; }

        public int ControlBorderWidth { get; set; }
        public Color ControlColor { get; set; }
        public Color ControlBorderColor { get; set; }
        public Color ControlBorderHighlitedColor { get; set; }

        public Color SelectionColor { get; set; }
        public Color SelectionTextColor { get; set; }
        public Color MenuSeperatorColor { get; set; }

        public Color TextBoxBackColor { get; set; }
        public Color ButtonBackColor { get; set; }
        #endregion

        public Theme()
        {
            BackColor = ColorUtil.GetSfmlColor("#F5F5F5");
            ControlColor = ColorUtil.GetSfmlColor("#EEEEF2");
            ControlBorderColor = ColorUtil.GetSfmlColor("#00A8E4");

            TextBoxBackColor = Color.White;
            ButtonBackColor = ColorUtil.GetSfmlColor("#DED8CD");

            Font = new Font("Resources/Fonts/Roboto/Roboto-Regular.ttf");

            ControlBorderWidth = 1;
        }

        #region Control Drawing Functions

        public virtual void DrawTextBox(TextBox control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, TextBoxBackColor, ControlBorderColor, ControlBorderWidth);
            GraphicsContext.DrawText(control.Text, prop.Position.x + 10, prop.Position.y + 6);
        }

        public virtual void DrawButton(Button control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.ControlColor, prop.BorderColor, ControlBorderWidth);
            GraphicsContext.DrawText(control.Text, prop.Position.x + 10, prop.Position.y + 6);
        }

        public virtual void DrawControl(Control control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.ControlColor, prop.BorderColor, ControlBorderWidth);
            GraphicsContext.DrawText(control.Text, prop.Position.x, prop.Position.y);
        }
               
        public virtual void DrawLabel(Label control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawText(control.Text, prop.Position.x, prop.Position.y);
        }

        public virtual void DrawForm(Form control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.BackColor);
        }

        public virtual bool DrawColumnLayout(ColumnLayout control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.BackColor);

            Dictionary<int, List<Control>> columns = new Dictionary<int, List<Control>>();
            
            int totalHeight = 0;
            int column = 1;
            
            foreach (var item in control.Chields)
            {
                totalHeight += item.Size.Height;
                
                if(totalHeight >= prop.Size.Height)
                {
                    totalHeight = item.Size.Height;
                    column++;
                }

                if(columns.Keys.Contains(column) == false)
                {
                    columns.Add(column, new List<Control>());
                }                
                columns[column].Add(item);
            }

            var colX = prop.Padding.Left;
            
            foreach (var item in columns)
            {
                var colControls = item.Value;
                var lastY = control.Padding.Top;

                foreach (var cc in colControls)
                {
                    cc.Position.x = colX;
                    cc.Position.y = lastY;
                    lastY += cc.Size.Height;
                }

                colX += colControls.Max(x => x.Size.Width);
            }

            return true;
        }

        public virtual bool DrawRowLayout(RowLayout control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.BackColor);

            Dictionary<int, List<Control>> rows = new Dictionary<int, List<Control>>();

            int totalWidth = 0;
            int row = 1;

            foreach (var item in control.Chields)
            {
                totalWidth += item.Size.Width;

                if (totalWidth >= prop.Size.Width)
                {
                    totalWidth = item.Size.Width;
                    row++;
                }

                if (rows.Keys.Contains(row) == false)
                {
                    rows.Add(row, new List<Control>());
                }
                rows[row].Add(item);
            }

            var colY = prop.Padding.Left;

            foreach (var item in rows)
            {
                var colControls = item.Value;
                var lastX = control.Padding.Left;

                foreach (var cc in colControls)
                {
                    cc.Position.x = lastX;
                    cc.Position.y = colY;
                    lastX += cc.Size.Width;
                }

                colY += colControls.Max(x => x.Size.Height);
            }

            return true;
        }

        private bool DrawGridLayout(GridLayout control)
        {
            Properties prop = control.GetProperties(this);
            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, prop.BackColor);

            var controlX = 0 + control.Padding.Left;
            var controlY = 0 + control.Padding.Top;

            foreach (var row in control.Chields)
            {
                row.Position.x = controlX;
                row.Position.y = controlY;

                var rowMaxHeight = row.Size.Height;
                var max1 = row.Chields.Max(x=>x.Size.Height);
                var max2 = row.Chields.SelectMany(x => x.Chields).Max(x => x.Size.Height);

                rowMaxHeight = Math.Max(rowMaxHeight, max1);
                rowMaxHeight = Math.Max(rowMaxHeight, max2);

                row.Size.Height = rowMaxHeight;
                row.Size.Width = prop.Size.Width;
                controlY += row.Size.Height;
            }

            return false;
        }
        
        private void DrawGridRow(GridRow control)
        {
            var rowProp = control.GetProperties(this);
            var rowWidth = rowProp.Size.Width;

            GraphicsContext.DrawRect(rowProp.Position.x, rowProp.Position.y, rowProp.Size.Width, rowProp.Size.Height, rowProp.BackColor);

            var colX = 0 + control.Padding.Left;
            var colY = 0 + control.Padding.Top;
            
            foreach (var col in control.Chields)
            {
                var gCol = (GridCol)col;

                col.Size.Width = (rowWidth / 12) * (int)gCol.ColSize;
                col.Size.Height = col.Chields.Max(x => x.Size.Height);

                col.Position.y = colY;
                col.Position.x = colX;

                colX += col.Size.Width;
            }            
        }

        private void DrawGridCol(GridCol control)
        {
            var colProp = control.GetProperties(this);            
            GraphicsContext.DrawRect(colProp.Position.x, colProp.Position.y, colProp.Size.Width, colProp.Size.Height, colProp.BackColor);
        }

        public virtual void DrawPanel(Control control)
        {
            Dictionary<Alignment, List<Control>> alignedControls = new Dictionary<Alignment, List<Control>>();
            alignedControls[Alignment.BottomCenter] = new List<Control>();
            alignedControls[Alignment.BottomLeft] = new List<Control>();
            alignedControls[Alignment.BottomRight] = new List<Control>();
            alignedControls[Alignment.Center] = new List<Control>();
            alignedControls[Alignment.CenterLeft] = new List<Control>();
            alignedControls[Alignment.CenterRight] = new List<Control>();
            alignedControls[Alignment.TopCenter] = new List<Control>();
            alignedControls[Alignment.TopLeft] = new List<Control>();
            alignedControls[Alignment.TopRight] = new List<Control>();
            
            Properties prop = control.GetProperties(this);
            Color backColor = prop.ControlColor;

            if(prop.BackColor != Colors.Default)
            {
                backColor = prop.BackColor;
            }

            foreach (var item in control.Chields)
            {
                if(item.Position.x == 0 && item.Position.y == 0)
                {
                    alignedControls[item.Alignment].Add(item);
                }
            }

            var startX = 0;
            var startY = 0;

            foreach (var item in alignedControls[Alignment.TopLeft])
            {
                item.Position.x = startX;
                item.Position.y = startY;
                startX += item.Size.Width + item.Margin.Right;

            }

            startX = 0;
            startY = control.Size.Height - control.Padding.Bottom;

            foreach (var item in alignedControls[Alignment.BottomLeft])
            {
                item.Position.x = startX;
                item.Position.y = startY - item.Size.Height;
                startX += item.Size.Width + item.Margin.Right;
            }

            startX = control.Size.Width - control.Padding.Right;
            startY = 0;

            foreach (var item in alignedControls[Alignment.TopRight])
            {
                item.Position.x = startX - item.Size.Width;
                item.Position.y = startY ;
                startX -= item.Size.Width;
            }

            startX = control.Size.Width - control.Padding.Right;
            startY = control.Size.Height - control.Padding.Bottom;

            foreach (var item in alignedControls[Alignment.BottomRight])
            {
                item.Position.x = startX - item.Size.Width;
                item.Position.y = startY-item.Size.Height;
                startX -= item.Size.Width;
            }


            GraphicsContext.DrawRect(prop.Position.x, prop.Position.y, prop.Size.Width, prop.Size.Height, backColor, prop.BorderColor, ControlBorderWidth);            
        }

        #endregion

        public virtual void RenderControls(List<Control> chields)
        {
            var orderdControlList = chields.OrderBy(x => x.ZedIndex).ToList();

            foreach (var item in orderdControlList)
            {

                switch (item)
                {
                    case Button control:
                        DrawButton(control);
                        break;

                    case TextBox control:
                        DrawTextBox(control);
                        break;

                    case Label control:
                        DrawLabel(control);
                        break;

                    case Form control:
                        DrawForm(control);
                        break;

                    case ColumnLayout control:
                        DrawColumnLayout(control);                        
                        break;

                    case RowLayout control:
                        DrawRowLayout(control);                        
                        break;

                    case GridRow control:
                        DrawGridRow(control);
                        break;

                    case GridCol control:
                        DrawGridCol(control);
                        break;

                    case GridLayout control:
                        DrawGridLayout(control);
                        break;
                    case Panel control:
                        DrawPanel(control);
                        break;
                    default:
                        DrawControl(item);
                        break;
                }

                if (item.Chields != null && item.Chields.Count > 0)
                {
                    RenderControls(item.Chields);
                }
            }
        }

    }
}

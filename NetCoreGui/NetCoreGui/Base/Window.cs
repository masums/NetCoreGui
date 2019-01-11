﻿using NetCoreGui.Drivers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreGui.Base
{
    public enum WindowState
    {
        Active,
        Inactive,
        Minimized,
        Restored,
        Maximized
    }

    public enum WindowStartupPosition
    {
        CenterScreen,
        CenterParent,
        LastPosition
    }
    
    public class Window : Control
    {
        internal GlfwGraphicsDriver _graphicsDriver;

        public string Title { get; set; }
        public string Icon { get; set; }
        public int CurrentZedIndex { get; set; }
        public GraphicsContext GraphicsContext { get; set; }

        public WindowState State { get; set; }
        public Monitor Monitor { get; set; }
        
        public Window(string title, Monitor monitor, Window parent = null, Rect position = null)
        {
            _graphicsDriver = new GlfwGraphicsDriver();
            Title = title;
            Monitor = monitor;
            Parent = parent;
            Position = position;
            
            if (position == null)
            {
                SetDefaultSizeAndPosition();   
            }

            State = WindowState.Active;
        }

        private void SetDefaultSizeAndPosition()
        {
            var y = (int) Monitor.ResolutionY / 4;
            var x = (int) Monitor.ResolutionX / 4;
            Position = new Rect(x, y, x + 400, y + 400);
        }

        internal void Show()
        {
            
        }

        internal void Start(int lastZedIndex)
        {
            ZedIndex = CurrentZedIndex = lastZedIndex;
            GraphicsContext = _graphicsDriver.CreateWindow(Title, new Size(Position.Right-Position.Left, Position.Bottom-Position.Top));
            Glfw3.Glfw.SetWindowRefreshCallback(GraphicsContext.GlfwWindow, (w) => {

                int width, heihgt;
                Glfw3.Glfw.GetWindowSize(w, out width, out heihgt);                
                GraphicsContext.Canvas2d.Clear(SKColors.WhiteSmoke);
                GraphicsContext.Canvas2d.Flush();
            });
        }
    }
}

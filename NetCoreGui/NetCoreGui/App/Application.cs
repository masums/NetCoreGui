﻿using System;
using System.Collections.Generic;
using System.Threading;

using NetCoreGui.Themes;
using NetCoreGui.Drivers;
using System.Diagnostics;
using System.Linq;
using NetCoreGui.Base;

namespace NetCoreGui.Utility
{
    public class Application
    {
        internal IGraphicsDriver graphicsDriver;
        public static Type ThemeType { get; set; }

        internal IntPtr consoleHandle;
        internal int _lastZedIndex = 1;

        internal bool _isInitilized = false;        
        internal bool _isRunning = true;

        public List<IWindow> Windows { get; set; }
        
        public static Application Create()
        {
            var app = new Application();
            app.consoleHandle = Process.GetCurrentProcess().Handle;
            app.graphicsDriver = new GraphicsDriver();
            ThemeType = typeof(DefaultTheme);

            return app;
        }

        /// <summary>
        /// Begin the application from here
        /// </summary>
        /// <param name="window">The window which will show</param>
        public void Bismillah(IWindow window){

            _lastZedIndex  = _lastZedIndex + 10000;
            window.Create(_lastZedIndex);
            var nativeWindow = window.Theme.GraphicsContext.Window;

            while (nativeWindow.IsOpen)
            {
                var windows = WindowManager.GetWindows().Where(x => x.State != WindowState.Minimized).ToList();
                foreach (var item in windows)
                {
                    var nWin = item.Theme.GraphicsContext.Window;
                    nWin.Clear(window.Theme.BackColor);
                    nWin.DispatchEvents();
                    item.DrawControls();
                    nWin.Display();
                }

                Thread.Sleep(3);
            }
        }

        public Application UseTheme(Type themeType)
        {
            ThemeType = themeType;
            return this;
        }
    }
}
